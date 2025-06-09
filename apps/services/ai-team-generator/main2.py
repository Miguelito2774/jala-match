from typing import List
from fastapi import FastAPI, HTTPException
from pydantic import UUID4, BaseModel, Field
import os
import json
from dotenv import load_dotenv
from anthropic import Anthropic
from fastapi.middleware.cors import CORSMiddleware
from team_db_service import TeamDatabaseService

from init import ask_ia 

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:5001"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

load_dotenv()


class TeamMemberData(BaseModel):
    id: str = Field(alias="Id")
    name: str = Field(alias="Name")
    role: str = Field(alias="Role")
    technologies: list[str] = Field(alias="Technologies")
    sfia_level: int = Field(alias="SfiaLevel")
    mbti: str = Field(default=None, alias="Mbti")
    interests: list[str] = Field(default_factory=list, alias="Interests")
    timezone: str = Field(default=None, alias="Timezone")
    country: str = Field(default=None, alias="Country")

    class Config:
        populate_by_name = True

class TechnicalRoleSpec(BaseModel):
    role: str = Field(..., alias="Role")  
    area: str = Field(..., alias="Area")  
    level: str = Field(..., alias="Level") 

    class Config:
        populate_by_name = True

class WeightsModel(BaseModel):
    sfia_weight: int = Field(alias="SfiaWeight")
    technical_weight: int = Field(alias="TechnicalWeight")
    psychological_weight: int = Field(alias="PsychologicalWeight")
    experience_weight: int = Field(alias="ExperienceWeight")
    language_weight: int = Field(alias="LanguageWeight")
    interests_weight: int = Field(alias="InterestsWeight")
    timezone_weight: int = Field(alias="TimezoneWeight")

    class Config:
        populate_by_name = True

class TeamGenerationRequest(BaseModel):
    creator_id: str = Field(alias="CreatorId")
    team_size: int = Field(alias="TeamSize")
    requirements: List[TechnicalRoleSpec] = Field(..., alias="Requirements")
    technologies: List[str] = Field(alias="Technologies")
    sfia_level: int = Field(alias="SfiaLevel")
    weights: WeightsModel = Field(alias="Weights")
    availability: bool = Field(alias="Availability")

    class Config:
        populate_by_name = True
        
class TeamMemberCompatibilityRequest(BaseModel):
    team: dict = Field(alias="Team")
    new_member: TeamMemberData = Field(alias="NewMember")

    class Config:
        populate_by_name = True

class TeamAnalysisRequest(BaseModel):
    team_id: str = Field(alias="TeamId")
    leader_id: str = Field(alias="LeaderId")
    team_size: int = Field(alias="TeamSize")
    sfia_level: int = Field(alias="SfiaLevel")
    technologies: list[str] = Field(alias="Technologies")
    roles: list[str] = Field(alias="Roles")
    sfia_weight: int = Field(alias="SfiaWeight")
    technical_weight: int = Field(alias="TechnicalWeight")
    psychological_weight: int = Field(alias="PsychologicalWeight")
    experience_weight: int = Field(alias="ExperienceWeight")
    language_weight: int = Field(alias="LanguageWeight")
    interests_weight: int = Field(alias="InterestsWeight")
    timezone_weight: int = Field(alias="TimezoneWeight")

    class Config:
        populate_by_name = True


class FindTeamMemberRequest(BaseModel):
    team_id: str = Field(alias="TeamId")
    role: str = Field(alias="Role")
    area: str = Field(alias="Area")
    level: str = Field(alias="Level")
    technologies: list[str] = Field(alias="Technologies")

    class Config:
        populate_by_name = True

class TeamMemberRecommendation(BaseModel):
    employee_id: str
    name: str
    role: str
    sfia_level: int
    compatibility_score: int
    analysis: str
    potential_conflicts: list[str]
    team_impact: str

class TeamReanalysisRequest(BaseModel):
    team_id: str = Field(alias="TeamId")
    members: list[dict]
    technologies: list[str]
    weights: dict


class TeamMember(BaseModel):
    profile_id: UUID4
    first_name: str
    last_name: str
    technologies: list[str]
    mbti: str

class TeamData(BaseModel):
    id: UUID4
    name: str
    compatibility_score: int
    members: list[TeamMember]

class Candidate(BaseModel):
    employee_id: UUID4
    name: str
    role: str
    sfia_level: int
    technologies: list[str]


