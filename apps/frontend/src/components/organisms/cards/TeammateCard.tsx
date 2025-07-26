'use client';

import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { Teammate } from '@/hooks/useEmployeeTeams';

import { Clock, Code, Crown, Heart, Languages, MapPin, Star, User, Wrench } from 'lucide-react';

interface TeammateCardProps {
  teammate: Teammate;
}

const getInitials = (firstName: string, lastName: string) => {
  return `${firstName[0]}${lastName[0]}`.toUpperCase();
};

const getSfiaLevelColor = (level: number) => {
  if (level <= 2) return 'bg-gray-100 text-gray-700';
  if (level <= 4) return 'bg-blue-100 text-blue-700';
  if (level <= 6) return 'bg-green-100 text-green-700';
  return 'bg-purple-100 text-purple-700';
};

const getProficiencyColor = (proficiency: string) => {
  switch (proficiency.toLowerCase()) {
    case 'beginner':
    case 'principiante':
      return 'bg-red-100 text-red-700';
    case 'intermediate':
    case 'intermedio':
      return 'bg-yellow-100 text-yellow-700';
    case 'advanced':
    case 'avanzado':
      return 'bg-green-100 text-green-700';
    case 'native':
    case 'nativo':
      return 'bg-blue-100 text-blue-700';
    default:
      return 'bg-gray-100 text-gray-700';
  }
};

const getExperienceLevelText = (level: number) => {
  switch (level) {
    case 1:
      return 'Junior';
    case 2:
      return 'Semi-Senior';
    case 3:
      return 'Senior';
    default:
      return `Nivel ${level}`;
  }
};

