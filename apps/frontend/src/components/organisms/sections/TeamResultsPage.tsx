'use client';

import { useState } from 'react';

import { adaptGeneratedTeamToAITeamResponse } from '@/app/utils/adapters';
import { Button } from '@/components/atoms/buttons/Button';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Progress } from '@/components/ui/progress';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { useProfileImages } from '@/hooks/useProfileImages';
import { GeneratedTeamResponse } from '@/hooks/useTeamGenerator';
import { TeamWeights, useTeams } from '@/hooks/useTeams';

import { AlertCircle, ArrowLeft, CheckCircle, Crown, Info, User } from 'lucide-react';
import { toast } from 'sonner';

import { buildCreateTeamRequest } from '../../../app/utils';

interface TeamMember {
  id: string;
  name: string;
  role: string;
  sfia_Level: number;
}

interface TeamResultsPageProps {
  teamData: GeneratedTeamResponse;
  formData: {
    creatorId: string;
    teamName?: string;
    requiredTechnologies: string[];
    weights: TeamWeights;
  };
  onBack: () => void;
  onSuccess?: (teamId: string) => void;
}

export const TeamResultsPage = ({ teamData, formData, onBack, onSuccess }: TeamResultsPageProps) => {
  const { createTeam } = useTeams();

  const [teamName, setTeamName] = useState(formData.teamName || 'Equipo Generado');
  const [isEditingName, setIsEditingName] = useState(false);
  const [currentLeader, setCurrentLeader] = useState(teamData.recommended_Leader.id);
  const [currentTeamMembers, setCurrentTeamMembers] = useState(teamData.teams[0].members);
  const [showMemberDetails, setShowMemberDetails] = useState<string | null>(null);
  const [showRecommendedMembers, setShowRecommendedMembers] = useState(false);
  const [showLeaderRationale, setShowLeaderRationale] = useState<string | null>(null);
  const [isCreatingTeam, setIsCreatingTeam] = useState(false);

  const [usedRecommendedMembers, setUsedRecommendedMembers] = useState<string[]>([]);

  const [leaderRationales, setLeaderRationales] = useState<Record<string, string>>({
    [teamData.recommended_Leader.id]: teamData.recommended_Leader.rationale,
  });

  // Hook to fetch profile images for all team members
  const memberIds = currentTeamMembers.map((member) => member.id);
  const { getProfileImage } = useProfileImages(memberIds);

  const analysis = teamData.team_Analysis;
  const score = teamData.compatibility_Score;

  const [originalMembers] = useState(teamData.teams[0].members);
  const [originalLeader] = useState(teamData.recommended_Leader.id);

  const isLeader = (memberId: string) => memberId === currentLeader;

  const getCurrentLeaderRationale = () => {
    return leaderRationales[currentLeader] || 'Seleccionado manualmente como líder del equipo.';
  };

  const toggleLeaderRationale = (memberId: string) => {
    if (showLeaderRationale === memberId) {
      setShowLeaderRationale(null);
    } else {
      setShowLeaderRationale(memberId);
    }
  };

  const handleLeaderChange = (memberId: string) => {
    if (!leaderRationales[memberId]) {
      const member = currentTeamMembers.find((m) => m.id === memberId);
      setLeaderRationales({
        ...leaderRationales,
        [memberId]: `${member?.name} ha sido seleccionado como líder basado en su rol como ${member?.role} y su nivel SFIA ${member?.sfia_Level}.`,
      });
    }
    setCurrentLeader(memberId);
  };

  const handleSwapMember = (currentMemberId: string, recommendedMemberId: string) => {
    const recommendedMember = teamData.recommended_members.find((member) => member.id === recommendedMemberId);

    if (!recommendedMember) return;

    const newTeamMember: TeamMember = {
      id: recommendedMember.id,
      name: recommendedMember.name,
      role: currentTeamMembers.find((m) => m.id === currentMemberId)?.role || 'Developer',
      sfia_Level: Math.floor(Math.random() * 3) + 3,
    };

    const updatedMembers = currentTeamMembers.map((member) => (member.id === currentMemberId ? newTeamMember : member));
    setUsedRecommendedMembers([...usedRecommendedMembers, recommendedMemberId]);

    setCurrentTeamMembers(updatedMembers);
    setShowMemberDetails(null);
    setShowRecommendedMembers(false);
  };

  const handleRestoreOriginal = () => {
    setCurrentLeader(originalLeader);
    setCurrentTeamMembers(originalMembers);
    setUsedRecommendedMembers([]);
  };

  const filteredRecommendedMembers = teamData.recommended_members.filter(
    (member) => !usedRecommendedMembers.includes(member.id),
  );

  const handleConfirmTeam = async () => {
    setIsCreatingTeam(true);

    const teamBuilderData = {
      teamName: teamName,
      creatorId: formData.creatorId,
      weights: formData.weights,
      requiredTechnologies: formData.requiredTechnologies,
    };

    const updatedTeamData = {
      ...teamData,
      teams: [
        {
          ...teamData.teams[0],
          members: currentTeamMembers,
        },
      ],
      recommended_Leader: {
        ...teamData.recommended_Leader,
        id: currentLeader,
        rationale: getCurrentLeaderRationale(),
      },
    };

    const requestData = buildCreateTeamRequest(
      adaptGeneratedTeamToAITeamResponse(updatedTeamData),
      teamBuilderData,
      currentLeader,
    );

    try {
      const result = await createTeam(requestData);
      if (result) {
        toast.success('Equipo creado correctamente', {
          description: `El equipo ${result.name} fue creado exitosamente`,
        });

        if (onSuccess) {
          onSuccess(result.teamId);
        }
      } else {
        toast.error('Error al crear el equipo', {
          description: 'Por favor verifica los datos e intenta nuevamente',
        });
      }
    } catch (_error) {
      toast.error('Error al crear el equipo', {
        description: 'Ocurrió un error en el servidor. Intenta más tarde.',
      });
    } finally {
      setIsCreatingTeam(false);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <Button onClick={onBack} variant="outline" size="sm" className="flex items-center gap-1">
          <ArrowLeft className="h-4 w-4" />
          Volver
        </Button>

        <div className="flex items-center gap-2">
          {isEditingName ? (
            <input
              type="text"
              value={teamName}
              onChange={(e) => setTeamName(e.target.value)}
              onBlur={() => setIsEditingName(false)}
              onKeyDown={(e) => e.key === 'Enter' && setIsEditingName(false)}
              className="rounded border border-gray-300 px-2 py-1 text-lg font-semibold"
              autoFocus
            />
          ) : (
            <h2 className="text-lg font-semibold">{teamName}</h2>
          )}
          <button onClick={() => setIsEditingName(!isEditingName)} className="text-gray-500 hover:text-gray-700">
            ✏️
          </button>
        </div>
      </div>

      {/* Score Bar */}
      <div className="rounded-lg bg-white p-4 shadow">
        <div className="mb-2 flex items-center justify-between">
          <h3 className="font-medium">Compatibilidad</h3>
          <span className="font-bold">{score}%</span>
        </div>
        <Progress value={score} className="h-3 bg-gray-200" />
      </div>

      {/* Team Management Actions */}
      <div className="flex flex-wrap items-center justify-between gap-2">
        <div>
          <Button
            variant="outline"
            onClick={() => setShowRecommendedMembers(true)}
            className="mr-2"
            disabled={filteredRecommendedMembers.length === 0}
          >
            Ver Miembros Recomendados
            {filteredRecommendedMembers.length > 0 && (
              <span className="ml-2 rounded-full bg-blue-100 px-2 py-0.5 text-xs text-blue-700">
                {filteredRecommendedMembers.length}
              </span>
            )}
          </Button>

          <Button variant="ghost" onClick={handleRestoreOriginal} className="text-gray-500">
            Restaurar Original
          </Button>
        </div>

        <Button onClick={handleConfirmTeam} disabled={isCreatingTeam} className="bg-blue-600 hover:bg-blue-700">
          {isCreatingTeam ? 'Creando equipo...' : 'Confirmar Equipo'}
        </Button>
      </div>

      {/* Team Members */}
      <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
        {currentTeamMembers.map((member) => (
          <div
            key={member.id}
            className={`relative rounded-lg bg-white p-4 shadow transition-shadow hover:shadow-md ${
              isLeader(member.id) ? 'border-2 border-yellow-400' : ''
            }`}
          >
            {isLeader(member.id) ? (
              <div className="absolute -top-3 -right-3 rounded-full bg-yellow-400 p-1 shadow">
                <TooltipProvider>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <button onClick={() => toggleLeaderRationale(member.id)}>
                        <Crown className="h-6 w-6 text-white" />
                      </button>
                    </TooltipTrigger>
                    <TooltipContent className="hidden bg-white p-3 shadow-lg md:block">
                      <p className="max-w-xs text-sm">{getCurrentLeaderRationale()}</p>
                    </TooltipContent>
                  </Tooltip>
                </TooltipProvider>
              </div>
            ) : (
              <button
                onClick={() => handleLeaderChange(member.id)}
                className="absolute -top-3 -right-3 rounded-full bg-gray-200 p-1 shadow hover:bg-yellow-200"
              >
                <TooltipProvider>
                  <Tooltip>
                    <TooltipTrigger>
                      <Crown className="h-6 w-6 text-gray-400" />
                    </TooltipTrigger>
                    <TooltipContent className="hidden bg-white p-2 shadow-lg md:block">
                      <p className="text-sm">Hacer Líder</p>
                    </TooltipContent>
                  </Tooltip>
                </TooltipProvider>
              </button>
            )}

            <div className="flex items-center gap-3">
              <div className={`relative ${isLeader(member.id) ? 'rounded-full ring-4 ring-yellow-200' : ''}`}>
                <Avatar className="h-14 w-14">
                  <AvatarImage src={getProfileImage(member.id) || undefined} alt={member.name} />
                  <AvatarFallback className="bg-gray-100 text-xl font-bold">
                    {member.name
                      .split(' ')
                      .map((word) => word[0])
                      .join('')
                      .toUpperCase()}
                  </AvatarFallback>
                </Avatar>
              </div>

              <div>
                <h3 className="text-lg font-semibold">{member.name}</h3>
                <p className="text-sm text-gray-600">
                  {member.role} · Nivel SFIA {member.sfia_Level || 'N/A'}
                </p>
              </div>
            </div>
            {isLeader(member.id) && showLeaderRationale === member.id && (
              <div className="mt-2 rounded-md bg-yellow-50 p-2 text-sm md:hidden">
                <p>
                  <strong>¿Por qué líder?</strong> {getCurrentLeaderRationale()}
                </p>
              </div>
            )}
          </div>
        ))}
      </div>

      {/* Team Analysis */}
      <div className="overflow-hidden rounded-lg bg-white shadow">
        <div className="border-b border-gray-200 p-4">
          <h3 className="text-lg font-medium">Análisis del Equipo</h3>
        </div>

        <div className="p-4">
          <div className="mb-6">
            <h4 className="mb-2 flex items-center gap-2 font-medium text-green-600">
              <CheckCircle className="h-5 w-5" /> Fortalezas
            </h4>
            <ul className="list-disc space-y-1 pl-6">
              {analysis.strengths.map((strength, index) => (
                <li key={index} className="text-gray-700">
                  {strength}
                </li>
              ))}
            </ul>
          </div>

          <div className="mb-6">
            <h4 className="mb-2 flex items-center gap-2 font-medium text-amber-600">
              <Info className="h-5 w-5" /> Debilidades
            </h4>
            <ul className="list-disc space-y-1 pl-6">
              {analysis.weaknesses.map((weakness, index) => (
                <li key={index} className="text-gray-700">
                  {weakness}
                </li>
              ))}
            </ul>
          </div>

          <div>
            <h4 className="mb-2 font-medium text-blue-600">Justificación</h4>
            <p className="text-gray-700">{analysis.compatibility}</p>
          </div>
        </div>
      </div>

      {/* Modal para ver miembros recomendados */}
      <Dialog open={showRecommendedMembers} onOpenChange={setShowRecommendedMembers}>
        <DialogContent className="max-h-[80vh] max-w-4xl overflow-y-auto">
          <DialogHeader>
            <DialogTitle className="text-xl">Miembros Recomendados</DialogTitle>
            <DialogDescription className="text-base">
              Estos son miembros adicionales que podrían encajar bien en el equipo.
            </DialogDescription>
          </DialogHeader>

          <div className="mt-4 space-y-4">
            {filteredRecommendedMembers.length > 0 ? (
              filteredRecommendedMembers.map((member) => (
                <div key={member.id} className="rounded-lg border border-gray-200 p-4">
                  <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
                    <div className="flex items-center gap-3">
                      <div className="flex h-12 w-12 items-center justify-center rounded-full bg-gray-100 text-lg font-bold">
                        {member.name
                          .split(' ')
                          .map((word) => word[0])
                          .join('')}
                      </div>
                      <div>
                        <h4 className="font-medium">{member.name}</h4>
                        <div className="flex items-center gap-1">
                          <span className="text-sm text-gray-600">Compatibilidad:</span>
                          <span className="text-sm font-medium text-blue-600">{member.compatibility_Score}%</span>
                        </div>
                      </div>
                    </div>

                    <Tabs defaultValue="info" className="w-full md:w-3/5">
                      <TabsList className="grid w-full grid-cols-2">
                        <TabsTrigger value="info">Detalles</TabsTrigger>
                        <TabsTrigger value="swap">Intercambiar</TabsTrigger>
                      </TabsList>
                      <TabsContent value="info" className="rounded bg-gray-50 p-3">
                        <div className="space-y-3 text-sm">
                          <p>
                            <span className="font-medium">Análisis:</span> {member.analysis}
                          </p>
                          <div>
                            <p className="font-medium">Posibles conflictos:</p>
                            <ul className="list-disc pl-5">
                              {member.potential_Conflicts.map((conflict, idx) => (
                                <li key={idx}>{conflict}</li>
                              ))}
                            </ul>
                          </div>
                          <p>
                            <span className="font-medium">Impacto:</span> {member.team_Impact}
                          </p>
                        </div>
                      </TabsContent>
                      <TabsContent value="swap" className="rounded bg-gray-50 p-3">
                        <p className="mb-2 text-sm text-gray-600">Intercambiar por:</p>
                        <div className="grid grid-cols-1 gap-2 md:grid-cols-2">
                          {currentTeamMembers.map((teamMember) => (
                            <Button
                              key={teamMember.id}
                              variant="outline"
                              size="sm"
                              className="w-full justify-start text-left"
                              onClick={() => handleSwapMember(teamMember.id, member.id)}
                              disabled={teamMember.id === currentLeader}
                            >
                              <User className="mr-2 h-4 w-4" />
                              {teamMember.name}
                              {teamMember.id === currentLeader && <Crown className="ml-1 h-4 w-4 text-yellow-500" />}
                            </Button>
                          ))}
                        </div>
                      </TabsContent>
                    </Tabs>
                  </div>
                </div>
              ))
            ) : (
              <Alert variant="default">
                <AlertCircle className="h-4 w-4" />
                <AlertDescription>
                  No hay miembros adicionales recomendados para este equipo o ya han sido agregados.
                </AlertDescription>
              </Alert>
            )}
          </div>
        </DialogContent>
      </Dialog>

      <Dialog open={!!showMemberDetails} onOpenChange={() => setShowMemberDetails(null)}>
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>Detalles del Miembro</DialogTitle>
          </DialogHeader>
          {showMemberDetails && <div className="space-y-4">{/* Contenido del detalle del miembro aquí */}</div>}
        </DialogContent>
      </Dialog>
    </div>
  );
};