@app.post("/generate-teams")
async def generate_teams(request: TeamGenerationRequest):
    # Initialize database service
    db_service = TeamDatabaseService("postgresql://postgres:postgres@postgres:5432/postgres-db")
    await db_service.connect()
    
    try:
        # Debug logging to see what data we're receiving
        print(f"DEBUG - Request data:")
        print(f"  requirements: {[req.model_dump(by_alias=True) for req in request.requirements]}")
        print(f"  technologies: {request.technologies}")
        print(f"  sfia_level: {request.sfia_level} (type: {type(request.sfia_level)})")
        print(f"  availability: {request.availability} (type: {type(request.availability)})")
        
        employees_data = await db_service.get_generation_candidates(
            [req.model_dump(by_alias=True) for req in request.requirements],
            request.technologies,
            request.sfia_level,
            request.availability
        )
        
        if not employees_data or len(employees_data) == 0:
            raise HTTPException(status_code=404, detail="No se encontraron candidatos que cumplan con los criterios")
        
        client = Anthropic(api_key=os.getenv("CLAUDE_API_KEY"))
        
        employees_json = json.dumps(employees_data, ensure_ascii=False, default=str)
        
        prompt = f"""
        # Análisis de Formación de Equipos

        ## Datos de Empleados Disponibles
        ```json
        {employees_json}
        ```

        ## Requisitos del Equipo
        - Tamaño del equipo: {request.team_size}
        - Roles requeridos: {[role.role for role in request.requirements]}
        - Areas requeridas: {[req.area for req in request.requirements]}
        - Tecnologías requeridas: {request.technologies}
        - Nivel SFIA mínimo: {request.sfia_level}

        ## Criterios de Evaluación (Ponderados)
        - Compatibilidad de Nivel SFIA: {request.weights.sfia_weight}%
        - Compatibilidad Técnica: {request.weights.technical_weight}%
        - Compatibilidad Psicológica basada en MBTI: {request.weights.psychological_weight}%
        - Nivel de Experiencia: {request.weights.experience_weight}%
        - Dominio del Idioma: {request.weights.language_weight}%
        - Compatibilidad basada en Intereses: {request.weights.interests_weight}%
        - Consideración de Zona Horaria: {request.weights.timezone_weight}%

        ## Principios de Formación de Equipos
        1. Asegurar cobertura de roles con niveles SFIA apropiados
        2. Equilibrar experiencia técnica en las tecnologías requeridas
        3. Crear diversidad psicológica mientras se minimizan patrones de conflicto severos
        4. Incorporar intereses complementarios para la cohesión del equipo
        5. Equilibrar niveles de experiencia (junior a senior) para transferencia de conocimiento
        6. Considerar superposición de zonas horarias para colaboración efectiva
        7. Incluir entre 3-5 miembros recomendados en 'recommended_Members' que:
        - No fueron seleccionados en el equipo principal
        - Tengan al menos 70% de compatibilidad
        - Complementen las habilidades del equipo principal
        - Sean relevantes para los roles y tecnologías requeridas

        ### IMPORTANTE:
        - UTILIZA ÚNICAMENTE los empleados proporcionados en los datos JSON arriba
        - NO inventes empleados ni información que no esté en los datos proporcionados
        - Si no hay suficientes empleados para cumplir con todos los criterios, usa los mejores disponibles
        - El equipo debe formarse con los empleados reales de los datos proporcionados

        ## Formato de Respuesta Requerido
        Responde EXCLUSIVAMENTE con un objeto JSON con la siguiente estructura exacta, sin texto adicional:

        ```json
        {{
          "teams": [
            {{
              "team_id": "guid-aleatorio-aquí",
              "members": [
                {{
                  "id": "guid-del-miembro-real-de-los-datos",
                  "name": "Nombre del Miembro",
                  "role": "Rol del Miembro",
                  "sfia_level": 5
                }}
              ]
            }}
          ],
          "recommended_leader": {{
            "id": "guid-del-líder-real-de-los-datos",
            "name": "Nombre del Líder",
            "rationale": "Justificación detallada de por qué esta persona es el líder recomendado"
          }},
          "team_analysis": {{
            "strengths": [
              "Fortaleza 1 explicada en detalle",
              "Fortaleza 2 explicada en detalle",
              "Fortaleza 3 explicada en detalle"
            ],
            "weaknesses": [
              "Debilidad 1 explicada en detalle",
              "Debilidad 2 explicada en detalle"
            ],
            "compatibility": "Justificación extensa y detallada sobre la compatibilidad del equipo, considerando los criterios técnicos, psicológicos y operativos. Analiza en profundidad cómo los perfiles se complementan entre sí y por qué funcionarían bien juntos como un equipo multidisciplinario."
          }},
          "compatibility_score": 85,
          "recommended_Members": [
            {{
              "id": "guid-del-miembro-recomendado-real-de-los-datos",
              "name": "Nombre del Miembro",
              "compatibility_score": 75,
              "analysis": "Justificación detallada de por qué sería una buena adición al equipo, incluyendo fortalezas complementarias y sinergias",
              "potential_conflicts": [
                "Conflicto potencial 1 detallado que requiere atención",
                "Conflicto potencial 2 detallado que requiere atención"
              ],
              "team_impact": "Análisis detallado de cómo este nuevo miembro cambiaría la dinámica del equipo, incluyendo aspectos positivos y posibles desafíos"
            }}
          ]
        }}
        ```
        
        IMPORTANTE:
        1. Tu respuesta debe ser ÚNICAMENTE el objeto JSON válido, sin texto explicativo.
        2. Las fortalezas y debilidades deben ser explicaciones detalladas, no solo puntos breves.
        3. La justificación de compatibilidad debe ser un análisis profundo de la dinámica del equipo.
        4. El team_id debe ser un GUID aleatorio en formato estándar (por ejemplo, "123e4567-e89b-12d3-a456-426614174000").
        5. Para los miembros del equipo, usa EXACTAMENTE los mismos ids que aparecen en los datos de empleados proporcionados.
        6. La respuesta debe estar en español.
        7. Solo puedes incluir empleados que existan en los datos proporcionados.
        """

        response = client.messages.create(
            model="claude-3-5-sonnet-20241022",
            max_tokens=4000,
            temperature=0.2,
            messages=[{"role": "user", "content": prompt}],
        )

        try:
            team_formation_result = json.loads(response.content[0].text)
            return team_formation_result
        except Exception as e:
            raise HTTPException(status_code=500, detail=f"Error al analizar resultado de formación de equipo: {str(e)}")

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        await db_service.disconnect()

