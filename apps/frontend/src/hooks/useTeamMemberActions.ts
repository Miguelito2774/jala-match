import { useCallback, useState } from 'react';

import { AvailableTeamForMove, Team, TeamMember, useTeams } from '@/hooks/useTeams';

import { toast } from 'sonner';

export interface MemberActionDialogState {
  isOpen: boolean;
  type: 'remove' | 'move' | null;
  member: TeamMember | null;
  sourceTeam: Team | null;
}

export const useTeamMemberActions = (currentTeam: Team | null) => {
  const { removeTeamMember, moveTeamMember, getAvailableTeamsForMember, loading } = useTeams();

  const [dialogState, setDialogState] = useState<MemberActionDialogState>({
    isOpen: false,
    type: null,
    member: null,
    sourceTeam: null,
  });

  const [availableTeams, setAvailableTeams] = useState<AvailableTeamForMove[]>([]);

  const openRemoveDialog = useCallback(
    (member: TeamMember) => {
      setDialogState({
        isOpen: true,
        type: 'remove',
        member,
        sourceTeam: currentTeam,
      });
    },
    [currentTeam],
  );

  const openMoveDialog = useCallback(
    async (member: TeamMember) => {
      if (!currentTeam) {
        toast.error('No se ha seleccionado un equipo');
        return;
      }

      try {
        const teams = await getAvailableTeamsForMember(member.employeeProfileId, currentTeam.teamId);
        setAvailableTeams(teams);

        setDialogState({
          isOpen: true,
          type: 'move',
          member,
          sourceTeam: currentTeam,
        });
      } catch (_) {
        toast.error('Error al obtener equipos disponibles');
      }
    },
    [currentTeam, getAvailableTeamsForMember],
  );

  const closeDialog = useCallback(() => {
    setDialogState({
      isOpen: false,
      type: null,
      member: null,
      sourceTeam: null,
    });
    setAvailableTeams([]);
  }, []);

  const handleRemoveMember = useCallback(async (): Promise<boolean> => {
    if (!dialogState.member || !dialogState.sourceTeam) {
      toast.error('Información de miembro o equipo no disponible');
      return false;
    }

    try {
      const success = await removeTeamMember(dialogState.sourceTeam.teamId, dialogState.member.employeeProfileId);

      if (success) {
        toast.success(`${dialogState.member.name} ha sido eliminado del equipo`);
        closeDialog();
        return true;
      } else {
        toast.error('No se pudo eliminar el miembro del equipo');
        return false;
      }
    } catch (_) {
      toast.error('Error al eliminar miembro del equipo');
      return false;
    }
  }, [dialogState, removeTeamMember, closeDialog]);

  const handleMoveMember = useCallback(
    async (targetTeamId: string): Promise<boolean> => {
      if (!dialogState.member || !dialogState.sourceTeam) {
        toast.error('Información de miembro o equipo no disponible');
        return false;
      }

      try {
        const request = {
          sourceTeamId: dialogState.sourceTeam.teamId,
          targetTeamId,
          employeeProfileId: dialogState.member.employeeProfileId,
        };

        const result = await moveTeamMember(request);

        if (result) {
          const targetTeam = availableTeams.find((t) => t.teamId === targetTeamId);
          toast.success(`${dialogState.member.name} ha sido movido a ${targetTeam?.name || 'el equipo seleccionado'}`);
          closeDialog();
          return true;
        } else {
          toast.error('No se pudo mover el miembro al equipo seleccionado');
          return false;
        }
      } catch (_) {
        toast.error('Error al mover miembro entre equipos');
        return false;
      }
    },
    [dialogState, availableTeams, moveTeamMember, closeDialog],
  );

  const getAvailableTeamsForMove = useCallback((): AvailableTeamForMove[] => {
    return availableTeams;
  }, [availableTeams]);

  return {
    dialogState,
    loading,
    openRemoveDialog,
    openMoveDialog,
    closeDialog,
    handleRemoveMember,
    handleMoveMember,
    getAvailableTeamsForMove,
  };
};
