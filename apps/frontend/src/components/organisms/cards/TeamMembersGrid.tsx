import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';

import { Crown, Globe, MessageCircle, Star, Users, Zap } from 'lucide-react';

interface TeamMemberDetail {
  employeeProfileId: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  role: string;
  sfiaLevel: number;
  isLeader: boolean;
  country: string;
  timezone: string;
  mbti: string;
  availability: boolean;
  sfiaLevelGeneral: number;
  profilePictureUrl?: string;
  specializedRoles: Array<{
    roleName: string;
    technicalArea: string;
    level: number;
    yearsExperience: number;
  }>;
  technologies: Array<{
    technologyName: string;
    categoryName: string;
    sfiaLevel: number;
    yearsExperience: number;
    version: string;
  }>;
  languages: Array<{
    language: string;
    proficiency: string;
  }>;
  personalInterests: string[];
}

interface TeamMembersGridProps {
  members: TeamMemberDetail[];
  currentUserId?: string;
}

export const TeamMembersGrid: React.FC<TeamMembersGridProps> = ({ members, currentUserId }) => {
  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  const getSfiaProgress = (level: number) => {
    return (level / 6) * 100; // Assuming max level is 6
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Users className="h-5 w-5" />
          Miembros del Equipo ({members.length})
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {members.map((member) => (
            <TooltipProvider key={member.employeeProfileId}>
              {' '}
              <div
                className={`rounded-lg border p-4 transition-all hover:shadow-md ${
                  member.employeeProfileId === currentUserId
                    ? 'border-blue-300 bg-blue-50'
                    : 'border-gray-200 hover:border-gray-300'
                }`}
              >
                <div className="space-y-3">
                  {/* Header with Avatar and Basic Info */}
                  <div className="flex items-start gap-3">
                    <div className="relative">
                      <Avatar className="h-12 w-12 ring-2 ring-gray-200">
                        <AvatarImage src={member.profilePictureUrl || undefined} alt={member.fullName} />
                        <AvatarFallback className="font-semibold">{getInitials(member.fullName)}</AvatarFallback>
                      </Avatar>
                    </div>

                    <div className="min-w-0 flex-1">
                      <div className="flex items-center gap-2">
                        <h3 className="truncate text-sm font-semibold">{member.fullName}</h3>
                        {member.isLeader && <Crown className="h-4 w-4 text-yellow-500" />}
                        {member.employeeProfileId === currentUserId && <Star className="h-4 w-4 text-blue-500" />}
                      </div>
                      <p className="truncate text-xs text-gray-600">{member.role}</p>
                    </div>
                  </div>

                  {/* SFIA Level Progress */}
                  <div className="space-y-1">
                    <div className="flex items-center justify-between">
                      <span className="text-xs font-medium text-gray-700">Nivel SFIA</span>
                      <span className="text-xs text-gray-600">{member.sfiaLevelGeneral}</span>
                    </div>
                    <Progress value={getSfiaProgress(member.sfiaLevelGeneral)} className="h-1.5" />
                  </div>

                  {/* Location and MBTI */}
                  <div className="flex items-center gap-2 text-xs text-gray-600">
                    <div className="flex items-center gap-1">
                      <Globe className="h-3 w-3" />
                      <span>{member.country}</span>
                    </div>
                    {member.mbti && (
                      <Badge variant="outline" className="px-1 py-0 text-xs">
                        {member.mbti}
                      </Badge>
                    )}
                  </div>

                  {/* Top Technologies */}
                  {member.technologies && member.technologies.length > 0 && (
                    <div className="space-y-1">
                      <div className="flex items-center gap-1">
                        <Zap className="h-3 w-3 text-gray-500" />
                        <span className="text-xs font-medium text-gray-700">Tecnologías</span>
                      </div>
                      <div className="flex flex-wrap gap-1">
                        {member.technologies
                          .sort((a, b) => b.sfiaLevel - a.sfiaLevel)
                          .slice(0, 3)
                          .map((tech, index) => (
                            <Tooltip key={index}>
                              <TooltipTrigger>
                                <Badge variant="secondary" className="cursor-help px-1.5 py-0.5 text-xs">
                                  {tech.technologyName}
                                </Badge>
                              </TooltipTrigger>
                              <TooltipContent>
                                <p>
                                  Nivel: {tech.sfiaLevel} | Experiencia: {tech.yearsExperience} años
                                </p>
                              </TooltipContent>
                            </Tooltip>
                          ))}
                        {member.technologies.length > 3 && (
                          <Badge variant="outline" className="px-1.5 py-0.5 text-xs">
                            +{member.technologies.length - 3}
                          </Badge>
                        )}
                      </div>
                    </div>
                  )}

                  {/* Languages */}
                  {member.languages && member.languages.length > 0 && (
                    <div className="space-y-1">
                      <div className="flex items-center gap-1">
                        <MessageCircle className="h-3 w-3 text-gray-500" />
                        <span className="text-xs font-medium text-gray-700">Idiomas</span>
                      </div>
                      <div className="flex flex-wrap gap-1">
                        {member.languages.slice(0, 3).map((lang, index) => (
                          <Tooltip key={index}>
                            <TooltipTrigger>
                              <Badge variant="outline" className="cursor-help px-1.5 py-0.5 text-xs">
                                {lang.language}
                              </Badge>
                            </TooltipTrigger>
                            <TooltipContent>
                              <p>Nivel: {lang.proficiency}</p>
                            </TooltipContent>
                          </Tooltip>
                        ))}
                        {member.languages.length > 3 && (
                          <Badge variant="outline" className="px-1.5 py-0.5 text-xs">
                            +{member.languages.length - 3}
                          </Badge>
                        )}
                      </div>
                    </div>
                  )}

                  {/* Personal Interests */}
                  {member.personalInterests && member.personalInterests.length > 0 && (
                    <div className="space-y-1">
                      <span className="text-xs font-medium text-gray-700">Intereses</span>
                      <div className="flex flex-wrap gap-1">
                        {member.personalInterests.map((interest, index) => (
                          <Badge
                            key={index}
                            variant="outline"
                            className="border-blue-200 px-1.5 py-0.5 text-xs text-blue-700"
                          >
                            {interest}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              </div>
            </TooltipProvider>
          ))}
        </div>
      </CardContent>
    </Card>
  );
};
