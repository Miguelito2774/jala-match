'use client';

import { useState } from 'react';

import { DashboardLayout } from '@/components/templates/DashboardLayout';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from '@/components/ui/collapsible';

import {
  BarChart3,
  Brain,
  CheckCircle,
  ChevronDown,
  ChevronUp,
  ExternalLink,
  HelpCircle,
  Shield,
  Star,
  Target,
  Users,
} from 'lucide-react';

interface FAQItem {
  id: string;
  question: string;
  answer: string | React.ReactNode;
  category: string;
  icon: React.ReactNode;
}

const faqData: FAQItem[] = [
  {
    id: 'sfia-what',
    question: '¿Qué es SFIA y por qué es importante?',
    category: 'SFIA',
    icon: <BarChart3 className="h-5 w-5 text-blue-600" />,
    answer: (
      <div className="space-y-4">
        <p>
          SFIA (Skills Framework for the Information Age) es un marco de referencia global que define las habilidades y
          competencias necesarias en el sector de las tecnologías de la información.
        </p>
        <p>
          En Jala Match, utilizamos SFIA para evaluar y categorizar las habilidades técnicas de nuestros empleados, lo
          que nos permite formar equipos más equilibrados y eficientes.
        </p>
        <div className="rounded-lg bg-blue-50 p-4">
          <h4 className="mb-2 font-semibold text-blue-900">Beneficios de SFIA:</h4>
          <ul className="list-inside list-disc space-y-1 text-blue-800">
            <li>Estandarización de habilidades técnicas</li>
            <li>Mejor formación de equipos</li>
            <li>Desarrollo profesional claro</li>
            <li>Evaluación objetiva de competencias</li>
          </ul>
        </div>
      </div>
    ),
  },
  {
    id: 'sfia-levels',
    question: '¿Cuáles son los niveles SFIA y cómo identificar el mío?',
    category: 'SFIA',
    icon: <Target className="h-5 w-5 text-green-600" />,
    answer: (
      <div className="space-y-4">
        <p>SFIA define 7 niveles de habilidad, del 1 al 7. Aquí te explicamos los más comunes:</p>

        <div className="grid gap-3">
          <div className="flex items-start gap-3 rounded-lg bg-gray-50 p-3">
            <Badge variant="outline">Nivel 1-2</Badge>
            <div>
              <h5 className="font-semibold">Principiante/Básico</h5>
              <p className="text-sm text-gray-600">
                Conocimientos básicos, requiere supervisión constante. Ideal para estudiantes o recién graduados.
              </p>
            </div>
          </div>

          <div className="flex items-start gap-3 rounded-lg bg-blue-50 p-3">
            <Badge variant="outline">Nivel 3-4</Badge>
            <div>
              <h5 className="font-semibold">Intermedio/Competente</h5>
              <p className="text-sm text-gray-600">
                1-3 años de experiencia. Puede trabajar de forma independiente en tareas específicas.
              </p>
            </div>
          </div>

          <div className="flex items-start gap-3 rounded-lg bg-green-50 p-3">
            <Badge variant="outline">Nivel 5-6</Badge>
            <div>
              <h5 className="font-semibold">Avanzado/Experto</h5>
              <p className="text-sm text-gray-600">
                3+ años de experiencia. Puede liderar proyectos y mentorear a otros desarrolladores.
              </p>
            </div>
          </div>

          <div className="flex items-start gap-3 rounded-lg bg-purple-50 p-3">
            <Badge variant="outline">Nivel 7</Badge>
            <div>
              <h5 className="font-semibold">Estratégico</h5>
              <p className="text-sm text-gray-600">
                Liderazgo organizacional, define estrategias tecnológicas a nivel empresarial.
              </p>
            </div>
          </div>
        </div>

        <div className="rounded-lg bg-yellow-50 p-4">
          <h4 className="mb-2 font-semibold text-yellow-900">💡 Tip para identificar tu nivel:</h4>
          <p className="text-sm text-yellow-800">
            Considera tu experiencia, autonomía, responsabilidades y capacidad de mentorear a otros. Si tienes dudas,
            consulta con tu manager o revisa proyectos similares en tu equipo.
          </p>
        </div>
      </div>
    ),
  },
  {
    id: 'mbti-what',
    question: '¿Qué es MBTI y por qué lo usamos?',
    category: 'MBTI',
    icon: <Brain className="h-5 w-5 text-purple-600" />,
    answer: (
      <div className="space-y-4">
        <p>
          MBTI (Myers-Briggs Type Indicator) es una herramienta de evaluación psicológica que identifica preferencias en
          la forma de percibir el mundo y tomar decisiones.
        </p>
        <p>
          En Jala Match, utilizamos MBTI para entender mejor las dinámicas de personalidad en los equipos y formar
          grupos de trabajo más compatibles y productivos.
        </p>

        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <div className="rounded-lg bg-purple-50 p-4">
            <h4 className="mb-2 font-semibold text-purple-900">Dimensiones MBTI:</h4>
            <ul className="space-y-1 text-sm text-purple-800">
              <li>
                <strong>E/I:</strong> Extraversión vs Introversión
              </li>
              <li>
                <strong>S/N:</strong> Sensación vs Intuición
              </li>
              <li>
                <strong>T/F:</strong> Pensamiento vs Sentimiento
              </li>
              <li>
                <strong>J/P:</strong> Juicio vs Percepción
              </li>
            </ul>
          </div>

          <div className="rounded-lg bg-blue-50 p-4">
            <h4 className="mb-2 font-semibold text-blue-900">Beneficios en equipos:</h4>
            <ul className="space-y-1 text-sm text-blue-800">
              <li>Mejor comunicación</li>
              <li>Resolución de conflictos</li>
              <li>Complementariedad de roles</li>
              <li>Mayor productividad</li>
            </ul>
          </div>
        </div>
      </div>
    ),
  },
  {
    id: 'mbti-test',
    question: '¿Cómo puedo conocer mi tipo MBTI?',
    category: 'MBTI',
    icon: <CheckCircle className="h-5 w-5 text-green-600" />,
    answer: (
      <div className="space-y-4">
        <p>
          Te recomendamos realizar el test oficial de 16personalities, que es gratuito, confiable y está disponible en
          español.
        </p>

        <div className="rounded-lg border border-green-200 bg-green-50 p-4">
          <div className="mb-3 flex items-center gap-2">
            <Star className="h-5 w-5 text-green-600" />
            <h4 className="font-semibold text-green-900">Test Recomendado</h4>
          </div>
          <p className="mb-3 text-green-800">
            16personalities.com ofrece un test preciso que toma aproximadamente 10-15 minutos.
          </p>
          <Button asChild className="bg-green-600 hover:bg-green-700">
            <a
              href="https://www.16personalities.com/es/test-de-personalidad"
              target="_blank"
              rel="noopener noreferrer"
              className="flex items-center gap-2"
            >
              Realizar Test MBTI
              <ExternalLink className="h-4 w-4" />
            </a>
          </Button>
        </div>

        <div className="rounded-lg bg-yellow-50 p-4">
          <h4 className="mb-2 font-semibold text-yellow-900">📝 Consejos para el test:</h4>
          <ul className="list-inside list-disc space-y-1 text-sm text-yellow-800">
            <li>Responde honestamente, no como &quot;deberías&quot; ser</li>
            <li>Piensa en tu comportamiento natural, no en situaciones específicas</li>
            <li>Tómate tu tiempo, no hay respuestas correctas o incorrectas</li>
            <li>Considera tu comportamiento en diferentes contextos</li>
          </ul>
        </div>
      </div>
    ),
  },
  {
    id: 'gdpr',
    question: '¿Qué es GDPR y cómo protegemos tus datos?',
    category: 'Privacidad',
    icon: <Shield className="h-5 w-5 text-red-600" />,
    answer: (
      <div className="space-y-4">
        <p>
          GDPR (Reglamento General de Protección de Datos) es la ley europea que regula el tratamiento de datos
          personales y garantiza la privacidad de los usuarios.
        </p>

        <div className="rounded-lg bg-red-50 p-4">
          <h4 className="mb-2 font-semibold text-red-900">En Jala Match cumplimos GDPR:</h4>
          <ul className="list-inside list-disc space-y-1 text-sm text-red-800">
            <li>Obtenemos consentimiento explícito para usar tus datos</li>
            <li>Solo recopilamos datos necesarios para el funcionamiento</li>
            <li>Tienes derecho a acceder, modificar o eliminar tus datos</li>
            <li>Implementamos medidas de seguridad técnicas y organizativas</li>
            <li>No compartimos datos con terceros sin consentimiento</li>
          </ul>
        </div>

        <div className="rounded-lg bg-blue-50 p-4">
          <h4 className="mb-2 font-semibold text-blue-900">Tus derechos:</h4>
          <div className="grid grid-cols-1 gap-2 text-sm text-blue-800 md:grid-cols-2">
            <div>• Acceso a tus datos</div>
            <div>• Rectificación de errores</div>
            <div>• Eliminación de datos</div>
            <div>• Portabilidad de datos</div>
            <div>• Limitación del tratamiento</div>
            <div>• Oposición al tratamiento</div>
          </div>
        </div>
      </div>
    ),
  },
  {
    id: 'team-formation',
    question: '¿Cómo funciona la formación de equipos con IA?',
    category: 'Plataforma',
    icon: <Users className="h-5 w-5 text-indigo-600" />,
    answer: (
      <div className="space-y-4">
        <p>Nuestro algoritmo de IA analiza múltiples factores para formar equipos equilibrados y compatibles.</p>

        <div className="grid gap-3">
          <div className="rounded-lg bg-indigo-50 p-3">
            <h5 className="font-semibold text-indigo-900">Factores técnicos:</h5>
            <p className="text-sm text-indigo-800">
              Habilidades SFIA, experiencia en tecnologías, roles especializados
            </p>
          </div>

          <div className="rounded-lg bg-purple-50 p-3">
            <h5 className="font-semibold text-purple-900">Factores psicológicos:</h5>
            <p className="text-sm text-purple-800">Tipos MBTI, estilos de comunicación, preferencias de trabajo</p>
          </div>

          <div className="rounded-lg bg-green-50 p-3">
            <h5 className="font-semibold text-green-900">Factores prácticos:</h5>
            <p className="text-sm text-green-800">Zona horaria, disponibilidad, intereses del proyecto</p>
          </div>
        </div>

        <div className="rounded-lg bg-gray-50 p-4">
          <h4 className="mb-2 font-semibold text-gray-900">Proceso de formación:</h4>
          <ol className="list-inside list-decimal space-y-1 text-sm text-gray-700">
            <li>El manager define requisitos del proyecto</li>
            <li>La IA analiza perfiles disponibles</li>
            <li>Se generan múltiples opciones de equipos</li>
            <li>Se presenta la mejor opción con análisis detallado</li>
            <li>El manager puede ajustar y confirmar el equipo</li>
          </ol>
        </div>
      </div>
    ),
  },
  {
    id: 'profile-verification',
    question: '¿Qué es la verificación de perfil y por qué es necesaria?',
    category: 'Plataforma',
    icon: <CheckCircle className="h-5 w-5 text-green-600" />,
    answer: (
      <div className="space-y-4">
        <p>
          La verificación de perfil es un proceso donde un manager o líder técnico valida la información de habilidades
          y experiencia que has proporcionado.
        </p>

        <div className="rounded-lg bg-green-50 p-4">
          <h4 className="mb-2 font-semibold text-green-900">¿Por qué es importante?</h4>
          <ul className="list-inside list-disc space-y-1 text-sm text-green-800">
            <li>Garantiza la precisión de los datos en la plataforma</li>
            <li>Mejora la calidad de los equipos formados por IA</li>
            <li>Aumenta la confianza entre miembros del equipo</li>
            <li>Facilita la asignación correcta de roles y responsabilidades</li>
          </ul>
        </div>

        <div className="rounded-lg bg-blue-50 p-4">
          <h4 className="mb-2 font-semibold text-blue-900">Proceso de verificación:</h4>
          <ol className="list-inside list-decimal space-y-1 text-sm text-blue-800">
            <li>Completas tu perfil técnico y personal</li>
            <li>Solicitas verificación desde tu dashboard</li>
            <li>Un manager revisa tu información</li>
            <li>Recibes feedback y estado de verificación</li>
            <li>Tu perfil queda disponible para formación de equipos</li>
          </ol>
        </div>
      </div>
    ),
  },
  {
    id: 'data-usage',
    question: '¿Cómo se usan mis datos personales?',
    category: 'Privacidad',
    icon: <Shield className="h-5 w-5 text-orange-600" />,
    answer: (
      <div className="space-y-4">
        <p>
          Tus datos se utilizan exclusivamente para mejorar la formación de equipos y tu experiencia en la plataforma.
        </p>

        <div className="grid gap-3">
          <div className="rounded-lg bg-orange-50 p-3">
            <h5 className="font-semibold text-orange-900">Datos técnicos:</h5>
            <p className="text-sm text-orange-800">
              Utilizados para matching de habilidades y asignación de roles apropiados
            </p>
          </div>

          <div className="rounded-lg bg-blue-50 p-3">
            <h5 className="font-semibold text-blue-900">Datos de personalidad:</h5>
            <p className="text-sm text-blue-800">Ayudan a formar equipos con mejor compatibilidad y comunicación</p>
          </div>

          <div className="rounded-lg bg-green-50 p-3">
            <h5 className="font-semibold text-green-900">Datos de contacto:</h5>
            <p className="text-sm text-green-800">
              Solo para comunicación dentro de la plataforma y notificaciones importantes
            </p>
          </div>
        </div>

        <div className="rounded-lg border border-red-200 bg-red-50 p-4">
          <h4 className="mb-2 font-semibold text-red-900">🚫 Lo que NO hacemos:</h4>
          <ul className="list-inside list-disc space-y-1 text-sm text-red-800">
            <li>Vender tus datos a terceros</li>
            <li>Usar datos fuera del contexto laboral</li>
            <li>Compartir información sin consentimiento</li>
            <li>Almacenar datos innecesarios</li>
          </ul>
        </div>
      </div>
    ),
  },
];