export const TeammateCard = ({ teammate }: TeammateCardProps) => {
  return (
    <Card className="group relative h-full transition-all duration-200 hover:shadow-lg">
      <CardHeader className="pb-3">
        <div className="flex items-start justify-between">
          <div className="flex items-center space-x-3">
            <div className="relative">
              <Avatar className="h-12 w-12">
                <AvatarImage src={teammate.profilePictureUrl || undefined} alt={teammate.fullName} />
                <AvatarFallback className="bg-gradient-to-br from-blue-500 to-purple-600 font-semibold text-white">
                  {getInitials(teammate.firstName, teammate.lastName)}
                </AvatarFallback>
              </Avatar>
              {teammate.isLeader && (
                <TooltipProvider>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <div className="absolute -top-1 -right-1 rounded-full bg-yellow-400 p-1 shadow-sm">
                        <Crown className="h-3 w-3 text-yellow-800" />
                      </div>
                    </TooltipTrigger>
                    <TooltipContent>
                      <p>Líder del equipo</p>
                    </TooltipContent>
                  </Tooltip>
                </TooltipProvider>
              )}
            </div>
            <div className="min-w-0 flex-1">
              <h3 className="truncate font-semibold text-gray-900">{teammate.fullName}</h3>
              <p className="truncate text-sm text-gray-600">{teammate.email}</p>
              <div className="mt-1 flex items-center space-x-2">
                <Badge variant="outline" className="text-xs">
                  {teammate.role}
                </Badge>
                <Badge className={`text-xs ${getSfiaLevelColor(teammate.sfiaLevel)}`}>SFIA {teammate.sfiaLevel}</Badge>
              </div>
            </div>
          </div>
          <div className="flex flex-col items-end space-y-1">
            <div
              className={`rounded-full px-2 py-1 text-xs font-medium ${
                teammate.availability ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
              }`}
            >
              {teammate.availability ? 'Disponible' : 'Ocupado'}
            </div>
          </div>
        </div>
      </CardHeader>

      <CardContent className="space-y-4">
        {/* Location and Basic Info */}
        <div className="flex items-center justify-between text-sm text-gray-600">
          <div className="flex items-center space-x-1">
            <MapPin className="h-4 w-4" />
            <span>{teammate.country}</span>
          </div>
          <div className="flex items-center space-x-1">
            <Clock className="h-4 w-4" />
            <span>{teammate.timezone}</span>
          </div>
        </div>

        {teammate.mbti && (
          <div className="flex items-center space-x-2">
            <User className="h-4 w-4 text-purple-500" />
            <span className="text-sm text-gray-600">MBTI:</span>
            <Badge variant="outline" className="bg-purple-50 text-xs text-purple-700">
              {teammate.mbti}
            </Badge>
          </div>
        )}

        <Separator />

        {/* Specialized Roles */}
        {teammate.specializedRoles.length > 0 && (
          <div className="space-y-2">
            <div className="flex items-center space-x-2">
              <Wrench className="h-4 w-4 text-blue-500" />
              <span className="text-sm font-medium text-gray-700">Roles Especializados</span>
            </div>
            <div className="space-y-1">
              {teammate.specializedRoles.slice(0, 2).map((role, index) => (
                <div key={index} className="flex items-center justify-between">
                  <div className="min-w-0 flex-1">
                    <p className="truncate text-xs font-medium text-gray-900">{role.roleName}</p>
                    <p className="truncate text-xs text-gray-600">{role.technicalArea}</p>
                  </div>
                  <div className="ml-2 flex items-center space-x-1">
                    <Badge variant="outline" className="text-xs">
                      {getExperienceLevelText(role.level)}
                    </Badge>
                    <span className="text-xs text-gray-500">{role.yearsExperience}a</span>
                  </div>
                </div>
              ))}
              {teammate.specializedRoles.length > 2 && (
                <p className="text-xs text-gray-500">+{teammate.specializedRoles.length - 2} más</p>
              )}
            </div>
          </div>
        )}

        {/* Top Technologies */}
        {teammate.technologies.length > 0 && (
          <div className="space-y-2">
            <div className="flex items-center space-x-2">
              <Code className="h-4 w-4 text-green-500" />
              <span className="text-sm font-medium text-gray-700">Tecnologías Principales</span>
            </div>
            <div className="flex flex-wrap gap-1">
              {teammate.technologies
                .sort((a, b) => b.sfiaLevel - a.sfiaLevel)
                .slice(0, 4)
                .map((tech, index) => (
                  <TooltipProvider key={index}>
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <Badge variant="outline" className={`cursor-help text-xs ${getSfiaLevelColor(tech.sfiaLevel)}`}>
                          {tech.technologyName}
                        </Badge>
                      </TooltipTrigger>
                      <TooltipContent>
                        <div className="space-y-1">
                          <p className="font-medium">{tech.technologyName}</p>
                          <p className="text-xs">Categoría: {tech.categoryName}</p>
                          <p className="text-xs">SFIA: {tech.sfiaLevel}</p>
                          <p className="text-xs">Experiencia: {tech.yearsExperience} años</p>
                          {tech.version && <p className="text-xs">Versión: {tech.version}</p>}
                        </div>
                      </TooltipContent>
                    </Tooltip>
                  </TooltipProvider>
                ))}
              {teammate.technologies.length > 4 && (
                <Badge variant="outline" className="text-xs text-gray-500">
                  +{teammate.technologies.length - 4}
                </Badge>
              )}
            </div>
          </div>
        )}

        {/* Languages */}
        {teammate.languages.length > 0 && (
          <div className="space-y-2">
            <div className="flex items-center space-x-2">
              <Languages className="h-4 w-4 text-orange-500" />
              <span className="text-sm font-medium text-gray-700">Idiomas</span>
            </div>
            <div className="flex flex-wrap gap-1">
              {teammate.languages.slice(0, 3).map((language, index) => (
                <Badge key={index} className={`text-xs ${getProficiencyColor(language.proficiency)}`}>
                  {language.language} ({language.proficiency})
                </Badge>
              ))}
              {teammate.languages.length > 3 && (
                <Badge variant="outline" className="text-xs text-gray-500">
                  +{teammate.languages.length - 3}
                </Badge>
              )}
            </div>
          </div>
        )}

        {/* Personal Interests */}
        {teammate.personalInterests.length > 0 && (
          <div className="space-y-2">
            <div className="flex items-center space-x-2">
              <Heart className="h-4 w-4 text-pink-500" />
              <span className="text-sm font-medium text-gray-700">Intereses</span>
            </div>
            <div className="flex flex-wrap gap-1">
              {teammate.personalInterests.slice(0, 3).map((interest, index) => (
                <Badge key={index} variant="outline" className="bg-pink-50 text-xs text-pink-700">
                  {interest}
                </Badge>
              ))}
              {teammate.personalInterests.length > 3 && (
                <Badge variant="outline" className="text-xs text-gray-500">
                  +{teammate.personalInterests.length - 3}
                </Badge>
              )}
            </div>
          </div>
        )}

        {/* Overall SFIA Level */}
        <div className="border-t border-gray-100 pt-2">
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-2">
              <Star className="h-4 w-4 text-yellow-500" />
              <span className="text-sm font-medium text-gray-700">Nivel General</span>
            </div>
            <Badge className={`text-xs ${getSfiaLevelColor(teammate.sfiaLevelGeneral)}`}>
              SFIA {teammate.sfiaLevelGeneral}
            </Badge>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};
