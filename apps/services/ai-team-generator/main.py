from fastapi import FastAPI, HTTPException
from pydantic import BaseModel, Field
import os
import json
from dotenv import load_dotenv
from anthropic import Anthropic
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:5001"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

load_dotenv()

class TechnologyExperience(BaseModel):
    name: str = Field(alias="Name")
    version: str = Field(default=None, alias="Version")
    sfia_level: int = Field(alias="SfiaLevel")
    years_experience: float = Field(alias="YearsExperience")
    experience_level: str = Field(alias="ExperienceLevel")

    class Config:
        populate_by_name = True
        allow_population_by_field_name = True

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
    technology_experiences: list[TechnologyExperience] = Field(default=None, alias="TechnologyExperiences")

    class Config:
        populate_by_name = True
        allow_population_by_field_name = True

class TeamGenerationRequest(BaseModel):
    roles: list[str] = Field(alias="Roles")
    technologies: list[str] = Field(alias="Technologies")
    sfia_level: int = Field(alias="SfiaLevel")
    availability: bool = Field(default=True, alias="Availability")
    members_data: list[TeamMemberData] = Field(alias="MembersData")
    team_size: int = Field(alias="TeamSize")
    technical_weight: int = Field(alias="TechnicalWeight")
    psychological_weight: int = Field(alias="PsychologicalWeight")
    interests_weight: int = Field(alias="InterestsWeight")
    sfia_weight: int = Field(alias="SfiaWeight")
    experience_weight: int = Field(alias="ExperienceWeight")
    language_weight: int = Field(alias="LanguageWeight")
    timezone_weight: int = Field(alias="TimezoneWeight")

    class Config:
        populate_by_name = True
        allow_population_by_field_name = True

class TeamMemberCompatibilityRequest(BaseModel):
    team: dict = Field(alias="Team")
    new_member: TeamMemberData = Field(alias="NewMember")

    class Config:
        populate_by_name = True
        allow_population_by_field_name = True

