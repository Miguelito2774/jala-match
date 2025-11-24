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
        # 🚀 GENERADOR INTELIGENTE DE EQUIPOS DE TRABAJO - Análisis Completo y Amigable

        Eres un experto consultor en recursos humanos y formación de equipos. Tu trabajo es crear el mejor equipo posible y explicar TODO de manera que cualquier persona pueda entender fácilmente tus decisiones.

        ## 📊 Datos de Empleados Disponibles
        ```json
        {employees_json}
        ```

        ## 🎯 Lo Que Me Han Pedido Crear
        - 👥 Necesito formar un equipo de: {request.team_size} personas
        - 💼 Para estos roles específicos: {[role.role for role in request.requirements]} 
        - 📋 Con estos niveles de experiencia: {[req.level for req in request.requirements]}
        - 🏢 En estas áreas de trabajo: {[req.area for req in request.requirements]}
        - 💻 Que dominen estas tecnologías: {request.technologies}
        - 📈 Con un nivel SFIA mínimo de: {request.sfia_level}

        ## ⚖️ PRIORIDADES DEL MANAGER - ¡Estos Son Los Criterios Más Importantes!
        El manager me ha dado estos pesos para priorizar qué es más importante:

        - 🎯 Nivel SFIA (Competencia Técnica): {request.weights.sfia_weight}% de importancia
        - 💻 Experiencia en Tecnologías: {request.weights.technical_weight}% de importancia  
        - 🧠 Compatibilidad de Personalidades: {request.weights.psychological_weight}% de importancia
        - 📅 Años de Experiencia: {request.weights.experience_weight}% de importancia
        - 🗣️ Habilidades de Comunicación: {request.weights.language_weight}% de importancia
        - 🎨 Intereses y Hobbies Compartidos: {request.weights.interests_weight}% de importancia
        - 🌍 Zona Horaria y Ubicación: {request.weights.timezone_weight}% de importancia

        ## 📚 GUÍA PARA ENTENDER TODO - Explicado de Manera Simple

        ### 🎓 ¿Qué Significan los Niveles SFIA? (Marco de Competencias Técnicas)
        Piensa en SFIA como los "niveles de videojuego" de las habilidades técnicas:

        **SFIA Nivel 1-2**: 🌱 "El Aprendiz"
        - Como un conductor novato que necesita instructor al lado
        - Requiere supervisión constante y mucha ayuda
        - Perfecto para roles de prácticas o trainee

        **SFIA Nivel 3**: 🚗 "El Conductor Independiente" 
        - Como alguien que ya maneja solo pero a veces pregunta direcciones
        - Puede trabajar de forma independiente con orientación ocasional
        - ¡PERFECTO para roles "Junior"! (No confundir: puede tener años de experiencia)

        **SFIA Nivel 4**: 🏎️ "El Conductor Experimentado"
        - Como un chofer profesional que puede enseñar a otros
        - Puede mentorear y guiar a niveles más bajos
        - Ideal para roles "Semi-Senior" o "Mid-Level"

        **SFIA Nivel 5**: 🏁 "El Instructor de Manejo"
        - Como el jefe de una escuela de manejo
        - Liderazgo técnico, toma decisiones complejas
        - Perfecto para roles "Senior" y "Tech Lead"

        **SFIA Nivel 6-7**: 🛣️ "El Planificador de Carreteras"
        - Como quien diseña las autopistas de todo el país
        - Arquitecto de sistemas, visión estratégica
        - Para roles de "Arquitecto" o "Principal"

        **🔥 SÚPER IMPORTANTE**: El nivel SFIA NO es lo mismo que "junior/senior" en el trabajo. 
        Alguien puede tener SFIA 3 y 10 años de experiencia, sigue siendo perfecto para un rol "Junior" específico.

        ### 🧩 ¿Qué Significan las Personalidades MBTI? (Los "Superpoderes" de Cada Persona)

        **ENFP - "El Motivador Estrella" 🌟**
        - Como el mejor animador de fiestas, pero para el trabajo
        - Superpoder: Genera entusiasmo contagioso y levanta el ánimo del equipo
        - Perfecto para: Brainstorming, motivar cuando hay problemas, generar ideas creativas

        **ENTP - "El Innovador Rebelde" 💡**
        - Como un inventor loco que siempre encuentra soluciones únicas
        - Superpoder: Ve problemas desde ángulos que nadie más ve
        - Perfecto para: Resolver problemas complejos, desafiar ideas, encontrar mejores maneras de hacer las cosas

        **INFP - "El Pacificador Empático" 🕊️**
        - Como un diplomático que resuelve conflictos sin que nadie se enoje
        - Superpoder: Mantiene la armonía y se preocupa genuinamente por todos
        - Perfecto para: Mediar conflictos, mantener moral alta, asegurar que todos se sientan valorados

        **INTJ - "El Arquitecto Maestro" 🏗️**
        - Como un gran maestro de ajedrez que planifica 10 movimientos adelante
        - Superpoder: Visión a largo plazo y planes estratégicos perfectos
        - Perfecto para: Arquitectura de sistemas, planificación a largo plazo, decisiones estratégicas

        **ENFJ - "El Líder Natural" 👑**
        - Como un entrenador que saca lo mejor de cada jugador
        - Superpoder: Desarrolla el potencial de otros y coordina perfectamente
        - Perfecto para: Liderar equipos, mentorear, coordinar proyectos complejos

        **ISTJ - "El Guardián de la Calidad" 🛡️**
        - Como un inspector de calidad que nunca deja pasar un error
        - Superpoder: Procesos perfectos, confiabilidad absoluta, atención al detalle
        - Perfecto para: Asegurar calidad, crear procesos, mantener estabilidad

        ## 🎯 INSTRUCCIONES SÚPER ESPECÍFICAS PARA CREAR EL MEJOR EQUIPO

        ### 📋 Lo Que DEBES Hacer:
        1. **Respeta LOS PESOS** como si fuera ley: Si el manager puso 25% en tecnología, ¡eso es SÚPER importante!
        2. **Explica TODO como si fueras un profesor**: Cada decisión debe tener una explicación que mi abuela entendería
        3. **Personalidades que se complementen**: Como piezas de rompecabezas que encajan perfectamente
        4. **Niveles SFIA apropiados**: No asumas que SFIA = senioridad laboral
        5. **Detalla las FORTALEZAS**: Explica por qué este equipo va a ser increíble
        6. **Identifica DEBILIDADES**: Sé honesto sobre qué podría ser problemático
        7. **Recomienda alternativas**: Otros candidatos que podrían ser útiles

        ### 🚨 Lo Que NO Debes Hacer:
        - NO inventes empleados que no están en los datos
        - NO asumas que SFIA 5 = "Senior" automáticamente
        - NO hagas explicaciones cortas y aburridas
        - NO ignores los pesos que me dieron
        - NO uses jerga técnica sin explicar
        - No duplicar miembros recomendados: Los candidatos en "recommended_Members" NO PUEDEN ser los mismos que están en el equipo principal

        ## 🎯 REGLA CRÍTICA PARA EL LÍDER:
        El "recommended_leader" DEBE ser uno de los miembros que incluiste en el equipo.
        NO inventes un líder nuevo. NO uses IDs que no estén en la lista de miembros del equipo.
        Selecciona al MEJOR líder de entre los miembros del equipo que ya formaste.

        ## �📝 FORMATO DE RESPUESTA - ¡Hazlo Súper Detallado y Amigable!

        Responde EXCLUSIVAMENTE con un objeto JSON con esta estructura, pero llena cada campo con explicaciones LARGAS y DETALLADAS:
        IMPORTANTE: Responde ÚNICAMENTE con JSON puro. NO uses bloques de código markdown (```json o ```). 
        Tu respuesta debe empezar directamente con {{ y terminar con }}.

        {{
          "teams": [
            {{
              "team_id": "guid aleatorio aqui",
              "members": [
                {{
                  "id": "id-real-del-empleado-de-los-datos",
                  "name": "Nombre Completo",
                  "role": "Su Rol",
                  "sfia_level": 4
                }}
              ]
            }}
          ],
          "recommended_leader": {{
            "id": "DEBE SER EL ID DE UNO DE LOS MIEMBROS DEL EQUIPO ARRIBA",
            "name": "DEBE SER EL NOMBRE DE UNO DE LOS MIEMBROS DEL EQUIPO ARRIBA",
            "rationale": "🎯 Explica en 2-3 párrafos por qué ESTA PERSONA DEL EQUIPO es el líder perfecto. IMPORTANTE: El líder DEBE ser uno de los miembros que ya incluiste en el equipo."
          }},
          "team_analysis": {{
            "strengths": [
              "💪 FORTALEZA TÉCNICA: Explica cómo las habilidades técnicas del equipo son perfectas para el proyecto. Menciona tecnologías específicas y niveles SFIA.",
              "🧠 FORTALEZA DE COMPETENCIAS SFIA: Describe qué nivel SFIA tiene cada miembro y qué significa eso en la práctica. NO asumas que SFIA = senioridad laboral.",
              "🤝 FORTALEZA DE PERSONALIDADES: Explica cómo cada personalidad MBTI aporta algo único y cómo se complementan."
            ],
            "weaknesses": [
              "⚠️ DEBILIDAD ESPECÍFICA: Identifica una limitación real del equipo con ejemplos concretos.",
              "🔧 RIESGO TECNOLÓGICO: Explica gaps tecnológicos o dependencias problemáticas.",
              "📊 RIESGO OPERACIONAL: Analiza factores como zonas horarias, cuellos de botella, o conflictos potenciales."
            ],
            "compatibility": "🎯 ANÁLISIS INTEGRAL (100-150 palabras): Explica cómo cada peso de criterio ({request.weights.sfia_weight}% SFIA, {request.weights.technical_weight}% técnico, etc.) influyó en la selección, qué significa cada nivel SFIA, y cómo las personalidades MBTI se complementan."
          }},
          "compatibility_score": 87,
          "recommended_Members": [
            {{ 
              "id": "id-candidato-1", 
              "name": "Nombre 1", 
              "compatibility_score": 85, 
              "analysis": "2-3 oraciones: nivel SFIA, stack técnico clave, personalidad MBTI y por qué no fue seleccionado.", 
              "potential_conflicts": ["Conflicto 1 en 1 oración", "Conflicto 2 en 1 oración"], 
              "team_impact": "2-3 oraciones sobre impacto técnico y de personalidad en el equipo." 
            }},
            {{ "id": "id-candidato-2", "name": "Nombre 2", "compatibility_score": 82, "analysis": "2-3 oraciones", "potential_conflicts": ["Conflicto 1", "Conflicto 2"], "team_impact": "2-3 oraciones" }},
            {{ "id": "id-candidato-3", "name": "Nombre 3", "compatibility_score": 78, "analysis": "2-3 oraciones", "potential_conflicts": ["Conflicto 1", "Conflicto 2"], "team_impact": "2-3 oraciones" }}
          ]
        }}

        IMPORTANTE: 
        - Incluye exactamente 3 candidatos en recommended_Members, no más.
        - Cada análisis debe ser BREVE: máximo 2-3 oraciones concisas
        - Cada conflicto: máximo 1 oración
        - Cada impacto: máximo 2-3 oraciones

        ## 🎯 REGLAS FINALES:

        1. **Tu respuesta debe ser ÚNICAMENTE el objeto JSON válido** - sin bloques markdown ```json
        2. **Sé BREVE y conciso** - máximo 2-3 oraciones por campo en recommended_Members
        3. **Usa EJEMPLOS ESPECÍFICOS** - no digas "buen comunicador", di "puede explicar conceptos técnicos complejos"
        4. **Usa EXACTAMENTE los IDs de empleados** que están en los datos proporcionados
        5. **Genera un GUID aleatorio válido** para el team_id
        6. **TODO en español** con tono amigable
        7. **🚨 CRÍTICO: El recommended_leader DEBE ser uno de los miembros del equipo**
        8. **SOLO 3 candidatos en recommended_Members**

        Responde en JSON puro, SIN bloques markdown. Empieza tu respuesta directamente con {{

        """

        response = client.messages.create(
            model="claude-sonnet-4-5-20250929",
            max_tokens=6000,  # Reduced further with briefer recommended_Members
            temperature=0.3,  # Slightly higher for faster generation
            messages=[{"role": "user", "content": prompt}],
        )

        # Handle Claude 4.5 specific stop reasons
        if response.stop_reason == "refusal":
            print(f"⚠️ CLAUDE REFUSED REQUEST: {response.content[0].text if response.content else 'No content'}")
            raise HTTPException(
                status_code=400, 
                detail="La IA rechazó procesar esta solicitud. Por favor revise los criterios e intente nuevamente."
            )
        
        if response.stop_reason == "model_context_window_exceeded":
            print(f"⚠️ CONTEXT WINDOW EXCEEDED")
            raise HTTPException(
                status_code=400,
                detail="La solicitud excede el contexto máximo. Intente con menos candidatos o criterios más simples."
            )
        
        # Check if response was cut off due to max_tokens
        if response.stop_reason == "max_tokens":
            print(f"⚠️ RESPONSE TRUNCATED - Claude reached max_tokens limit")
            raise HTTPException(
                status_code=500,
                detail="La respuesta de la IA fue truncada. Intente con un equipo más pequeño o contacte soporte."
            )

        try:
            # Log the raw response from Claude
            raw_response = response.content[0].text
            print(f"🤖 RAW CLAUDE RESPONSE: {raw_response[:500]}...")  # Only print first 500 chars
            
            # Clean markdown code blocks from Claude's response
            cleaned_response = raw_response.strip()
            if cleaned_response.startswith("```json"):
                cleaned_response = cleaned_response[7:]
            elif cleaned_response.startswith("```"):
                cleaned_response = cleaned_response[3:]
            if cleaned_response.endswith("```"):
                cleaned_response = cleaned_response[:-3]
            cleaned_response = cleaned_response.strip()
            
            team_formation_result = json.loads(cleaned_response)
            
            # Log the parsed JSON to see structure
            print(f"📋 PARSED JSON KEYS: {list(team_formation_result.keys())}")
            if 'recommended_Leader' in team_formation_result:
                print(f"✅ recommended_Leader FOUND: {team_formation_result['recommended_Leader']}")
            else:
                print(f"❌ recommended_Leader NOT FOUND in keys: {list(team_formation_result.keys())}")
            
            return team_formation_result
        except Exception as e:
            print(f"💥 JSON PARSE ERROR: {str(e)}")
            print(f"🔍 RAW RESPONSE CAUSING ERROR: {response.content[0].text}")
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
        Responde EXCLUSIVAMENTE con un array JSON con la siguiente estructura exacta, sin texto adicional, tampoco bloques de código markdown o (```json```):

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
            model="claude-sonnet-4-5-20250929",
            max_tokens=4000,
            temperature=0.2,  # Using only temperature (not top_p) as per Claude 4.5 requirements
            messages=[{"role": "user", "content": prompt}],
        )

        # Handle Claude 4.5 specific stop reasons
        if response.stop_reason == "refusal":
            print(f"⚠️ CLAUDE REFUSED REQUEST: {response.content[0].text if response.content else 'No content'}")
            raise HTTPException(
                status_code=400,
                detail="La IA rechazó procesar esta solicitud de búsqueda de candidatos."
            )
        
        if response.stop_reason == "model_context_window_exceeded":
            raise HTTPException(
                status_code=400,
                detail="Demasiados candidatos para analizar. Intente con filtros más restrictivos."
            )

        try:
            # Clean markdown code blocks from Claude's response
            raw_response = response.content[0].text
            cleaned_response = raw_response.strip()
            if cleaned_response.startswith("```json"):
                cleaned_response = cleaned_response[7:]
            elif cleaned_response.startswith("```"):
                cleaned_response = cleaned_response[3:]
            if cleaned_response.endswith("```"):
                cleaned_response = cleaned_response[:-3]
            cleaned_response = cleaned_response.strip()
            
            recommendations = json.loads(cleaned_response)
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
            model="claude-sonnet-4-5-20250929",  # Updated to Claude 4.5
            max_tokens=3000,
            temperature=0.1,  # Using only temperature (not top_p) as per Claude 4.5 requirements
            messages=[{"role": "user", "content": prompt}]
        )
        
        # Handle Claude 4.5 specific stop reasons
        if response.stop_reason == "refusal":
            print(f"⚠️ CLAUDE REFUSED REANALYSIS REQUEST")
            raise HTTPException(
                status_code=400,
                detail="La IA rechazó re-analizar este equipo."
            )
        
        if response.stop_reason == "model_context_window_exceeded":
            raise HTTPException(
                status_code=400,
                detail="El equipo es demasiado grande para re-analizar."
            )
        
        # Clean markdown code blocks from Claude's response
        raw_response = response.content[0].text
        cleaned_response = raw_response.strip()
        if cleaned_response.startswith("```json"):
            cleaned_response = cleaned_response[7:]
        elif cleaned_response.startswith("```"):
            cleaned_response = cleaned_response[3:]
        if cleaned_response.endswith("```"):
            cleaned_response = cleaned_response[:-3]
        cleaned_response = cleaned_response.strip()
        
        return json.loads(cleaned_response)
        
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
