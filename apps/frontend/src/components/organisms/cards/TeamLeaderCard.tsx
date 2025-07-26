import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';

import { Clock, Crown, Mail, MapPin } from 'lucide-react';

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

interface TeamLeaderCardProps {
  leader: TeamMemberDetail;
}

export const TeamLeaderCard: React.FC<TeamLeaderCardProps> = ({ leader }) => {
  const getSfiaLevelName = (level: number) => {
    const levels = ['Trainee', 'Junior', 'Staff', 'Senior', 'Lead', 'Principal', 'Architect'];
    return levels[Math.floor(level)] || 'Unknown';
  };

  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  const topTechnologies = leader.technologies?.sort((a, b) => b.sfiaLevel - a.sfiaLevel).slice(0, 3) || [];

  return (
    <Card className="border-yellow-200 bg-gradient-to-br from-yellow-50 to-orange-50">
      <CardHeader>
        <CardTitle className="flex items-center gap-2 text-yellow-800">
          <Crown className="h-5 w-5" />
          Líder del Equipo
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="flex items-start gap-4">
          <Avatar className="h-16 w-16 ring-2 ring-yellow-300">
            <AvatarImage src={leader.profilePictureUrl || undefined} alt={leader.fullName} />
            <AvatarFallback className="bg-yellow-200 font-semibold text-yellow-800">
              {getInitials(leader.fullName)}
            </AvatarFallback>
          </Avatar>

          <div className="flex-1 space-y-2">
            <div>
              <h3 className="text-lg font-semibold text-gray-900">{leader.fullName}</h3>
              <p className="text-sm text-gray-600">{leader.role}</p>
            </div>

            <div className="flex flex-wrap gap-2">
              <Badge variant="secondary" className="bg-yellow-100 text-yellow-800">
                {getSfiaLevelName(leader.sfiaLevelGeneral)}
              </Badge>
              {leader.availability && (
                <Badge variant="outline" className="border-green-300 text-green-700">
                  Disponible
                </Badge>
              )}
              {leader.mbti && (
                <Badge variant="outline" className="border-purple-300 text-purple-700">
                  {leader.mbti}
                </Badge>
              )}
            </div>
          </div>
        </div>

        {/* Contact and Location Info */}
        <div className="space-y-2 border-t border-yellow-200 pt-2">
          <div className="flex items-center gap-2 text-sm text-gray-600">
            <Mail className="h-4 w-4" />
            <span>{leader.email}</span>
          </div>
          <div className="flex items-center gap-2 text-sm text-gray-600">
            <MapPin className="h-4 w-4" />
            <span>{leader.country}</span>
          </div>
          <div className="flex items-center gap-2 text-sm text-gray-600">
            <Clock className="h-4 w-4" />
            <span>{leader.timezone}</span>
          </div>
        </div>

        {/* Top Technologies */}
        {topTechnologies.length > 0 && (
          <div className="space-y-2 border-t border-yellow-200 pt-2">
            <h4 className="text-sm font-medium text-gray-700">Tecnologías Principales</h4>
            <div className="flex flex-wrap gap-1">
              {topTechnologies.map((tech, index) => (
                <Badge key={index} variant="outline" className="border-orange-300 text-xs text-orange-700">
                  {tech.technologyName}
                  <span className="ml-1 text-orange-500">Lv.{tech.sfiaLevel}</span>
                </Badge>
              ))}
            </div>
          </div>
        )}

        {/* Personal Interests */}
        {leader.personalInterests && leader.personalInterests.length > 0 && (
          <div className="space-y-2 border-t border-yellow-200 pt-2">
            <h4 className="text-sm font-medium text-gray-700">Intereses Personales</h4>
            <div className="flex flex-wrap gap-1">
              {leader.personalInterests.slice(0, 4).map((interest, index) => (
                <Badge key={index} variant="outline" className="border-blue-300 text-xs text-blue-700">
                  {interest}
                </Badge>
              ))}
              {leader.personalInterests.length > 4 && (
                <Badge variant="outline" className="text-xs text-gray-500">
                  +{leader.personalInterests.length - 4} más
                </Badge>
              )}
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
};
