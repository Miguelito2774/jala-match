// src/components/teams/TeamCard.tsx
import { useState } from 'react';

import { useRouter } from 'next/navigation';

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardFooter, CardHeader } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';
import type { Team } from '@/hooks/useTeams';

import { Code, Crown, Loader2, Trash2, Users } from 'lucide-react';
import { toast } from 'sonner';

interface TeamCardProps {
  team: Team;
  onDelete: (teamId: string) => Promise<void> | void;
}

export const TeamCard = ({ team, onDelete }: TeamCardProps) => {
  const router = useRouter();
  const [isDeleting, setIsDeleting] = useState(false);
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);

  const maxAvatars = 5;
  const visibleMembers = team.members.slice(0, maxAvatars);
  const extraCount = team.members.length - visibleMembers.length;

  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((part) => part[0])
      .join('')
      .toUpperCase();
  };

  const getCompatibilityColorClass = (score: number) => {
    if (score >= 80) return 'text-green-600';
    if (score >= 60) return 'text-yellow-600';
    return 'text-red-600';
  };

  const handleViewTeam = () => {
    router.push(`/teams/${team.teamId}`);
  };

  const handleDeleteClick = (e: React.MouseEvent) => {
    e.stopPropagation(); // Evita que se active handleViewTeam
    setShowConfirmDialog(true);
  };

  const handleConfirmDelete = async () => {
    setIsDeleting(true);
    try {
      await onDelete(team.teamId);
      toast.success(`Equipo "${team.name}" eliminado correctamente`);
    } catch (_error) {
      toast.error('Error al eliminar el equipo', {
        description: 'Por favor intenta nuevamente más tarde.',
      });
    } finally {
      setIsDeleting(false);
      setShowConfirmDialog(false);
    }
  };

  return (
    <>
      <Card
        className="flex h-full cursor-pointer flex-col overflow-hidden transition-all duration-200 hover:shadow-md"
        onClick={handleViewTeam}
      >
        <CardHeader className="bg-gradient-to-r from-blue-50 to-indigo-50 pb-2">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-semibold text-gray-800">{team.name}</h3>
            <div className="flex items-center">
              <span className={`text-lg font-bold ${getCompatibilityColorClass(team.compatibilityScore)}`}>
                {team.compatibilityScore}%
              </span>
            </div>
          </div>
          <div className="mt-1 w-full">
            <Progress value={team.compatibilityScore} className="h-2" />
          </div>
        </CardHeader>

        <CardContent className="flex-1 pt-4">
          <div className="h-full space-y-4">
            {/* Team Members */}
            <div className="flex flex-wrap gap-2">
              {visibleMembers.map((member) => (
                <div
                  key={member.employeeProfileId}
                  className={`flex items-center ${member.isLeader ? 'order-first' : ''}`}
                >
                  <div className="relative">
                    <Avatar className={`h-8 w-8 ${member.isLeader ? 'border-2 border-yellow-400' : ''}`}>
                      <AvatarImage src={member.profilePictureUrl || undefined} alt={member.name} />
                      <AvatarFallback className="bg-blue-100 text-xs text-blue-700">
                        {getInitials(member.name)}
                      </AvatarFallback>
                    </Avatar>
                    {member.isLeader && (
                      <span className="absolute -top-1 -right-1 rounded-full bg-yellow-400 p-0.5">
                        <Crown className="h-3 w-3 text-white" />
                      </span>
                    )}
                  </div>
                  <span className="ml-1 text-xs text-gray-600">{member.name.split(' ')[0]}</span>
                </div>
              ))}
              {extraCount > 0 && (
                <Badge variant="secondary" className="text-xs font-normal">
                  +{extraCount}
                </Badge>
              )}
            </div>

            {/* Required Technologies */}
            <div className="flex flex-wrap gap-1">
              <div className="mr-1 flex items-center">
                <Code className="mr-1 h-4 w-4 text-gray-500" />
                <span className="text-xs text-gray-500">Tech:</span>
              </div>
              {team.requiredTechnologies.map((tech, index) => (
                <Badge key={index} variant="secondary" className="text-xs font-normal">
                  {tech}
                </Badge>
              ))}
            </div>

            {/* Team Strengths Summary */}
            <div className="text-sm text-gray-600">
              <p className="line-clamp-2">{team.analysis.strengths[0]}</p>
            </div>
          </div>
        </CardContent>

        <CardFooter className="flex justify-between bg-gray-50 py-2">
          <div className="flex items-center text-sm text-gray-500">
            <Users className="mr-1 h-4 w-4" />
            <span>{team.members.length} miembros</span>
          </div>
          <div className="flex items-center gap-2">
            <Button
              variant="ghost"
              size="sm"
              className="p-1 text-red-600 hover:bg-red-50 hover:text-red-800"
              onClick={handleDeleteClick}
              disabled={isDeleting}
            >
              {isDeleting ? <Loader2 className="h-4 w-4 animate-spin" /> : <Trash2 className="h-4 w-4" />}
            </Button>
          </div>
        </CardFooter>
      </Card>

      {/* AlertDialog para confirmar eliminación */}
      <AlertDialog open={showConfirmDialog} onOpenChange={setShowConfirmDialog}>
        <AlertDialogContent className="bg-white">
          <AlertDialogHeader>
            <AlertDialogTitle>¿Estás seguro?</AlertDialogTitle>
            <AlertDialogDescription>
              Esta acción no se puede deshacer. Se eliminará permanentemente el equipo &quot;{team.name}&quot; y toda su
              información asociada.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleConfirmDelete}
              className="bg-red-600 text-white hover:bg-red-700"
              disabled={isDeleting}
            >
              {isDeleting ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : null}
              Eliminar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
};
