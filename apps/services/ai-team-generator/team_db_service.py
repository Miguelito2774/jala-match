from databases import Database
from fastapi import HTTPException

class TeamDatabaseService:
    def __init__(self, connection_string):
        self.db = Database(connection_string)
        
    async def connect(self):
        await self.db.connect()
        
    async def disconnect(self):
        await self.db.disconnect()
    
    def _get_privacy_filter(self):
        """
        Returns privacy filter for team matching based on user consent.
        
        GDPR Implementation: Only includes employees who have explicitly 
        consented to 'team_matching_analysis' in their privacy settings.
        
        Impact: Employees with team_matching_analysis=false will NOT appear
        in team suggestions or automatic team generation.
        """
        # Only include employees who have consented to team matching analysis
        return """
          AND EXISTS (
            SELECT 1 FROM public.user_privacy_consents upc
            JOIN public.users u ON u.id = upc.user_id
            WHERE u.id = ep.user_id 
              AND upc.team_matching_analysis = true
          )
        """
        
    async def get_team_data(self, team_id):
        # Direct SQL query for team data
        query = """
        WITH
        team_data AS (
          SELECT
            t.id,
            t.name,
            t.compatibility_score,
            t.ai_analysis,
            t.weight_criteria
          FROM public.teams AS t
          WHERE t.id = :team_id
        ),
        member_data AS (
          SELECT
            tm.team_id,
            jsonb_build_object(
              'profile_id', ep.id,
              'first_name', ep.first_name,
              'last_name', ep.last_name,
              'role', sr.name,
              'sfia_level', ep.sfia_level_general,
              'mbti', ep.mbti,
              'timezone', ep.timezone,
              'country', ep.country,
              'availability', ep.availability,
              'technologies', (
                SELECT jsonb_agg(tech.name)
                FROM public.employee_technologies AS et
                JOIN public.technologies AS tech ON tech.id = et.technology_id
                WHERE et.employee_profile_id = ep.id
              ),
              'interests', (
                SELECT jsonb_agg(pi.name)
                FROM public.personal_interests AS pi
                WHERE pi.employee_profile_id = ep.id
              ),
              'languages', (
                SELECT jsonb_agg(el.language)
                FROM public.employee_languages AS el
                WHERE el.employee_profile_id = ep.id
              )
            ) AS member_json
          FROM public.team_members AS tm
          JOIN public.employee_profiles AS ep ON ep.id = tm.employee_profile_id
          LEFT JOIN public.employee_specialized_roles AS esr ON esr.employee_profile_id = ep.id
          LEFT JOIN public.specialized_roles AS sr ON sr.id = esr.specialized_role_id
          WHERE tm.team_id = :team_id
        )
        SELECT jsonb_build_object(
            'team', (
                SELECT row_to_json(td)
                FROM team_data td
            ),
            'members', (
                SELECT coalesce(jsonb_agg(md.member_json) FILTER (WHERE md.member_json IS NOT NULL), '[]'::jsonb)
                FROM member_data md
            )
        ) AS result
        """
        result = await self.db.fetch_one(query, {"team_id": team_id})
        if not result:
            return None
        return result["result"]
    
    async def get_team_candidates(self, team_id, role, area, level_name, technologies):
        mem_rows = await self.db.fetch_all(
            "SELECT employee_profile_id FROM public.team_members WHERE team_id = :team_id",
            {"team_id": team_id},
        )
        current_ids = [str(r["employee_profile_id"]) for r in mem_rows]
        exclude_filter = (
            f" AND ep.id NOT IN ('{"','".join(current_ids)}')" if current_ids else ""
        )

        # Map level name to string for SQL filter (matching database storage)
        level_map = {"Junior": "Junior", "Staff": "Staff", "Senior": "Senior", "Architect": "Architect"}
        level = level_map.get(level_name) if level_name else None
        level_filter = f" AND esr.level = '{level}'" if level is not None else ""

        role_filter = f" AND sr.name = '{role}'" if role else ""
        
        # Handle multiple areas separated by commas
        if area:
            areas = [area.strip() for area in area.split(",")]
            area_conditions = [f"ta.name = '{a}'" for a in areas if a]
            if area_conditions:
                area_filter = f" AND ({' OR '.join(area_conditions)})"
            else:
                area_filter = ""
        else:
            area_filter = ""

        tech_list = technologies or []
        tech_str = "','".join(tech_list)
        tech_filter = (
            f" AND EXISTS ("
            f"SELECT 1 FROM public.employee_technologies et JOIN public.technologies tech ON tech.id = et.technology_id "
            f"WHERE et.employee_profile_id = ep.id AND tech.name IN ('{tech_str}'))"
            if tech_list
            else ""
        )

        # Privacy filter - only include employees who consent to team matching
        privacy_filter = self._get_privacy_filter()

        sql = f"""
        SELECT
            ep.id AS employee_id,
            ep.first_name || ' ' || ep.last_name AS name,
            sr.name AS role,
            ep.sfia_level_general AS sfia_level,
            ep.mbti,
            ep.timezone,
            ep.country,
            ep.availability,
            ta.name AS technical_area,
            (
                SELECT jsonb_agg(DISTINCT tech.name)
                FROM public.employee_technologies et
                JOIN public.technologies tech ON tech.id = et.technology_id
                WHERE et.employee_profile_id = ep.id
            ) AS technologies,
            (
                SELECT jsonb_agg(DISTINCT pi.name)
                FROM public.personal_interests pi
                WHERE pi.employee_profile_id = ep.id
            ) AS interests,
            EXISTS (
                SELECT 1 FROM public.employee_technologies et
                JOIN public.technologies tech ON tech.id = et.technology_id
                WHERE et.employee_profile_id = ep.id
                  AND tech.name IN ('{tech_str}')
            ) AS has_required_tech
        FROM public.employee_profiles ep
        LEFT JOIN public.employee_specialized_roles esr ON esr.employee_profile_id = ep.id
        LEFT JOIN public.specialized_roles sr ON sr.id = esr.specialized_role_id
        LEFT JOIN public.technical_areas ta ON ta.id = sr.technical_area_id
        WHERE 1=1
          {exclude_filter}
          {role_filter}
          {area_filter}
          {level_filter}
          {tech_filter}
          {privacy_filter}
        ORDER BY has_required_tech DESC, ep.sfia_level_general DESC
        LIMIT 15;
        """
        try:
            rows = await self.db.fetch_all(sql)
            return [dict(r) for r in rows]
        except Exception as e:
            raise HTTPException(status_code=500, detail=str(e))
    
    async def get_generation_candidates(self, requirements, technologies, min_sfia_level, availability):
        # Build OR conditions for each requirement
        # Map level names to their string representations (matching the database)
        level_map = {"Junior": "Junior", "Staff": "Staff", "Senior": "Senior", "Architect": "Architect"}
        req_clauses = []
        
        print(f"DEBUG - get_generation_candidates requirements: {requirements}")
        
        for req in requirements:
            print(f"DEBUG - Processing requirement: {req}")
            raw_level = req.get("Level")
            
            # Handle both string and potential numeric level values
            if isinstance(raw_level, int):
                # If you have a numeric mapping, convert it here
                numeric_to_string = {0: "Junior", 1: "Staff", 2: "Senior", 3: "Architect"}
                level_name = numeric_to_string.get(raw_level)
            else:
                level_name = level_map.get(raw_level)
                
            print(f"DEBUG - Original level: {raw_level}, mapped level: {level_name}")
            
            if req.get("Role") and req.get("Area") and level_name:
                # Handle multiple areas separated by commas
                areas = [area.strip() for area in req.get("Area", "").split(",")]
                area_conditions = []
                for area in areas:
                    if area:  # Skip empty areas
                        area_conditions.append(f"ta.name = '{area}'")
                
                if area_conditions:
                    area_clause = " OR ".join(area_conditions)
                    clause = f"(sr.name = '{req['Role']}' AND ({area_clause}) AND esr.level = '{level_name}')"
                    print(f"DEBUG - Generated clause: {clause}")
                    req_clauses.append(clause)
        req_filter = f"AND ({' OR '.join(req_clauses)})" if req_clauses else ""
        print(f"DEBUG - Final req_filter: {req_filter}")

        # Technologies filter
        tech_list = technologies or []
        tech_str = "','".join(tech_list)
        tech_filter = (
            f"AND EXISTS (SELECT 1 FROM public.employee_technologies et JOIN public.technologies tech ON tech.id = et.technology_id "
            f"WHERE et.employee_profile_id = ep.id AND tech.name IN ('{tech_str}'))"
            if tech_list else ""
        )

        verification_value = "2"  # Approved status as string

        # Availability and SFIA level filter
        avail_filter = f"AND ep.availability = {str(availability).lower()}"
        sfia_filter = f"AND ep.sfia_level_general >= {min_sfia_level}" if min_sfia_level is not None else ""
        verification_filter = f"AND ep.verification_status = '{verification_value}'"
        
        # Privacy filter - only include employees who consent to team matching
        privacy_filter = self._get_privacy_filter()

        sql = f"""
        SELECT
            ep.id AS employee_id,
            ep.first_name || ' ' || ep.last_name AS name,
            sr.name AS role,
            ta.name AS technical_area,
            ep.sfia_level_general AS sfia_level,
            ep.mbti,
            ep.timezone,
            ep.country,
            ep.availability,
            (
                SELECT jsonb_agg(DISTINCT tech.name)
                FROM public.employee_technologies et
                JOIN public.technologies tech ON tech.id = et.technology_id
                WHERE et.employee_profile_id = ep.id
            ) AS technologies,
            (
                SELECT jsonb_agg(DISTINCT pi.name)
                FROM public.personal_interests pi
                WHERE pi.employee_profile_id = ep.id
            ) AS interests
        FROM public.employee_profiles ep
        LEFT JOIN public.employee_specialized_roles esr ON esr.employee_profile_id = ep.id
        LEFT JOIN public.specialized_roles sr ON sr.id = esr.specialized_role_id
        LEFT JOIN public.technical_areas ta ON ta.id = sr.technical_area_id
        WHERE 1=1
          {avail_filter}
          {sfia_filter}
          {verification_filter}
          {req_filter}
          {tech_filter}
          {privacy_filter}
        ORDER BY ep.sfia_level_general DESC
        LIMIT 20;
        """
        try:
            rows = await self.db.fetch_all(sql)
            return [dict(r) for r in rows]
        except Exception as e:
            raise HTTPException(status_code=500, detail=str(e))
