'use client';

import { useEffect, useState } from 'react';

import { useRouter } from 'next/navigation';

import { DashboardLayout } from '@/components/templates/DashboardLayout';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { Team, useTeams } from '@/hooks/useTeams';

import { Loader2, Users2 } from 'lucide-react';
import { toast } from 'sonner';

import { TeamCard } from '../../components/organisms/teams/TeamCard';

export default function TeamsListPage() {
  const router = useRouter();
  const { getAllTeams, deleteTeam, loading, error } = useTeams();
  const [teams, setTeams] = useState<Team[]>([]);
  const [initialLoad, setInitialLoad] = useState(true);

  useEffect(() => {
    const abortController = new AbortController();
    let isMounted = true;

    const fetchData = async () => {
      setInitialLoad(true);
      try {
        const data = await getAllTeams(abortController);
        if (isMounted) setTeams(data);
      } catch (error) {
        toast.error(`Error al cargar los equipos: ${error instanceof Error ? error.message : 'Desconocido'}`);
      } finally {
        if (isMounted) setInitialLoad(false);
      }
    };

    fetchData();

    return () => {
      isMounted = false;
      abortController.abort();
    };
  }, [getAllTeams]);

  const handleDeleteTeam = async (teamId: string) => {
    const success = await deleteTeam(teamId);
    if (success) {
      setTeams((currentTeams) => currentTeams.filter((team) => team.teamId !== teamId));
    } else {
      toast.error('Error eliminando el equipo');
    }
  };

  const handleCreateTeam = () => {
    router.push('/team-builder');
  };

  if (initialLoad) {
    return (
      <div className="flex flex-col items-center justify-center p-12">
        <Loader2 className="mb-4 h-12 w-12 animate-spin text-blue-500" />
        <p className="text-gray-600">Cargando equipos...</p>
      </div>
    );
  }

  if (loading && teams.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center p-12">
        <Loader2 className="mb-4 h-12 w-12 animate-spin text-blue-500" />
        <p className="text-gray-600">Cargando equipos...</p>
      </div>
    );
  }

  return (
    <DashboardLayout>
      <div className="px-4 py-6 sm:px-6 lg:px-8">
        <div className="mb-8 flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Mis Equipos</h1>
            <p className="mt-1 text-sm text-gray-500">
              Gestiona tus equipos generados con IA y visualiza su rendimiento.
            </p>
          </div>
        </div>

        {error && (
          <Alert variant="destructive" className="mb-6">
            <AlertDescription>Ocurrió un error al cargar los equipos: {error}</AlertDescription>
          </Alert>
        )}

        {teams.length === 0 && !loading ? (
          <div className="flex flex-col items-center justify-center py-16 text-center">
            <div className="mb-4 rounded-full bg-gray-100 p-6">
              <Users2 className="h-12 w-12 text-gray-400" />
            </div>
            <h2 className="mb-2 text-xl font-semibold text-gray-900">Aún no has creado equipos</h2>
            <p className="mb-6 max-w-md text-gray-500">
              Crea tu primer equipo optimizado con IA y visualiza su composición y compatibilidad.
            </p>
            <Button onClick={handleCreateTeam}>Crear mi primer equipo</Button>
          </div>
        ) : (
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {teams.map((team) => (
              <TeamCard key={team.teamId} team={team} onDelete={() => handleDeleteTeam(team.teamId)} />
            ))}
          </div>
        )}
      </div>
    </DashboardLayout>
  );
}