const categories = ['Todos', 'SFIA', 'MBTI', 'Privacidad', 'Plataforma'];

export default function FAQPage() {
  const [selectedCategory, setSelectedCategory] = useState('Todos');
  const [openItems, setOpenItems] = useState<string[]>([]);
  const [searchQuery, setSearchQuery] = useState('');

  const filteredFAQs = faqData.filter((item) => {
    const matchesCategory = selectedCategory === 'Todos' || item.category === selectedCategory;
    const matchesSearch =
      item.question.toLowerCase().includes(searchQuery.toLowerCase()) ||
      (typeof item.answer === 'string' && item.answer.toLowerCase().includes(searchQuery.toLowerCase()));
    return matchesCategory && matchesSearch;
  });

  const toggleItem = (id: string) => {
    setOpenItems((prev) => (prev.includes(id) ? prev.filter((item) => item !== id) : [...prev, id]));
  };

  return (
    <DashboardLayout>
      <div className="space-y-8">
        {/* Header */}
        <div className="space-y-4 text-center">
          <div className="flex justify-center">
            <div className="rounded-full bg-blue-100 p-3">
              <HelpCircle className="h-8 w-8 text-blue-600" />
            </div>
          </div>
          <h1 className="text-4xl font-bold text-gray-900">Preguntas Frecuentes</h1>
          <p className="mx-auto max-w-2xl text-lg text-gray-600">
            Encuentra respuestas a las preguntas más comunes sobre Jala Match, desde conceptos técnicos hasta políticas
            de privacidad.
          </p>
        </div>

        {/* Search Bar */}
        <div className="mx-auto max-w-md">
          <div className="relative">
            <HelpCircle className="absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 transform text-gray-400" />
            <input
              type="text"
              placeholder="Buscar preguntas..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="w-full rounded-lg border border-gray-300 py-2 pr-4 pl-10 focus:border-transparent focus:ring-2 focus:ring-blue-500"
            />
          </div>
        </div>

        {/* Category Filter */}
        <div className="flex flex-wrap justify-center gap-2">
          {categories.map((category) => (
            <Button
              key={category}
              variant={selectedCategory === category ? 'default' : 'outline'}
              size="sm"
              onClick={() => setSelectedCategory(category)}
              className="flex-shrink-0"
            >
              {category}
            </Button>
          ))}
        </div>

        {/* FAQ Items */}
        <div className="mx-auto max-w-4xl space-y-4">
          {filteredFAQs.length === 0 ? (
            <Card>
              <CardContent className="py-12 text-center">
                <HelpCircle className="mx-auto mb-4 h-12 w-12 text-gray-400" />
                <h3 className="mb-2 text-lg font-semibold text-gray-900">No se encontraron preguntas</h3>
                <p className="text-gray-600">
                  Intenta con otros términos de búsqueda o selecciona una categoría diferente.
                </p>
              </CardContent>
            </Card>
          ) : (
            filteredFAQs.map((item) => (
              <Card key={item.id} className="overflow-hidden">
                <Collapsible open={openItems.includes(item.id)} onOpenChange={() => toggleItem(item.id)}>
                  <CollapsibleTrigger className="w-full">
                    <CardHeader className="transition-colors hover:bg-gray-50">
                      <div className="flex items-center justify-between text-left">
                        <div className="flex items-center gap-3">
                          {item.icon}
                          <div>
                            <CardTitle className="text-lg">{item.question}</CardTitle>
                            <CardDescription>{item.category}</CardDescription>
                          </div>
                        </div>
                        {openItems.includes(item.id) ? (
                          <ChevronUp className="h-5 w-5 text-gray-400" />
                        ) : (
                          <ChevronDown className="h-5 w-5 text-gray-400" />
                        )}
                      </div>
                    </CardHeader>
                  </CollapsibleTrigger>
                  <CollapsibleContent>
                    <CardContent className="pt-0">
                      <div className="border-t pt-4">{item.answer}</div>
                    </CardContent>
                  </CollapsibleContent>
                </Collapsible>
              </Card>
            ))
          )}
        </div>

        {/* Contact Section */}
        <div className="mx-auto max-w-2xl">
          <Card className="border-blue-200 bg-gradient-to-r from-blue-50 to-indigo-50">
            <CardContent className="py-8 text-center">
              <HelpCircle className="mx-auto mb-4 h-8 w-8 text-blue-600" />
              <h3 className="mb-2 text-lg font-semibold text-gray-900">¿No encontraste lo que buscabas?</h3>
              <p className="mb-4 text-gray-600">
                Si tienes otras preguntas o necesitas ayuda adicional, no dudes en contactar a tu manager o al equipo de
                soporte.
              </p>
              <div className="flex flex-wrap justify-center gap-3">
                <Badge variant="secondary" className="bg-blue-100 text-blue-800">
                  💡 Tip: Revisa tu perfil regularmente
                </Badge>
                <Badge variant="secondary" className="bg-green-100 text-green-800">
                  ✅ Mantén tus datos actualizados
                </Badge>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </DashboardLayout>
  );
}
