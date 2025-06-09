'use client';

import { use } from 'react';

import { useRouter } from 'next/navigation';

import { TeamLeaderCard } from '@/components/organisms/cards/TeamLeaderCard';
import { TeamMembersGrid } from '@/components/organisms/cards/TeamMembersGrid';
import { TeamStatsCard } from '@/components/organisms/cards/TeamStatsCard';
import { DashboardLayout } from '@/components/templates/DashboardLayout';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { useAuth } from '@/contexts/AuthContext';
import { useTeamDetails } from '@/hooks/useTeamDetails';

import { AlertCircle, ArrowLeft, Calendar, RefreshCw, Shield, Users } from 'lucide-react';

interface TeamPageProps {
  params: Promise<{
    teamId: string;
  }>;
}

export default function TeamPage({ params }: TeamPageProps) {
  const router = useRouter();
  const { user } = useAuth();
  const resolvedParams = use(params);
  const { teamDetails, loading, error, refetch } = useTeamDetails(resolvedParams.teamId);

  // Loading state
  if (loading) {
    return (
      <DashboardLayout>
        <div className="space-y-6">
          {/* Header skeleton */}
          <div className="flex items-center gap-4">
            <Skeleton className="h-10 w-10 rounded" />
            <div className="space-y-2">
              <Skeleton className="h-8 w-64" />
              <Skeleton className="h-4 w-48" />
            </div>
          </div>

          {/* Content skeleton */}
          <div className="grid gap-6 lg:grid-cols-3">
            <div className="space-y-6 lg:col-span-2">
              <Card>
                <CardHeader>
                  <Skeleton className="h-6 w-32" />
                </CardHeader>
                <CardContent>
                  <div className="grid gap-4 md:grid-cols-2">
                    {Array.from({ length: 4 }).map((_, i) => (
                      <div key={i} className="space-y-3 rounded-lg border p-4">
                        <div className="flex items-center gap-3">
                          <Skeleton className="h-12 w-12 rounded-full" />
                          <div className="space-y-1">
                            <Skeleton className="h-4 w-32" />
                            <Skeleton className="h-3 w-24" />
                          </div>
                        </div>
                        <Skeleton className="h-2 w-full" />
                        <div className="flex gap-1">
                          <Skeleton className="h-5 w-16" />
                          <Skeleton className="h-5 w-20" />
                        </div>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>
            </div>

            <div className="space-y-6">
              <Card>
                <CardHeader>
                  <Skeleton className="h-6 w-32" />
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="flex items-center gap-4">
                    <Skeleton className="h-16 w-16 rounded-full" />
                    <div className="space-y-2">
                      <Skeleton className="h-5 w-32" />
                      <Skeleton className="h-4 w-24" />
                    </div>
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <Skeleton className="h-6 w-32" />
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="grid grid-cols-2 gap-4">
                    {Array.from({ length: 4 }).map((_, i) => (
                      <div key={i} className="rounded-lg border p-3">
                        <Skeleton className="mb-2 h-8 w-8" />
                        <Skeleton className="h-4 w-12" />
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>
            </div>
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
          <div className="flex items-center gap-4">
            <Button variant="outline" size="sm" onClick={() => router.back()} className="flex items-center gap-2">
              <ArrowLeft className="h-4 w-4" />
              Volver
            </Button>
            <div>
              <h1 className="text-3xl font-bold tracking-tight">Detalles del Equipo</h1>
              <p className="text-muted-foreground">Error al cargar la información del equipo</p>
            </div>
          </div>

          <Alert variant="destructive">
            <AlertCircle className="h-4 w-4" />
            <AlertDescription className="flex items-center justify-between">
              <span>Error al cargar el equipo. {error}</span>
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

  if (!teamDetails) {
    return (
      <DashboardLayout>
        <div className="space-y-6">
          <div className="flex items-center gap-4">
            <Button variant="outline" size="sm" onClick={() => router.back()} className="flex items-center gap-2">
              <ArrowLeft className="h-4 w-4" />
              Volver
            </Button>
            <div>
              <h1 className="text-3xl font-bold tracking-tight">Equipo no encontrado</h1>
              <p className="text-muted-foreground">El equipo solicitado no existe o no tienes acceso a él</p>
            </div>
          </div>
        </div>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex items-start justify-between">
          <div className="flex items-center gap-4">
            <Button variant="outline" size="sm" onClick={() => router.back()} className="flex items-center gap-2">
              <ArrowLeft className="h-4 w-4" />
              Volver
            </Button>
            <div>
              <div className="mb-2 flex items-center gap-3">
                <h1 className="text-3xl font-bold tracking-tight">{teamDetails.teamName}</h1>
                <div className="flex items-center gap-2">
                  {teamDetails.isActive ? (
                    <Badge variant="default" className="bg-green-100 text-green-800">
                      <Shield className="mr-1 h-3 w-3" />
                      Activo
                    </Badge>
                  ) : (
                    <Badge variant="secondary">Inactivo</Badge>
                  )}
                  {teamDetails.isCurrentUserLeader && (
                    <Badge variant="outline" className="border-yellow-300 text-yellow-700">
                      Eres el líder
                    </Badge>
                  )}
                </div>
              </div>
              <div className="text-muted-foreground flex items-center gap-4 text-sm">
                <div className="flex items-center gap-1">
                  <Users className="h-4 w-4" />
                  <span>{teamDetails.teammates.length} miembros</span>
                </div>
                <div className="flex items-center gap-1">
                  <Calendar className="h-4 w-4" />
                  <span>Creado por {teamDetails.creatorName}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Main Content */}
        <div className="grid gap-6 lg:grid-cols-3">
          {/* Left Column - Team Members */}
          <div className="space-y-6 lg:col-span-2">
            <TeamMembersGrid members={teamDetails.teammates} currentUserId={user?.id} />
          </div>

          {/* Right Column - Leader and Stats */}
          <div className="space-y-6">
            {/* Team Leader */}
            {teamDetails.teamLeader && <TeamLeaderCard leader={teamDetails.teamLeader} />}

            {/* Team Statistics */}
            <TeamStatsCard stats={teamDetails.teamStats} compatibilityScore={teamDetails.compatibilityScore} />
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