@app.post("/find-team-members")
async def find_team_members(request: FindTeamMemberRequest):
    # Initialize database service
    db_service = TeamDatabaseService("postgresql://postgres:postgres@postgres:5432/postgres-db")
    await db_service.connect()
    
    try:
        # Debug logging to see what data we're receiving
        print(f"DEBUG - Find team members request:")
        print(f"  team_id: {request.team_id}")
        print(f"  role: {request.role}")
        print(f"  area: {request.area}")
        print(f"  level: {request.level} (type: {type(request.level)})")
        print(f"  technologies: {request.technologies}")
        
        # Get team data reliably with direct database access
        team_data = await db_service.get_team_data(request.team_id)
        if not team_data:
            raise HTTPException(status_code=404, detail="No se encontró información del equipo")
          
        # Get candidates reliably with direct database access
        candidates_data = await db_service.get_team_candidates(
            request.team_id,
            request.role,
            request.area,
            request.level,
            request.technologies,
        )
        
        if not candidates_data or len(candidates_data) == 0:
            raise HTTPException(status_code=404, detail="No se encontraron candidatos que cumplan con los criterios")
        
        # Use direct Claude API for analysis instead of ask_ia to avoid inconsistencies
        client = Anthropic(api_key=os.getenv("CLAUDE_API_KEY"))
        
        # Format data for the prompt
        team_json = json.dumps(team_data, ensure_ascii=False, default=str)
        candidates_json = json.dumps(candidates_data, ensure_ascii=False, default=str)
        technologies_json = json.dumps(request.technologies, ensure_ascii=False)
        
        # Create prompt for compatibility analysis
        prompt = f"""
        # Análisis de Compatibilidad de Nuevos Miembros para Equipo Existente

        ## Datos del Equipo Actual
        ```json
        {team_json}
        ```

        ## Datos de Candidatos Disponibles
        ```json
        {candidates_json}
        ```

        ## Criterios de Búsqueda
        - Rol buscado: {request.role}
        - Área técnica: {request.area}
        - Nivel requerido: {request.level}
        - Tecnologías requeridas: {technologies_json}

        ## Instrucciones
        1. Evalúa los candidatos según los criterios de pesos del equipo actual
        2. Para cada candidato, calcula un puntaje de compatibilidad (0-100) dando especial importancia a:
           - Compatibilidad con las tecnologías requeridas ({technologies_json})
           - Nivel SFIA adecuado para el rol
           - Complementariedad con los perfiles MBTI actuales del equipo
        3. Analiza cómo cada candidato complementaría al equipo actual considerando:
           - Compatibilidad técnica (stack tecnológico)
           - Compatibilidad de personalidad (MBTI)
           - Experiencia relevante
           - Complementariedad con las fortalezas y debilidades del equipo actual
        4. Selecciona los 5 mejores candidatos según su puntaje de compatibilidad

        ### IMPORTANTE:
        - UTILIZA ÚNICAMENTE los candidatos proporcionados en los datos JSON
        - Asegúrate que el candidato no sea ya miembro del equipo
        - Realiza un análisis profundo considerando aspectos técnicos y de compatibilidad psicológica
        - Da mayor peso a candidatos con experiencia en las tecnologías específicamente solicitadas

        ## Formato de Respuesta Requerido
        Responde EXCLUSIVAMENTE con un array JSON con la siguiente estructura exacta, sin texto adicional:

        ```json
        [
          {{
            "employee_id": "guid-real-del-empleado",
            "name": "Nombre Completo",
            "role": "Rol Actual",
            "area": "Área Técnica",
            "technologies": ["Tecnología1", "Tecnología2", "Tecnología3"],
            "sfia_level": 3,
            "compatibility_score": 85,
            "analysis": "Análisis detallado de por qué este candidato sería una buena adición al equipo."
          }}
        ]
        ```
        
        IMPORTANTE:
        1. Tu respuesta debe ser EXACTAMENTE 5 candidatos en un array JSON
        2. Los candidatos deben estar ordenados por puntaje de compatibilidad (de mayor a menor)
        3. No incluyas ningún texto fuera del JSON
        4. Asegúrate de incluir el campo "technologies" como un array de strings
        5. Cada análisis debe ser detallado pero conciso (2-4 oraciones)
        6. Asegúrate de que los IDs de los empleados sean exactamente los proporcionados en los datos
        7. La respuesta debe estar en español
        """

        response = client.messages.create(
            model="claude-3-5-sonnet-20241022",
            max_tokens=4000,
            temperature=0.2,
            messages=[{"role": "user", "content": prompt}],
        )

        try:
            recommendations = json.loads(response.content[0].text)
            return recommendations
        except Exception as e:
            raise HTTPException(status_code=500, detail=f"Error al analizar recomendaciones de candidatos: {str(e)}")

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        await db_service.disconnect()
    

