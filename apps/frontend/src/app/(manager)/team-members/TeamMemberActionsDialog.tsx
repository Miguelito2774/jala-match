import React from 'react';

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
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { ScrollArea } from '@/components/ui/scroll-area';
import { AvailableTeamForMove } from '@/hooks/useTeams';

import { AlertTriangle, ArrowRight, Crown, Loader2, Users } from 'lucide-react';

interface TeamMemberActionsDialogProps {
  dialogState: import('@/hooks/useTeamMemberActions').MemberActionDialogState;
  loading: boolean;
  closeDialog: () => void;
  handleRemoveMember: () => Promise<boolean>;
  handleMoveMember: (targetTeamId: string) => Promise<boolean>;
  getAvailableTeamsForMove: () => AvailableTeamForMove[];
  onMemberRemoved?: () => void;
  onMemberMoved?: () => void;
}

export const TeamMemberActionsDialog: React.FC<TeamMemberActionsDialogProps> = ({
  dialogState,
  loading,
  closeDialog,
  handleRemoveMember,
  handleMoveMember,
  getAvailableTeamsForMove,
  onMemberRemoved,
  onMemberMoved,
}) => {
  const getInitials = (name: string) =>
    name
      .split(' ')
      .map((part) => part[0])
      .join('')
      .toUpperCase();

  const onRemoveConfirm = async () => {
    const success = await handleRemoveMember();
    if (success && onMemberRemoved) onMemberRemoved();
  };

  const onMoveConfirm = async (targetTeamId: string) => {
    const success = await handleMoveMember(targetTeamId);
    if (success && onMemberMoved) onMemberMoved();
  };

  // Diálogo para eliminar miembro
  if (dialogState.type === 'remove') {
    return (
      <AlertDialog open={dialogState.isOpen} onOpenChange={(open) => (!open ? closeDialog() : undefined)}>
        <AlertDialogContent className="fixed top-1/2 left-1/2 z-50 flex w-full max-w-md -translate-x-1/2 -translate-y-1/2 flex-col gap-6 rounded-lg bg-white p-6 shadow-lg">
          <AlertDialogHeader className="text-center">
            <div className="mx-auto mb-2 flex h-10 w-10 items-center justify-center rounded-full bg-red-100">
              <AlertTriangle className="h-5 w-5 text-red-600" />
            </div>
            <AlertDialogTitle className="text-lg">Eliminar miembro del equipo</AlertDialogTitle>
            <AlertDialogDescription className="text-muted-foreground text-center text-sm">
              ¿Estás seguro de que deseas eliminar a <span className="font-semibold">{dialogState.member?.name}</span>{' '}
              del equipo <span className="font-semibold">{dialogState.sourceTeam?.name}</span>? Esta acción no se puede
              deshacer.
            </AlertDialogDescription>
          </AlertDialogHeader>

          <AlertDialogFooter className="mt-2 justify-center gap-4">
            <AlertDialogCancel disabled={loading} className="px-6 py-2">
              Cancelar
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={onRemoveConfirm}
              disabled={loading}
              className="bg-red-600 px-6 py-2 hover:bg-red-700 focus:ring-red-600"
            >
              {loading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Eliminando...
                </>
              ) : (
                'Eliminar'
              )}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    );
  }

  // Diálogo para mover miembro
  if (dialogState.type === 'move') {
    const availableTeams = getAvailableTeamsForMove();

    return (
      <Dialog open={dialogState.isOpen} onOpenChange={(open) => (!loading && !open ? closeDialog() : undefined)}>
        <DialogContent className="ml-[-1.5rem] max-w-[95vw] sm:max-w-2xl">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <ArrowRight className="h-5 w-5 text-blue-600" />
              Mover miembro a otro equipo
            </DialogTitle>
            <DialogDescription>
              Selecciona el equipo de destino para <span className="font-semibold">{dialogState.member?.name}</span>
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4">
            {/* Información del miembro actual */}
            <Card className="border-blue-100 bg-blue-50">
              <CardContent className="p-4">
                <div className="flex items-center space-x-3">
                  <div className="relative">
                    <Avatar className="h-12 w-12">
                      <AvatarFallback className="bg-blue-100 text-blue-700">
                        {dialogState.member && getInitials(dialogState.member.name)}
                      </AvatarFallback>
                    </Avatar>
                    {dialogState.member?.isLeader && (
                      <div className="absolute -top-2 -right-2 rounded-full bg-yellow-400 p-1">
                        <Crown className="h-3 w-3 text-white" />
                      </div>
                    )}
                  </div>
                  <div className="flex-1">
                    <h3 className="font-semibold text-gray-900">{dialogState.member?.name}</h3>
                    <div className="mt-1 flex flex-wrap items-center gap-2">
                      <Badge variant="outline" className="text-xs">
                        {dialogState.member?.role}
                      </Badge>
                      <Badge variant="secondary" className="text-xs">
                        SFIA {dialogState.member?.sfiaLevel}
                      </Badge>
                      {dialogState.member?.isLeader && (
                        <Badge className="bg-yellow-100 text-xs text-yellow-800">Líder</Badge>
                      )}
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Selección de equipos */}
            <div>
              <h4 className="mb-3 text-sm font-medium text-gray-900">Equipos disponibles ({availableTeams.length})</h4>

              {loading ? (
                <div className="py-8 text-center">
                  <Loader2 className="mx-auto mb-2 h-8 w-8 animate-spin text-blue-500" />
                  <p className="text-gray-500">Cargando equipos disponibles...</p>
                </div>
              ) : availableTeams.length === 0 ? (
                <div className="py-8 text-center text-gray-500">
                  <Users className="mx-auto mb-2 h-8 w-8 text-gray-400" />
                  <p>No hay equipos disponibles para el traslado</p>
                  <p className="mt-1 text-xs text-gray-400">
                    El miembro puede estar ya en todos los otros equipos disponibles
                  </p>
                </div>
              ) : (
                <ScrollArea className="h-64">
                  <div className="space-y-2 pr-4">
                    {availableTeams.map((team) => (
                      <Card
                        key={team.teamId}
                        className={`cursor-pointer border-gray-200 transition-all hover:border-blue-200 hover:shadow-sm ${
                          loading ? 'pointer-events-none opacity-50' : ''
                        }`}
                        onClick={() => !loading && onMoveConfirm(team.teamId)}
                      >
                        <CardContent className="p-4">
                          <div className="flex items-center justify-between">
                            <div className="flex-1">
                              <h3 className="mb-1 font-semibold text-gray-900">{team.name}</h3>
                              <div className="flex items-center gap-4 text-sm text-gray-600">
                                <span className="flex items-center gap-1">
                                  <Users className="h-4 w-4" />
                                  {team.currentMemberCount} miembros
                                </span>
                              </div>
                              <div className="mt-2">
                                <Badge variant={team.hasMember ? 'destructive' : 'default'} className="text-xs">
                                  {team.hasMember ? 'Ya es miembro' : 'Disponible'}
                                </Badge>
                              </div>
                            </div>
                            {loading ? (
                              <Loader2 className="ml-4 h-5 w-5 animate-spin text-blue-600" />
                            ) : (
                              <ArrowRight className="ml-4 h-5 w-5 text-gray-400" />
                            )}
                          </div>
                        </CardContent>
                      </Card>
                    ))}
                  </div>
                </ScrollArea>
              )}
            </div>
          </div>

          <DialogFooter>
            <Button variant="outline" onClick={closeDialog} disabled={loading}>
              Cancelar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    );
  }

  return null;
};
