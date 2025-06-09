'use client';

import { EmployeeTeamCard } from '@/components/organisms/cards/EmployeeTeamCard';
import { DashboardLayout } from '@/components/templates/DashboardLayout';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { useEmployeeTeams } from '@/hooks/useEmployeeTeams';

import { AlertCircle, RefreshCw, Users } from 'lucide-react';

export default function MyTeamsPage() {
  const { teams, loading, error, refetch } = useEmployeeTeams();

  // Loading state
  if (loading) {
    return (
      <DashboardLayout>
        <div className="space-y-6">
          <div className="space-y-2">
            <h1 className="text-3xl font-bold tracking-tight">Mis Equipos</h1>
            <p className="text-muted-foreground">Equipos en los que participas como miembro</p>
          </div>

          <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
            {Array.from({ length: 3 }).map((_, index) => (
              <Card key={index}>
                <CardHeader>
                  <Skeleton className="h-6 w-3/4" />
                  <Skeleton className="h-4 w-1/2" />
                </CardHeader>
                <CardContent className="space-y-4">
                  <Skeleton className="h-4 w-full" />
                  <div className="flex gap-2">
                    <Skeleton className="h-6 w-16" />
                    <Skeleton className="h-6 w-20" />
                  </div>
                  <div className="flex -space-x-2">
                    {Array.from({ length: 4 }).map((_, i) => (
                      <Skeleton key={i} className="h-8 w-8 rounded-full" />
                    ))}
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </DashboardLayout>
    );
  }

  // Error state
  if (error) {
    return (
      <DashboardLayout>
        <div className="space-y-6">
          <div className="space-y-2">
            <h1 className="text-3xl font-bold tracking-tight">Mis Equipos</h1>
            <p className="text-muted-foreground">Equipos en los que participas como miembro</p>
          </div>

          <Alert variant="destructive">
            <AlertCircle className="h-4 w-4" />
            <AlertDescription className="flex items-center justify-between">
              <span>Error al cargar los equipos. {error}</span>
              <Button variant="outline" size="sm" onClick={() => refetch()} className="ml-2">
                <RefreshCw className="mr-2 h-4 w-4" />
                Reintentar
              </Button>
            </AlertDescription>
          </Alert>
        </div>
      </DashboardLayout>
    );
  }

  // Empty state
  if (!teams || teams.length === 0) {
    return (
      <DashboardLayout>
        <div className="space-y-6">
          <div className="space-y-2">
            <h1 className="text-3xl font-bold tracking-tight">Mis Equipos</h1>
            <p className="text-muted-foreground">Equipos en los que participas como miembro</p>
          </div>

          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Users className="h-5 w-5" />
                No estás en ningún equipo
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-muted-foreground">
                Actualmente no formas parte de ningún equipo. Tu manager puede agregarte a equipos según las necesidades
                del proyecto.
              </p>
            </CardContent>
          </Card>
        </div>
      </DashboardLayout>
    );
  }

  // Main content
  return (
    <DashboardLayout>
      <div className="space-y-6">
        <div className="space-y-2">
          <h1 className="text-3xl font-bold tracking-tight">Mis Equipos</h1>
          <p className="text-muted-foreground">
            Equipos en los que participas como miembro ({teams.length} {teams.length === 1 ? 'equipo' : 'equipos'})
          </p>
        </div>

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {teams.map((team) => (
            <EmployeeTeamCard key={team.teamId} team={team} />
          ))}
        </div>
      </div>
    </DashboardLayout>
  );
}
