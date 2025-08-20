'use client';

import { useMemo } from 'react';

import { Team } from '@/hooks/useTeams';

import {
  CategoryScale,
  Chart as ChartJS,
  Filler,
  Legend,
  LineElement,
  LinearScale,
  PointElement,
  RadialLinearScale,
  Title,
  Tooltip,
} from 'chart.js';
import { Radar } from 'react-chartjs-2';

// Registrar los componentes necesarios de Chart.js
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  RadialLinearScale,
  Filler,
);

interface TeamRadarChartProps {
  team: Team;
  className?: string;
}

export function TeamRadarChart({ team, className = '' }: TeamRadarChartProps) {
  const radarData = useMemo(() => {
    // Calcular métricas basadas en los datos del equipo
    const metrics = calculateTeamMetrics(team);

    return {
      labels: [
        'Habilidades SFIA',
        'Cobertura Técnica',
        'Distribución Experiencia',
        'Diversidad',
        'Balance del Equipo',
        'Tamaño Optimal',
      ],
      datasets: [
        {
          label: team.name,
          data: [
            metrics.sfiaScore,
            metrics.technicalScore,
            metrics.experienceDistributionScore,
            metrics.diversityScore,
            metrics.balanceScore,
            metrics.teamSizeScore,
          ],
          backgroundColor: 'rgba(59, 130, 246, 0.2)',
          borderColor: 'rgba(59, 130, 246, 1)',
          borderWidth: 2,
          pointBackgroundColor: 'rgba(59, 130, 246, 1)',
          pointBorderColor: '#fff',
          pointHoverBackgroundColor: '#fff',
          pointHoverBorderColor: 'rgba(59, 130, 246, 1)',
          pointRadius: 4,
          pointHoverRadius: 6,
        },
      ],
    };
  }, [team]);

  // Detectar si es móvil para ajustar opciones
  const isMobile = typeof window !== 'undefined' && window.innerWidth < 768;

  const options = useMemo(
    () => ({
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        title: {
          display: false,
        },
        legend: {
          display: !isMobile,
          position: 'top' as const,
          labels: {
            color: '#374151',
            font: {
              size: 12,
            },
          },
        },
        tooltip: {
          backgroundColor: 'rgba(0, 0, 0, 0.9)',
          titleColor: '#fff',
          bodyColor: '#fff',
          borderColor: 'rgba(59, 130, 246, 1)',
          borderWidth: 1,
          padding: isMobile ? 8 : 12,
          cornerRadius: 8,
          displayColors: false,
          callbacks: {
            title: function (context: any) {
              const metricNames = [
                'Habilidades SFIA',
                'Cobertura Técnica',
                'Distribución Experiencia',
                'Diversidad',
                'Balance del Equipo',
                'Tamaño Optimal',
              ];
              return metricNames[context[0].dataIndex];
            },
            label: function (context: any) {
              const descriptions = [
                'Nivel promedio de habilidades',
                'Variedad de tecnologías',
                'Distribución de experiencia',
                'Diversidad de roles',
                'Equilibrio de criterios',
                'Tamaño adecuado',
              ];
              return isMobile
                ? `${context.parsed.r}/100`
                : `${descriptions[context.dataIndex]}: ${context.parsed.r}/100`;
            },
          },
        },
      },
      scales: {
        r: {
          angleLines: {
            color: 'rgba(156, 163, 175, 0.3)',
          },
          grid: {
            color: 'rgba(156, 163, 175, 0.3)',
          },
          pointLabels: {
            color: '#4B5563',
            font: {
              size: isMobile ? 9 : 11,
            },
          },
          ticks: {
            color: '#6B7280',
            font: {
              size: isMobile ? 8 : 10,
            },
            stepSize: 20,
            max: 100,
            min: 0,
            backdropColor: 'rgba(255, 255, 255, 0.8)',
          },
        },
      },
    }),
    [isMobile],
  );

  return (
    <div className={`relative ${className}`}>
      <Radar data={radarData} options={options} />
    </div>
  );
}

