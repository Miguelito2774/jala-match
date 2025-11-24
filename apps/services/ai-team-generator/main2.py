from typing import List
from fastapi import FastAPI, HTTPException
from pydantic import UUID4, BaseModel, Field
import os
import json
import uuid
from dotenv import load_dotenv
from anthropic import Anthropic
from fastapi.middleware.cors import CORSMiddleware
from team_db_service import TeamDatabaseService

from init import ask_ia 

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "http://localhost:3000",  # Frontend Next.js
        "http://localhost:5001",  # Backend .NET
    ],
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

class BlendedTeamGenerationRequest(BaseModel):
    creator_id: str = Field(alias="CreatorId")
    team_size: int = Field(alias="TeamSize")
    technologies: List[str] = Field(alias="Technologies")
    project_complexity: str = Field(alias="ProjectComplexity")  # Low, Medium, High
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

        ```json
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
            "rationale": "🎯 EXPLICACIÓN SÚPER DETALLADA: Explica paso a paso por qué ESTA PERSONA DEL EQUIPO es el líder perfecto. IMPORTANTE: El líder DEBE ser uno de los miembros que ya incluiste en el equipo. NO inventes un líder nuevo. Selecciona el mejor líder de entre los miembros del equipo y explica por qué."
          }},
          "team_analysis": {{
            "strengths": [
              "💪 FORTALEZA TÉCNICA DETALLADA: Explica en 3-4 oraciones completas cómo las habilidades técnicas del equipo son perfectas para el proyecto. Menciona tecnologías específicas, niveles SFIA de cada miembro y qué significa eso en la práctica diaria. Explica cómo los pesos de criterios (ej: {request.weights.technical_weight}% técnico) influyeron en elegir estas personas.",
              "🧠 FORTALEZA DE COMPETENCIAS SFIA EXPLICADA: Describe en detalle qué nivel SFIA tiene cada miembro y qué significa eso en términos que cualquiera entienda. Por ejemplo: 'María tiene SFIA 3, lo que significa que puede trabajar sola pero pregunta cuando tiene dudas, perfecto para un rol Junior. Juan tiene SFIA 5, lo que significa que puede liderar técnicamente y enseñar a otros.' NO asumas que SFIA = senioridad laboral.",
              "🤝 FORTALEZA DE PERSONALIDADES COMPLEMENTARIAS: Explica en detalle cómo cada personalidad MBTI aporta algo único y cómo se complementan. Por ejemplo: 'Ana (ENTP) aporta innovación y encuentra soluciones creativas cuando hay problemas, Pedro (INFP) mantiene la armonía cuando hay tensiones, y Luis (ISTJ) asegura que todo se haga con calidad y procesos correctos. Juntos forman un equilibrio perfecto entre creatividad, armonía y estructura.'"
            ],
            "weaknesses": [
              "⚠️ DEBILIDAD ESPECÍFICA EXPLICADA: Identifica una limitación real del equipo y explica por qué podría ser problemática. Usa ejemplos concretos.",
              "🔧 RIESGO TECNOLÓGICO DETALLADO: Explica si hay dependencia excesiva en ciertas tecnologías, falta de diversidad técnica, o algún gap tecnológico importante. Da ejemplos específicos.",
              "📊 RIESGO OPERACIONAL EXPLICADO: Analiza factores como distribución de carga de trabajo, zonas horarias, posibles cuellos de botella, o conflictos de personalidad que podrían surgir."
            ],
            "compatibility": "🎯 ANÁLISIS INTEGRAL SÚPER DETALLADO (mínimo 200 palabras): Este debe ser un análisis completo que incluya: 1) Explicación específica de cómo CADA peso de criterio ({request.weights.sfia_weight}% SFIA, {request.weights.technical_weight}% técnico, {request.weights.psychological_weight}% psicológico, etc.) influyó en la selección del equipo - da ejemplos concretos, 2) Descripción clara de qué significa cada nivel SFIA presente en el equipo y sus implicaciones para el trabajo diario (ej: 'SFIA 3 significa que puede hacer tareas complejas solo pero necesita guidance ocasional'), 3) Análisis detallado de cada personalidad MBTI del equipo y cómo contribuye específicamente al éxito - explica las sinergias entre personalidades con ejemplos, 4) Evaluación de la alineación con los niveles solicitados ({[req.level for req in request.requirements]}) explicando por qué los niveles SFIA seleccionados son apropiados SIN asumir que equivalen a senioridad laboral"
          }},
          "compatibility_score": 87,
          "recommended_Members": [
            {{
              "id": "id-real-del-candidato-alternativo",
              "name": "Nombre del Candidato",
              "compatibility_score": 78,
              "analysis": "🔍 ANÁLISIS DETALLADO DEL CANDIDATO (mínimo 100 palabras): Explica paso a paso por qué este candidato sería genial para el equipo: 1) Su nivel SFIA específico y qué significa en términos simples para el trabajo diario, 2) Su personalidad MBTI y cómo complementaría específicamente a las personalidades ya en el equipo (da ejemplos de interacciones), 3) Sus fortalezas técnicas únicas y cómo llenarían gaps, 4) Por qué no fue seleccionado para el equipo principal pero sigue siendo valioso",
              "potential_conflicts": [
                "⚡ CONFLICTO POTENCIAL ESPECÍFICO: Describe exactamente qué tipo de fricción podría surgir y por qué. Por ejemplo: 'Su personalidad INTJ (planificador estructurado) podría chocar con el estilo más espontáneo del equipo actual, especialmente con Juan (ENFP) que prefiere improvisar.'",
                "⚡ RIESGO OPERACIONAL ESPECÍFICO: Identifica otro riesgo concreto con ejemplos. Por ejemplo: 'Está en zona horaria diferente (GMT-3 vs GMT-5 del resto del equipo), lo que podría dificultar las reuniones diarias.'"
              ],
              "team_impact": "📈 IMPACTO DETALLADO EN EL EQUIPO (mínimo 100 palabras): Explica específicamente cómo este miembro cambiaría la dinámica del equipo: 1) Cómo su personalidad MBTI específica afectaría las interacciones diarias del equipo con ejemplos concretos, 2) Qué nuevas capacidades técnicas aportaría y cómo eso beneficiaría al proyecto, 3) Cómo su nivel SFIA se integraría con la estructura existente del equipo, 4) Qué beneficios específicos y qué desafíos de gestión traería al equipo"
            }}
          ]
        }}
        ```

        ## 🎯 REGLAS FINALES SÚPER IMPORTANTES:

        1. **Tu respuesta debe ser ÚNICAMENTE el objeto JSON válido** - sin texto antes o después
        2. **Cada explicación debe ser LARGA y DETALLADA** - mínimo 2-3 oraciones por fortaleza/debilidad
        3. **Usa EJEMPLOS ESPECÍFICOS** - no digas "buen comunicador", di "puede explicar conceptos técnicos complejos de manera simple"
        4. **Explica TODOS los acrónimos y términos técnicos** - como si le hablaras a alguien que no sabe nada de tech
        5. **Usa EXACTAMENTE los IDs de empleados** que están en los datos proporcionados
        6. **Genera un GUID aleatorio válido** para el team_id (formato: 12345678-1234-1234-1234-123456789012)
        7. **TODO en español** y con un tono amigable y explicativo
        8. **JAMÁS inventes empleados** - solo usa los que están en los datos JSON
        9. **🚨 CRÍTICO: El recommended_leader DEBE ser uno de los miembros del equipo** - NO inventes un líder nuevo

        ## 💡 RECUERDA: 
        Tu objetivo es que cualquier manager, sin importar su nivel técnico, pueda leer tu respuesta y entender PERFECTAMENTE:
        - Por qué elegiste a cada persona
        - Qué significa cada nivel SFIA en términos prácticos  
        - Cómo las personalidades se van a complementar en el día a día
        - **🎯 SÚPER IMPORTANTE: Por qué seleccionaste a ESE MIEMBRO DEL EQUIPO como líder**
        - Qué fortalezas y debilidades reales tiene el equipo
        - Por qué respetaste los pesos de criterios que te dieron

        ¡Hazlo súper detallado y amigable! 🚀
        """

        response = client.messages.create(
            model="claude-3-5-sonnet-20241022",
            max_tokens=6000,
            temperature=0.2,
            messages=[{"role": "user", "content": prompt}],
        )

        try:
            # Log the raw response from Claude
            raw_response = response.content[0].text
            print(f"🤖 RAW CLAUDE RESPONSE: {raw_response}")
            
            team_formation_result = json.loads(raw_response)
            
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


@app.post("/generate-blended-team")
async def generate_blended_team(request: BlendedTeamGenerationRequest):
    """
    Endpoint para generar equipos en modo Blended.
    La IA determina automáticamente la mezcla óptima de roles, niveles y especialidades
    basándose en las tecnologías y la complejidad del proyecto.
    """
    db_service = TeamDatabaseService("postgresql://postgres:postgres@postgres:5432/postgres-db")
    await db_service.connect()
    
    try:
        # Obtener candidatos disponibles con el filtro de privacidad
        candidates = await db_service.get_generation_candidates(
            requirements=[],  # No especificamos roles, la IA los determinará
            technologies=request.technologies,
            min_sfia_level=request.sfia_level,
            availability=request.availability
        )
        
        if not candidates:
            raise HTTPException(status_code=404, detail="No se encontraron candidatos disponibles")
        
        # Generar UUID para el equipo
        generated_team_id = str(uuid.uuid4())
        
        # Preparar prompt para la IA con análisis de complejidad
        client = Anthropic(api_key=os.getenv("CLAUDE_API_KEY"))
        
        complexity_factors = {
            "Low": "Proyecto simple con tecnologías estándar y bajo riesgo técnico.",
            "Medium": "Proyecto balanceado que requiere experiencia moderada y coordinación.",
            "High": "Proyecto complejo que demanda alta especialización y arquitectura robusta."
        }
        
        prompt = f"""
        # ANÁLISIS Y GENERACIÓN DE EQUIPO BLENDED

        ## CONTEXTO DEL PROYECTO
        - **Tecnologías Requeridas**: {', '.join(request.technologies)}
        - **Complejidad del Proyecto**: {request.project_complexity} - {complexity_factors.get(request.project_complexity, 'Media')}
        - **Tamaño del Equipo**: {request.team_size} personas
        - **Nivel SFIA Mínimo**: {request.sfia_level}

        ## CRITERIOS DE PRIORIZACIÓN (Pesos)
        - SFIA/Experiencia: {request.weights.sfia_weight}%
        - Habilidades Técnicas: {request.weights.technical_weight}%
        - Perfil Psicológico: {request.weights.psychological_weight}%
        - Experiencia Previa: {request.weights.experience_weight}%
        - Idiomas: {request.weights.language_weight}%
        - Intereses: {request.weights.interests_weight}%
        - Zona Horaria: {request.weights.timezone_weight}%

        ## POOL DE CANDIDATOS DISPONIBLES
        {json.dumps(candidates[:50], indent=2, default=str)}  # Limitamos a 50 candidatos para evitar prompts muy largos

        ## TU MISIÓN COMO AI
        
        Debes crear un equipo blended óptimo considerando:
        
        1. **ANÁLISIS DE TECNOLOGÍAS**: 
           - Identifica qué tecnologías requieren especialistas senior vs generalistas
           - Ejemplo: React puede necesitar 1 senior frontend, mientras PostgreSQL puede ser manejado por un mid-level
        
        2. **DISTRIBUCIÓN DE SENIORITY**:
           - Para complejidad BAJA: Mayoría mid/junior con 1 senior como guía
           - Para complejidad MEDIA: Balance entre seniors (30-40%), mids (40-50%), juniors (10-20%)
           - Para complejidad ALTA: Mayoría seniors/architects (50-60%), algunos mids (30-40%), pocos juniors (10%)
        
        3. **MEZCLA DE ROLES**:
           - No todos deben ser super especializados
           - Incluye generalistas (fullstack) para flexibilidad
           - Asegura cobertura de todas las áreas técnicas necesarias
        
        4. **OPTIMIZACIÓN COSTO-BENEFICIO**:
           - Maximiza valor del equipo sin sobre-ingenierizar
           - Balancea experiencia con costo (seniors vs juniors)
        
        5. **COMPATIBILIDAD Y CULTURA**:
           - Considera MBTI para dinámicas de equipo
           - Timezone overlap para colaboración
           - Intereses compartidos para cohesión
        
        ## FORMATO DE RESPUESTA REQUERIDO (JSON estricto)
        
        IMPORTANTE: 
        - Usa SOLO los IDs exactos de empleados del pool de candidatos (campo "id")
        - Los member IDs deben ser los UUIDs tal como aparecen en el pool (sin modificar)
        - NO inventes IDs nuevos, usa los que están en el pool de candidatos
        - El team_id ya está generado: {generated_team_id}
        
        {{
          "teams": [
            {{
              "team_id": "{generated_team_id}",
              "members": [
                {{
                  "id": "copia_el_id_exacto_del_candidato_del_pool",
                  "name": "Nombre Completo del candidato",
                  "role": "Frontend Developer / Backend Developer / DevOps / etc",
                  "sfia_level": 4,
                  "assigned_responsibilities": "Qué hará específicamente en el proyecto"
                }}
              ]
            }}
          ],
          "recommended_members": [
            {{
              "id": "employee_id_del_pool",
              "name": "Nombre Completo",
              "compatibility_score": 85,
              "analysis": "Análisis de por qué este candidato sería una buena alternativa",
              "potential_conflicts": [
                "Posible conflicto 1 si se añade",
                "Posible conflicto 2 relacionado con el equipo"
              ],
              "team_impact": "Impacto positivo/negativo que tendría en la dinámica del equipo"
            }}
          ],
          "recommended_leader": {{
            "id": "employee_id",
            "name": "Nombre",
            "rationale": "Por qué es el mejor líder para este proyecto blended"
          }},
          "team_analysis": {{
            "strengths": [
              "Fortaleza 1 del equipo mixto",
              "Fortaleza 2 de la distribución de seniority",
              "Fortaleza 3 de cobertura técnica"
            ],
            "weaknesses": [
              "Posible gap de conocimiento en X",
              "Riesgo de coordinación en Y"
            ],
            "compatibility": "Análisis detallado de la dinámica del equipo blended (200 palabras)",
            "blending_strategy": "Explicación de por qué esta mezcla específica es óptima para el proyecto"
          }},
          "compatibility_score": 85,
          "complexity_analysis": {{
            "technology_breakdown": [
              {{
                "tech": "{request.technologies[0] if request.technologies else 'N/A'}",
                "complexity_level": "High/Medium/Low",
                "required_expertise": "Senior/Mid/Junior",
                "justification": "Por qué necesita este nivel"
              }}
            ],
            "recommended_distribution": {{
              "seniors": 2,
              "mids": 1,
              "juniors": 1
            }}
          }}
        }}
        
        IMPORTANTE: 
        - Selecciona EXACTAMENTE {request.team_size} miembros para el equipo principal
        - "recommended_members" debe incluir 3-5 candidatos alternativos del pool (NO incluidos en el equipo)
        - Solo usa IDs de empleados que aparecen en el pool de candidatos
        - La mezcla debe ser ESTRATÉGICA, no aleatoria
        - Justifica cada decisión de seniority y rol
        """
        
        response = client.messages.create(
            model="claude-3-5-sonnet-20241022",
            max_tokens=4000,
            temperature=0.3,  # Un poco más de creatividad para la mezcla
            messages=[{"role": "user", "content": prompt}]
        )
        
        result = json.loads(response.content[0].text)
        return result
        
    except json.JSONDecodeError as e:
        raise HTTPException(status_code=500, detail=f"Error parsing AI response: {str(e)}")
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error generating blended team: {str(e)}")
    finally:
        await db_service.disconnect()