@app.post("/generate-teams")
async def generate_teams(request: TeamGenerationRequest):
    try:
        print("Received request:", request.dict()) 
        client = Anthropic(api_key=os.getenv('CLAUDE_API_KEY'))
        
        prompt = f"""
        # Análisis de Formación de Equipos

        ## Objetivo
        Analizar perfiles de candidatos y formar equipos optimizados basados en una evaluación de compatibilidad multidimensional.

        ## Requisitos
        - Roles Primarios Requeridos: {request.roles}
        - Stack Tecnológico: {request.technologies}
        - Nivel SFIA Mínimo: {request.sfia_level}
        - Solo Disponibles: {request.availability}
        - Tamaño del Equipo: {request.team_size}

        ## Criterios de Evaluación (Ponderados)
        - Compatibilidad de Nivel SFIA: {request.sfia_weight}%
        - Compatibilidad Técnica: {request.technical_weight}%
        - Compatibilidad Psicológica basada en MBTI: {request.psychological_weight}%
        - Nivel de Experiencia: {request.experience_weight}%
        - Dominio del Idioma: {request.language_weight}%
        - Compatibilidad basada en Intereses: {request.interests_weight}%
        - Consideración de Zona Horaria: {request.timezone_weight}%

        ## Principios de Formación de Equipos
        1. Asegurar cobertura de roles con niveles SFIA apropiados
        2. Equilibrar experiencia técnica en las tecnologías requeridas
        3. Crear diversidad psicológica mientras se minimizan patrones de conflicto severos
        4. Incorporar intereses complementarios para la cohesión del equipo
        5. Equilibrar niveles de experiencia (junior a senior) para transferencia de conocimiento
        6. Considerar superposición de zonas horarias para colaboración efectiva

        ### IMPORTANTE: Cálculo de Nivel SFIA
        - Cada candidato tiene un nivel SFIA calculado que representa su competencia promedio ÚNICAMENTE en las tecnologías requeridas
        - Al evaluar la compatibilidad técnica, enfócate principalmente en las tecnologías especificadas en los requisitos
        - Un candidato con alto nivel SFIA en tecnologías no requeridas no debe considerarse más técnicamente compatible que alguien con nivel SFIA moderado en las tecnologías requeridas

        ## Perfiles de Candidatos
        {json.dumps([member.dict() for member in request.members_data], indent=2, ensure_ascii=False)}

        ## Formato de Respuesta Requerido
        Responde EXCLUSIVAMENTE con un objeto JSON con la siguiente estructura exacta, sin texto adicional:

        ```json
        {{
          "teams": [
            {{
              "team_id": "guid-aleatorio-aquí",
              "members": [
                {{
                  "id": "id-del-miembro",
                  "name": "Nombre del Miembro",
                  "role": "Rol del Miembro",
                  "sfia_level": 5
                }}
              ]
            }}
          ],
          "recommended_leader": {{
            "id": "id-del-líder",
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
          "compatibility_score": 85
        }}
        ```

        IMPORTANTE: 
        1. Tu respuesta debe ser ÚNICAMENTE el objeto JSON válido, sin texto explicativo.
        2. Las fortalezas y debilidades deben ser explicaciones detalladas, no solo puntos breves.
        3. La justificación de compatibilidad debe ser un análisis profundo de la dinámica del equipo.
        4. Genera un team_id único en **formato GUID válido** (por ejemplo, "123e4567-e89b-12d3-a456-426614174000"). Asegúrate de que el GUID generado siga el estándar UUID v4: 36 caracteres con el patrón 8-4-4-4-12 y solo contenga números del 0 al 9 y letras de la A a F.
        5. La respuesta debe estar en español.
        """
        
        response = client.messages.create(
            model="claude-3-5-sonnet-20241022",
            max_tokens=2000,
            temperature=0.2,
            messages=[{"role": "user", "content": prompt}]
        )
        
        # Imprime la respuesta del IA para inspección
        print("Respuesta cruda del IA Service:\n", response.content[0].text)
        
        return json.loads(response.content[0].text)
    
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/calculate-compatibility")
async def calculate_compatibility(request: TeamMemberCompatibilityRequest):
    try:
        client = Anthropic(api_key=os.getenv('CLAUDE_API_KEY'))
        
        prompt = f"""
        # Análisis de Compatibilidad de Nuevo Miembro

        ## Objetivo
        Analizar cuán bien se integraría un potencial nuevo miembro con un equipo existente basado en compatibilidad multidimensional.

        ## Equipo Existente
        {json.dumps(request.team['members'], indent=2, ensure_ascii=False)}

        ## Potencial Nuevo Miembro
        {json.dumps(request.new_member.dict(), indent=2, ensure_ascii=False)}

        ## Dimensiones de Evaluación
        1. Complementariedad de Habilidades Técnicas (enfocarse solo en tecnologías requeridas)
        2. Compatibilidad Psicológica (MBTI)
        3. Alineación de Intereses
        4. Ajuste de Nivel de Experiencia
        5. Comunicaciones (Idioma y Zona Horaria)

        ### IMPORTANTE: Consideración de Nivel SFIA
        - El nivel SFIA proporcionado para el nuevo miembro está calculado específicamente basado en sus habilidades en las tecnologías requeridas por el equipo
        - Evalúa la compatibilidad técnica basada en cómo sus habilidades en las tecnologías requeridas complementan al equipo existente

        ## Formato de Respuesta Requerido
        Responde EXCLUSIVAMENTE con un objeto JSON con la siguiente estructura exacta, sin texto adicional:

        ```json
        {{
          "compatibility_score": 75,
          "analysis": {{
            "technical_compatibility": "Análisis detallado de la compatibilidad técnica, considerando cómo las habilidades del nuevo miembro complementan las del equipo existente en las tecnologías requeridas",
            "psychological_compatibility": "Análisis detallado de la compatibilidad psicológica basada en tipos MBTI y cómo esto afectaría la dinámica del equipo",
            "interest_alignment": "Análisis detallado de cómo los intereses del nuevo miembro se alinean con los del equipo y cómo esto podría afectar la cohesión",
            "experience_fit": "Análisis detallado de cómo el nivel de experiencia del nuevo miembro se ajusta al equipo y las oportunidades para transferencia de conocimiento",
            "communication_compatibility": "Análisis detallado de la compatibilidad en términos de idioma y zona horaria para la colaboración efectiva"
          }},
          "recommendations": [
            "Recomendación 1 detallada para maximizar la integración",
            "Recomendación 2 detallada para maximizar la integración",
            "Recomendación 3 detallada para maximizar la integración"
          ],
          "potential_conflicts": [
            "Conflicto potencial 1 detallado que requiere atención",
            "Conflicto potencial 2 detallado que requiere atención"
          ],
          "team_impact": "Análisis detallado de cómo este nuevo miembro cambiaría la dinámica del equipo, incluyendo aspectos positivos y posibles desafíos"
        }}
        ```

        IMPORTANTE: 
        1. Tu respuesta debe ser ÚNICAMENTE el objeto JSON válido, sin texto explicativo.
        2. Todos los análisis deben ser explicaciones detalladas, no solo puntos breves.
        3. La respuesta debe estar en español.
        """
        
        response = client.messages.create(
            model="claude-3-5-sonnet-20241022",
            max_tokens=1000,
            temperature=0.2,
            messages=[{"role": "user", "content": prompt}]
        )

        # Imprime la respuesta del IA para inspección
        print("Respuesta cruda del IA Service:\n", response.content[0].text)
        
        return json.loads(response.content[0].text)
    
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))