@app.post("/reanalyze-team")
async def reanalyze_team(request: TeamReanalysisRequest):
    try:
        client = Anthropic(api_key=os.getenv("CLAUDE_API_KEY"))
        
        prompt = f"""
        # Re-análisis de Equipo Post-Adición de Miembro
        
        ## Miembros Actuales del Equipo:
        {json.dumps(request.members, indent=2)}
        
        ## Tecnologías Requeridas:
        {', '.join(request.technologies)}
        
        ## Pesos de Compatibilidad:
        {json.dumps(request.weights, indent=2)}
        
        ## Tareas:
        1. Calcular nuevo puntaje de compatibilidad (0-100)
        2. Identificar 3 nuevas fortalezas clave
        3. Detectar 2 posibles debilidades emergentes
        4. Analizar impacto en dinámica del equipo
        5. Proporcionar recomendaciones de mejora
        
        ## Formato Requerido:
        {{
          "new_compatibility_score": 85,
          "updated_strengths": [
            "Fortaleza 1 con detalles técnicos",
            "Fortaleza 2 con justificación psicológica"
          ],
          "updated_weaknesses": [
            "Debilidad 1 relacionada con skills faltantes",
            "Debilidad 2 de coordinación horaria"
          ],
          "detailed_analysis": "Análisis de 200 palabras...",
          "recommendations": [
            "Recomendación 1 para mejorar el equipo",
            "Recomendación 2 de capacitación"
          ]
        }}
        """
        
        response = client.messages.create(
            model="claude-3-5-sonnet-20241022",
            max_tokens=3000,
            temperature=0.1,
            messages=[{"role": "user", "content": prompt}]
        )
        
        return json.loads(response.content[0].text)
        
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