function calculateTeamMetrics(team: Team) {
  const members = team.members;
  const totalMembers = members.length;

  if (totalMembers === 0) {
    return {
      sfiaScore: 0,
      technicalScore: 0,
      experienceDistributionScore: 0,
      diversityScore: 0,
      balanceScore: 0,
      teamSizeScore: 0,
    };
  }

  // 1. SFIA Score - Basado en el nivel promedio de SFIA del equipo
  const avgSfiaLevel = members.reduce((sum, member) => sum + member.sfiaLevel, 0) / totalMembers;
  const sfiaScore = Math.min((avgSfiaLevel / 7) * 100, 100); // Normalizado a 100, asumiendo máximo nivel 7

  // 2. Technical Score - Mejorado: Basado en la presencia de tecnologías requeridas
  const requiredTechCount = team.requiredTechnologies.length;
  let technicalScore = 0;
  if (requiredTechCount === 0) {
    technicalScore = 50; // Si no hay tecnologías requeridas específicas, score neutro
  } else {
    // Score basado en la cantidad de tecnologías requeridas (más tecnologías = mayor cobertura)
    // Asumimos que 1-3 tech = básico, 4-6 = bueno, 7+ = excelente
    if (requiredTechCount <= 3) {
      technicalScore = 60 + requiredTechCount * 10; // 70-90
    } else if (requiredTechCount <= 6) {
      technicalScore = 80 + (requiredTechCount - 3) * 5; // 85-95
    } else {
      technicalScore = 95 + Math.min((requiredTechCount - 6) * 1, 5); // 96-100
    }
  }

  // 3. Experience Distribution Score - Evalúa qué tan bien distribuida está la experiencia
  // Clasifica miembros por nivel de experiencia basado en SFIA
  const juniors = members.filter((m) => m.sfiaLevel <= 2).length; // SFIA 1-2: Junior
  const mids = members.filter((m) => m.sfiaLevel >= 3 && m.sfiaLevel <= 4).length; // SFIA 3-4: Mid-level
  const seniors = members.filter((m) => m.sfiaLevel >= 5).length; // SFIA 5+: Senior

  let experienceDistributionScore = 0;
  if (totalMembers === 1) {
    experienceDistributionScore = 50; // Un solo miembro, score neutro
  } else if (totalMembers === 2) {
    // Para equipos de 2, ideal tener diferentes niveles
    experienceDistributionScore = juniors !== totalMembers && seniors !== totalMembers ? 80 : 40;
  } else {
    // Para equipos más grandes, evaluar distribución
    const hasJuniors = juniors > 0;
    const hasMids = mids > 0;
    const hasSeniors = seniors > 0;

    if (hasJuniors && hasMids && hasSeniors) {
      experienceDistributionScore = 100; // Distribución ideal
    } else if ((hasJuniors && hasSeniors) || (hasMids && hasSeniors) || (hasJuniors && hasMids)) {
      experienceDistributionScore = 75; // Buena distribución
    } else {
      // Todos del mismo nivel
      experienceDistributionScore = 30;
    }

    // Bonus por balance en la distribución
    const maxGroup = Math.max(juniors, mids, seniors);
    const balanceBonus = maxGroup / totalMembers <= 0.7 ? 15 : 0; // No más del 70% en un grupo
    experienceDistributionScore = Math.min(100, experienceDistributionScore + balanceBonus);
  }

  // 4. Diversity Score - Mejorado: Basado en variedad de roles y distribución de niveles SFIA
  const uniqueRoles = new Set(members.map((m) => m.role)).size;

  // Calcular diversidad de roles (más variedad = mejor)
  const roleDiversityScore = Math.min((uniqueRoles / totalMembers) * 100, 100);

  // Calcular diversidad de niveles SFIA (distribución equilibrada = mejor)
  const sfiaSpread = Math.max(...members.map((m) => m.sfiaLevel)) - Math.min(...members.map((m) => m.sfiaLevel));
  const sfiaSpreadScore = Math.min((sfiaSpread / 6) * 100, 100); // 6 es el spread máximo posible (1-7)

  const diversityScore = (roleDiversityScore + sfiaSpreadScore) / 2;

  // 5. Balance Score - Basado en la distribución de pesos del equipo
  const weights = team.weights;
  const weightValues = [
    weights.sfiaWeight,
    weights.technicalWeight,
    weights.psychologicalWeight,
    weights.experienceWeight,
    weights.languageWeight,
    weights.interestsWeight,
    weights.timezoneWeight,
  ];

  // Calcular qué tan balanceados están los pesos (menos desviación = mejor balance)
  const avgWeight = weightValues.reduce((sum, w) => sum + w, 0) / weightValues.length;
  const variance = weightValues.reduce((sum, w) => sum + Math.pow(w - avgWeight, 2), 0) / weightValues.length;
  const standardDeviation = Math.sqrt(variance);

  // Normalizar: menor desviación estándar = mejor balance
  const balanceScore = Math.max(0, 100 - standardDeviation * 2); // Ajustar factor según necesidad

  // 6. Team Size Score - Basado en tamaño optimal (3-8 miembros ideal)
  let teamSizeScore = 0;
  if (totalMembers >= 3 && totalMembers <= 8) {
    teamSizeScore = 100; // Tamaño ideal
  } else if (totalMembers >= 2 && totalMembers <= 10) {
    teamSizeScore = 70; // Tamaño aceptable
  } else if (totalMembers >= 1 && totalMembers <= 12) {
    teamSizeScore = 40; // Tamaño subóptimo
  } else {
    teamSizeScore = 20; // Tamaño problemático
  }

  return {
    sfiaScore: Math.round(sfiaScore),
    technicalScore: Math.round(technicalScore),
    experienceDistributionScore: Math.round(experienceDistributionScore),
    diversityScore: Math.round(diversityScore),
    balanceScore: Math.round(balanceScore),
    teamSizeScore: Math.round(teamSizeScore),
  };
}
