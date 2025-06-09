import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';

import { BarChart3, Globe, Languages, Users } from 'lucide-react';

interface TeamStatsProps {
  stats: {
    totalMembers: number;
    averageSfiaLevel: number;
    uniqueTechnologies: number;
    uniqueLanguages: number;
    countriesRepresented: number;
    mbtiDistribution: Record<string, number>;
  };
  compatibilityScore: number;
}

export const TeamStatsCard: React.FC<TeamStatsProps> = ({ stats, compatibilityScore }) => {
  const getScoreColor = (score: number) => {
    if (score >= 80) return 'text-green-600';
    if (score >= 60) return 'text-yellow-600';
    return 'text-red-600';
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <BarChart3 className="h-5 w-5" />
          Estadísticas del Equipo
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        {/* Compatibility Score */}
        <div className="space-y-2">
          <div className="flex items-center justify-between">
            <span className="text-sm font-medium">Puntuación de Compatibilidad</span>
            <span className={`text-lg font-bold ${getScoreColor(compatibilityScore)}`}>{compatibilityScore}%</span>
          </div>
          <Progress value={compatibilityScore} className="h-2" />
        </div>

        {/* Team Overview Stats */}
        <div className="grid grid-cols-2 gap-4">
          <div className="flex items-center gap-3 rounded-lg bg-blue-50 p-3">
            <Users className="h-5 w-5 text-blue-600" />
            <div>
              <p className="text-sm font-medium text-blue-600">Miembros</p>
              <p className="text-lg font-bold text-blue-800">{stats.totalMembers}</p>
            </div>
          </div>

          <div className="flex items-center gap-3 rounded-lg bg-green-50 p-3">
            <Languages className="h-5 w-5 text-green-600" />
            <div>
              <p className="text-sm font-medium text-green-600">Tecnologías</p>
              <p className="text-lg font-bold text-green-800">{stats.uniqueTechnologies}</p>
            </div>
          </div>

          <div className="flex items-center gap-3 rounded-lg bg-orange-50 p-3">
            <Globe className="h-5 w-5 text-orange-600" />
            <div>
              <p className="text-sm font-medium text-orange-600">Países</p>
              <p className="text-lg font-bold text-orange-800">{stats.countriesRepresented}</p>
            </div>
          </div>
        </div>

        {/* MBTI Distribution */}
        {Object.keys(stats.mbtiDistribution).length > 0 && (
          <div className="space-y-3">
            <h4 className="text-sm font-medium">Distribución MBTI</h4>
            <div className="flex flex-wrap gap-2">
              {Object.entries(stats.mbtiDistribution).map(([mbti, count]) => (
                <Badge key={mbti} variant="outline" className="text-xs">
                  {mbti}: {count}
                </Badge>
              ))}
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
};
