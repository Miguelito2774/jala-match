from typing import List
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel, Field
import os
import json
from dotenv import load_dotenv
from anthropic import Anthropic
from fastapi.middleware.cors import CORSMiddleware

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
        allow_population_by_field_name = True

class TechnicalRoleSpec(BaseModel):
    role: str = Field(..., alias="Role")  
    area: str = Field(..., alias="Area")  
    level: str = Field(..., alias="Level") 

    class Config:
        allow_population_by_field_name = True

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
        allow_population_by_field_name = True

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
        allow_population_by_field_name = True
        
class TeamMemberCompatibilityRequest(BaseModel):
    team: dict = Field(alias="Team")
    new_member: TeamMemberData = Field(alias="NewMember")

    class Config:
        populate_by_name = True
        allow_population_by_field_name = True

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
        allow_population_by_field_name = True


@app.post("/generate-teams")
async def generate_teams(request: TeamGenerationRequest):
    try:
        print("Received request:", request.dict())
        
        roles_list = [req.role for req in request.requirements]
        
        members_query = f"""
        De mi base de datos necesito obtener empleados que cumplan estos criterios:
        
        1. Roles: {roles_list}
        2. Tecnologías: {request.technologies}
        3. Nivel SFIA mínimo: {request.sfia_level}
        4. Disponibilidad: {request.availability}
        
        Relaciones importantes:
        - La tabla principal es employee_profiles que contiene id, name, role, sfia_level, mbti, timezone, country y availability
        - Las tecnologías están en employee_technologies relacionada con technology
        - Los idiomas están en employee_language
        - Los intereses están en personal_interests
        - La experiencia laboral está en work_experience
        - employee_profiles -> employee_specialized_roles (roles y niveles)
        - employee_specialized_roles -> specialized_roles -> technical_areas (área técnica)
        - employee_technologies -> technologies (skills técnicos)
        
        Por favor:
        - Trae al menos 10 empleados que mejor cumplan los criterios
        - Cada empleado debe tener: id, name, role, technologies (array), sfia_level, mbti, interests (array), timezone y country
        - Si un empleado cumple con al menos una tecnología solicitada, inclúyelo
        - Prioriza empleados con mayor número de tecnologías coincidentes
        - Solo devuelve datos en formato JSON
        """
        
        employees_data = await ask_ia(members_query)
        
        if not employees_data or not isinstance(employees_data, list) or len(employees_data) == 0:
            raise HTTPException(status_code=500, detail="No se pudieron obtener datos de empleados desde MCP")
        
        print(f"Obtenidos {len(employees_data)} empleados desde MCP")
        
        client = Anthropic(api_key=os.getenv("CLAUDE_API_KEY"))

        employees_json = json.dumps(employees_data, ensure_ascii=False)
        
        prompt = f"""
        # Análisis de Formación de Equipos

        ## Datos de Empleados Disponibles
        ```json
        {employees_json}
        ```

        ## Requisitos del Equipo
        - Tamaño del equipo: {request.team_size}
-        - Roles requeridos: {[role.role for role in request.requirements]}
+        - Areas requeridas: {[req.area for req in request.requirements]}
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