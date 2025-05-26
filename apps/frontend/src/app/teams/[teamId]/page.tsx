'use client';

import { useEffect, useState } from 'react';
import * as React from 'react';

import { useRouter } from 'next/navigation';

import { AddTeamMemberComponent } from '@/components/organisms/teams/AddMemberDialog';
import { TeamMemberActionsDialog } from '@/components/organisms/teams/TeamMemberActionsDialog';
import { DashboardLayout } from '@/components/templates/DashboardLayout';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from '@/components/ui/alert-dialog';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';
import { Separator } from '@/components/ui/separator';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { useTeamMemberActions } from '@/hooks/useTeamMemberActions';
import { Team, TeamMember, useTeams } from '@/hooks/useTeams';

import {
  AlertCircle,
  ArrowLeft,
  BarChart,
  CheckCircle,
  Code,
  Crown,
  Info,
  Move,
  Trash2,
  UserMinus,
  Users,
} from 'lucide-react';
import { toast } from 'sonner';

interface TeamPageProps {
  params: Promise<{
    teamId: string;
  }>;
}

export default function TeamPage({ params }: TeamPageProps) {
  const router = useRouter();
  const { getTeamById, deleteTeam, loading, error } = useTeams();
  const [team, setTeam] = useState<Team | null>(null);
  const {
    dialogState,
    loading: memberActionsLoading,
    openRemoveDialog,
    openMoveDialog,
    closeDialog,
    handleRemoveMember,
    handleMoveMember,
    getAvailableTeamsForMove,
  } = useTeamMemberActions(team);
  const unwrappedParams = React.use(params);
  const { teamId } = unwrappedParams;

  const handleMembersAdded = async () => {
    try {
      const updated = await getTeamById(teamId);
      setTeam(updated);
    } catch {
      toast.error('Error al recargar datos del equipo');
    }
  };

  const handleMemberActionComplete = async () => {
    await handleMembersAdded();
  };

  useEffect(() => {
    const fetchTeam = async () => {
      try {
        const data = await getTeamById(teamId);
        setTeam(data);
      } catch (err) {
        toast.error('Error al cargar el equipo', {
          description: err instanceof Error ? err.message : 'Ha ocurrido un error inesperado',
        });
      }
    };

    fetchTeam();
  }, [getTeamById, teamId]);

  const handleBack = () => {
    router.push('/teams');
  };

  const handleDelete = async () => {
    if (team) {
      const success = await deleteTeam(team.teamId);

      if (success) {
        toast.success('Equipo eliminado', {
          description: `${team.name} ha sido eliminado correctamente.`,
        });
        router.push('/teams');
      } else {
        toast.error('No se pudo eliminar el equipo. Por favor, inténtalo de nuevo.');
      }
    }
  };

  // Open remove-dialog for a member
  const openRemove = (member: TeamMember) => {
    if (team) openRemoveDialog(member);
  };

  // Open move-dialog for a member
  const openMove = (member: TeamMember) => {
    if (team) openMoveDialog(member);
  };

  const getInitials = (name: string) =>
    name
      .split(' ')
      .map((part) => part[0])
      .join('')
      .toUpperCase();

  const getCompatibilityColorClass = (score: number) => {
    if (score >= 80) return 'text-green-600';
    if (score >= 60) return 'text-yellow-600';
    return 'text-red-600';
  };

  const getCompatibilityBgClass = (score: number) => {
    if (score >= 80) return 'bg-green-100';
    if (score >= 60) return 'bg-yellow-100';
    return 'bg-red-100';
  };

  if (loading && !team) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center p-4">
        <div className="mb-4 h-12 w-12 animate-spin rounded-full border-b-2 border-blue-600"></div>
        <p className="text-gray-600">Cargando información del equipo...</p>
      </div>
    );
  }

  if (error || !team) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center p-4">
        <AlertCircle className="mb-4 h-12 w-12 text-red-500" />
        <h2 className="mb-2 text-xl font-semibold text-gray-900">Error al cargar el equipo</h2>
        <p className="mb-6 text-gray-600">{error || 'No se encontró el equipo solicitado'}</p>
        <Button onClick={handleBack}>Volver a mis equipos</Button>
      </div>
    );
  }

  return (
    <DashboardLayout>
      <div className="container max-w-6xl px-4 py-6 sm:px-6 lg:px-8">
        <div className="mb-6 flex items-center justify-between">
          <Button variant="outline" size="sm" className="flex items-center gap-1" onClick={handleBack}>
            <ArrowLeft className="h-4 w-4" />
            Volver
          </Button>
          <AlertDialog>
            <div className="flex gap-2">
              <AddTeamMemberComponent teamId={teamId} onMembersAdded={handleMembersAdded} />
              <AlertDialogTrigger asChild>
                <Button variant="destructive" size="sm" className="flex items-center gap-1">
                  <Trash2 className="h-4 w-4" />
                  Eliminar equipo
                </Button>
              </AlertDialogTrigger>
            </div>
            <AlertDialogContent className="max-w-md bg-white">
              <AlertDialogHeader className="text-center">
                <AlertDialogTitle className="text-lg">Eliminar equipo</AlertDialogTitle>
                <AlertDialogDescription className="mt-2">
                  ¿Estás seguro de que deseas eliminar permanentemente el equipo &quot;{team?.name}&quot;? Esta acción
                  no se puede deshacer.
                </AlertDialogDescription>
              </AlertDialogHeader>
              <AlertDialogFooter className="gap-2 sm:gap-2">
                <AlertDialogCancel className="flex-1">Cancelar</AlertDialogCancel>
                <AlertDialogAction
                  className="flex-1 bg-red-600 text-white hover:bg-red-700 focus:ring-red-600"
                  onClick={handleDelete}
                >
                  {loading ? 'Eliminando...' : 'Eliminar'}
                </AlertDialogAction>
              </AlertDialogFooter>
            </AlertDialogContent>
          </AlertDialog>
        </div>

        <div className="mb-8">
          <h1 className="mb-2 text-3xl font-bold text-gray-900">{team.name}</h1>
          <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:gap-6">
            <div className="flex items-center gap-2">
              <Users className="h-5 w-5 text-gray-500" />
              <span className="text-gray-600">{team.members.length} miembros</span>
            </div>

            <div className="flex items-center gap-2">
              <BarChart className="h-5 w-5 text-gray-500" />
              <span className="text-gray-600">Compatibilidad: </span>
              <span className={`font-semibold ${getCompatibilityColorClass(team.compatibilityScore)}`}>
                {team.compatibilityScore}%
              </span>
            </div>

            <div className="flex items-center gap-2">
              <Code className="h-5 w-5 text-gray-500" />
              <div className="flex flex-wrap gap-1">
                {team.requiredTechnologies.map((tech, index) => (
                  <Badge key={index} variant="outline" className="text-xs">
                    {tech}
                  </Badge>
                ))}
              </div>
            </div>
          </div>
        </div>

        <Card className="mb-8 w-full">
          <CardHeader className="pb-2">
            <CardTitle className="text-lg">Nivel de Compatibilidad</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="mb-2 flex items-center gap-3">
              <div
                className={`h-16 w-16 rounded-full ${getCompatibilityBgClass(team.compatibilityScore)} flex items-center justify-center`}
              >
                <span className={`text-2xl font-bold ${getCompatibilityColorClass(team.compatibilityScore)}`}>
                  {team.compatibilityScore}%
                </span>
              </div>
              <div className="flex-1">
                <Progress value={team.compatibilityScore} className="h-3" />
                <p className="mt-2 text-sm text-gray-600">
                  {team.compatibilityScore >= 80
                    ? 'Este equipo tiene una compatibilidad excepcional'
                    : team.compatibilityScore >= 60
                      ? 'Este equipo tiene una compatibilidad aceptable'
                      : 'Este equipo podría presentar desafíos de compatibilidad'}
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        <div className="grid grid-cols-1 gap-6 lg:grid-cols-12">
          <Card className="lg:col-span-7">
            <CardHeader>
              <CardTitle className="text-lg">Miembros del Equipo</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              {team.members.map((member) => (
                <div
                  key={member.employeeProfileId}
                  className={`flex items-start rounded-lg p-4 ${
                    member.isLeader ? 'border border-blue-100 bg-blue-50' : 'bg-gray-50'
                  }`}
                >
                  <div className="relative mr-4">
                    <Avatar className={`h-12 w-12 ${member.isLeader ? 'border-2 border-yellow-400' : ''}`}>
                      <AvatarFallback
                        className={`${member.isLeader ? 'bg-yellow-100 text-yellow-800' : 'bg-blue-100 text-blue-700'}`}
                      >
                        {getInitials(member.name)}
                      </AvatarFallback>
                    </Avatar>
                    {member.isLeader && (
                      <TooltipProvider>
                        <Tooltip>
                          <TooltipTrigger asChild>
                            <div className="absolute -top-2 -right-2 rounded-full bg-yellow-400 p-1">
                              <Crown className="h-4 w-4 text-white" />
                            </div>
                          </TooltipTrigger>
                          <TooltipContent>
                            <p>Líder del equipo</p>
                          </TooltipContent>
                        </Tooltip>
                      </TooltipProvider>
                    )}
                  </div>
                  <div className="flex-grow">
                    <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
                      <h3 className="text-lg font-semibold">{member.name}</h3>
                      <div className="flex items-center gap-2">
                        <Badge variant={member.isLeader ? 'default' : 'outline'} className="my-1 sm:my-0">
                          {member.role}
                        </Badge>

                        {/* Only show action buttons for non-leaders */}
                        {!member.isLeader && (
                          <div className="flex items-center gap-1">
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => openMove(member)}
                              title="Mover a otro equipo"
                            >
                              <Move className="h-4 w-4" />
                            </Button>
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => openRemove(member)}
                              title="Eliminar del equipo"
                            >
                              <UserMinus className="h-4 w-4 text-red-600" />
                            </Button>
                          </div>
                        )}
                      </div>
                    </div>
                    <div className="mt-1 flex items-center">
                      <span className="mr-2 text-sm text-gray-600">Nivel SFIA:</span>
                      <Badge variant="secondary" className="text-xs">
                        {member.sfiaLevel}
                      </Badge>
                    </div>
                  </div>
                </div>
              ))}
            </CardContent>
          </Card>

          <div className="space-y-6 lg:col-span-5">
            <Card>
              <CardHeader>
                <CardTitle className="text-lg">Análisis del Equipo</CardTitle>
              </CardHeader>
              <CardContent className="space-y-6">
                <div>
                  <h3 className="mb-2 flex items-center text-sm font-medium text-green-600">
                    <CheckCircle className="mr-2 h-4 w-4" /> Fortalezas
                  </h3>
                  <ul className="space-y-2 text-sm">
                    {team.analysis.strengths.map((strength, idx) => (
                      <li key={idx} className="flex">
                        <span className="mr-2 text-green-600">•</span>
                        <span className="text-gray-700">{strength}</span>
                      </li>
                    ))}
                  </ul>
                </div>

                <Separator />

                <div>
                  <h3 className="mb-2 flex items-center text-sm font-medium text-amber-600">
                    <AlertCircle className="mr-2 h-4 w-4" /> Debilidades
                  </h3>
                  <ul className="space-y-2 text-sm">
                    {team.analysis.weaknesses.map((weakness, idx) => (
                      <li key={idx} className="flex">
                        <span className="mr-2 text-amber-600">•</span>
                        <span className="text-gray-700">{weakness}</span>
                      </li>
                    ))}
                  </ul>
                </div>

                <Separator />

                <div>
                  <h3 className="mb-2 flex items-center text-sm font-medium text-blue-600">
                    <Info className="mr-2 h-4 w-4" /> Compatibilidad
                  </h3>
                  <p className="text-sm text-gray-700">{team.analysis.compatibility}</p>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle className="text-lg">Distribución de Pesos</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {Object.entries(team.weights).map(([key, value]) => {
                  const labelMap: Record<string, string> = {
                    sfiaWeight: 'Habilidades SFIA',
                    technicalWeight: 'Competencias Técnicas',
                    psychologicalWeight: 'Perfil Psicológico',
                    experienceWeight: 'Experiencia',
                    languageWeight: 'Idiomas',
                    interestsWeight: 'Intereses',
                    timezoneWeight: 'Zona Horaria',
                  };

                  return (
                    <div key={key} className="space-y-1">
                      <div className="flex justify-between text-sm">
                        <span>{labelMap[key] || key}</span>
                        <span className="font-mono">{value}%</span>
                      </div>
                      <Progress value={value} className="h-2" />
                    </div>
                  );
                })}
              </CardContent>
            </Card>
          </div>
        </div>

        <TeamMemberActionsDialog
          dialogState={dialogState}
          loading={memberActionsLoading}
          closeDialog={closeDialog}
          handleRemoveMember={handleRemoveMember}
          handleMoveMember={handleMoveMember}
          getAvailableTeamsForMove={getAvailableTeamsForMove}
          onMemberRemoved={handleMemberActionComplete}
          onMemberMoved={handleMemberActionComplete}
        />
      </div>
    </DashboardLayout>
  );
}
