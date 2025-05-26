from databases import Database
from fastapi import HTTPException

class TeamDatabaseService:
    def __init__(self, connection_string):
        self.db = Database(connection_string)
        
    async def connect(self):
        await self.db.connect()
        
    async def disconnect(self):
        await self.db.disconnect()
        
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

        # Map level name to integer for SQL filter
        level_map = {"Junior": 0, "Mid": 1, "Senior": 2, "Architect": 3}
        level = level_map.get(level_name) if level_name else None
        level_filter = f" AND esr.level = {level}" if level is not None else ""

        role_filter = f" AND sr.name = '{role}'" if role else ""
        area_filter = f" AND ta.name = '{area}'" if area else ""

        tech_list = technologies or []
        tech_str = "','".join(tech_list)
        tech_filter = (
            f" AND EXISTS ("
            f"SELECT 1 FROM public.employee_technologies et JOIN public.technologies tech ON tech.id = et.technology_id "
            f"WHERE et.employee_profile_id = ep.id AND tech.name IN ('{tech_str}'))"
            if tech_list
            else ""
        )

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
        level_map = {"Junior": 0, "Mid":1, "Senior":2, "Architect":3}
        req_clauses = []
        for req in requirements:
            lvl = level_map.get(req.get("Level"), None)
            if req.get("Role") and req.get("Area") and lvl is not None:
                req_clauses.append(
                    f"(sr.name = '{req['Role']}' AND ta.name = '{req['Area']}' AND esr.level = {lvl})"
                )
        req_filter = f"AND ({' OR '.join(req_clauses)})" if req_clauses else ""

        # Technologies filter
        tech_list = technologies or []
        tech_str = "','".join(tech_list)
        tech_filter = (
            f"AND EXISTS (SELECT 1 FROM public.employee_technologies et JOIN public.technologies tech ON tech.id = et.technology_id "
            f"WHERE et.employee_profile_id = ep.id AND tech.name IN ('{tech_str}'))"
            if tech_list else ""
        )

        verification_value = 2

        # Availability and SFIA level filter
        avail_filter = f"AND ep.availability = {str(availability).lower()}"
        sfia_filter = f"AND ep.sfia_level_general >= {min_sfia_level}" if min_sfia_level is not None else ""
        verification_filter = f"AND ep.verification_status = '{verification_value}'"

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
        ORDER BY ep.sfia_level_general DESC
        LIMIT 20;
        """
        try:
            rows = await self.db.fetch_all(sql)
            return [dict(r) for r in rows]
        except Exception as e:
            raise HTTPException(status_code=500, detail=str(e))
