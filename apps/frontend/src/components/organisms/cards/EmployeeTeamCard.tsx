'use client';

import { useRouter } from 'next/navigation';

import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';
import { EmployeeTeam } from '@/hooks/useEmployeeTeams';

import { ArrowRight, Crown, Star, Users } from 'lucide-react';

interface EmployeeTeamCardProps {
  team: EmployeeTeam;
}

const getInitials = (firstName: string, lastName: string) => {
  return `${firstName[0]}${lastName[0]}`.toUpperCase();
};

const getCompatibilityColor = (score: number) => {
  if (score >= 80) return 'text-green-600';
  if (score >= 60) return 'text-yellow-600';
  return 'text-red-600';
};

const getCompatibilityBgColor = (score: number) => {
  if (score >= 80) return 'bg-green-50';
  if (score >= 60) return 'bg-yellow-50';
  return 'bg-red-50';
};

export const EmployeeTeamCard = ({ team }: EmployeeTeamCardProps) => {
  const router = useRouter();
  const maxAvatars = 4;
  const visibleTeammates = team.teammates.slice(0, maxAvatars);
  const extraCount = team.teammates.length - visibleTeammates.length;
  const leader = team.teammates.find((t) => t.isLeader);

  const handleCardClick = () => {
    router.push(`/team/${team.teamId}`);
  };

  return (
    <Card
      className="group relative cursor-pointer transition-all duration-200 hover:scale-[1.02] hover:shadow-lg"
      onClick={handleCardClick}
    >
      <CardHeader className="pb-3">
        <div className="flex items-start justify-between">
          <div className="min-w-0 flex-1">
            <CardTitle className="flex items-center gap-2 truncate text-lg font-semibold text-gray-900 transition-colors group-hover:text-blue-600">
              {team.teamName}
              <ArrowRight className="h-4 w-4 text-blue-600 opacity-0 transition-opacity group-hover:opacity-100" />
            </CardTitle>
            <p className="mt-1 text-sm text-gray-600">Creado por {team.creatorName}</p>
          </div>
          <div className="flex flex-col items-end space-y-1">
            <div
              className={`rounded-full px-2 py-1 text-xs font-medium ${
                team.isActive ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'
              }`}
            >
              {team.isActive ? 'Activo' : 'Inactivo'}
            </div>
            {team.isCurrentUserLeader && (
              <Badge variant="outline" className="bg-yellow-50 text-xs text-yellow-700">
                <Crown className="mr-1 h-3 w-3" />
                Tu equipo
              </Badge>
            )}
          </div>
        </div>
      </CardHeader>

      <CardContent className="space-y-4">
        {/* Team Statistics */}
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-2">
            <Users className="h-4 w-4 text-gray-500" />
            <span className="text-sm text-gray-600">
              {team.teammates.length} miembro{team.teammates.length !== 1 ? 's' : ''}
            </span>
          </div>
          <div className="flex items-center space-x-2">
            <Star className="h-4 w-4 text-yellow-500" />
            <span className={`text-sm font-medium ${getCompatibilityColor(team.compatibilityScore)}`}>
              {team.compatibilityScore}% compatibilidad
            </span>
          </div>
        </div>

        {/* Compatibility Progress */}
        <div className={`rounded-lg p-3 ${getCompatibilityBgColor(team.compatibilityScore)}`}>
          <div className="mb-2 flex items-center justify-between">
            <span className="text-sm font-medium text-gray-700">Nivel de Compatibilidad</span>
            <span className={`text-sm font-bold ${getCompatibilityColor(team.compatibilityScore)}`}>
              {team.compatibilityScore}%
            </span>
          </div>
          <Progress value={team.compatibilityScore} className="h-2" />
          <p className="mt-1 text-xs text-gray-600">
            {team.compatibilityScore >= 80
              ? 'Compatibilidad excepcional'
              : team.compatibilityScore >= 60
                ? 'Buena compatibilidad'
                : 'Compatibilidad moderada'}
          </p>
        </div>

        {/* Team Leader */}
        {leader && (
          <div className="space-y-2">
            <p className="text-sm font-medium text-gray-700">Líder del equipo</p>
            <div className="flex items-center space-x-3">
              <div className="relative">
                <Avatar className="h-8 w-8">
                  <AvatarImage src={leader.profilePictureUrl} alt={leader.fullName} />
                  <AvatarFallback className="bg-gradient-to-br from-yellow-400 to-orange-500 text-sm font-semibold text-white">
                    {getInitials(leader.firstName, leader.lastName)}
                  </AvatarFallback>
                </Avatar>
                <div className="absolute -top-1 -right-1 rounded-full bg-yellow-400 p-0.5">
                  <Crown className="h-2.5 w-2.5 text-yellow-800" />
                </div>
              </div>
              <div className="min-w-0 flex-1">
                <p className="truncate text-sm font-medium text-gray-900">{leader.fullName}</p>
                <p className="truncate text-xs text-gray-600">{leader.role}</p>
              </div>
              <Badge variant="outline" className="text-xs">
                SFIA {leader.sfiaLevel}
              </Badge>
            </div>
          </div>
        )}

        {/* Team Members Preview */}
        <div className="space-y-2">
          <p className="text-sm font-medium text-gray-700">Miembros del equipo</p>
          <div className="flex items-center space-x-2">
            <div className="flex -space-x-2">
              {visibleTeammates.map((teammate) => (
                <Avatar key={teammate.employeeProfileId} className="h-8 w-8 border-2 border-white">
                  <AvatarImage src={teammate.profilePictureUrl} alt={teammate.fullName} />
                  <AvatarFallback className="bg-gradient-to-br from-blue-500 to-purple-600 text-sm font-semibold text-white">
                    {getInitials(teammate.firstName, teammate.lastName)}
                  </AvatarFallback>
                </Avatar>
              ))}
              {extraCount > 0 && (
                <div className="flex h-8 w-8 items-center justify-center rounded-full border-2 border-white bg-gray-100 text-xs font-medium text-gray-600">
                  +{extraCount}
                </div>
              )}
            </div>
            <div className="flex-1 text-xs text-gray-600">
              {team.teammates.map((teammate, index) => (
                <span key={teammate.employeeProfileId}>
                  {teammate.firstName}
                  {index < team.teammates.length - 1 ? ', ' : ''}
                </span>
              ))}
            </div>
          </div>
        </div>

        {/* Top Technologies in the Team */}
        {team.teammates.some((t) => t.technologies.length > 0) && (
          <div className="space-y-2">
            <p className="text-sm font-medium text-gray-700">Tecnologías del equipo</p>
            <div className="flex flex-wrap gap-1">
              {[
                ...new Set(
                  team.teammates.flatMap((t) => t.technologies.slice(0, 2).map((tech) => tech.technologyName)),
                ),
              ]
                .slice(0, 6)
                .map((tech) => (
                  <Badge key={tech} variant="outline" className="text-xs">
                    {tech}
                  </Badge>
                ))}
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
};
