'use client';

import { useEffect, useState } from 'react';

import Link from 'next/link';

import { DashboardLayout } from '@/components/templates';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { TeamWithReviewStatus, usePsychologistReview } from '@/hooks/usePsychologistReview';

import { AlertCircle, AlertTriangle, Brain, CheckCircle2 } from 'lucide-react';
import { toast } from 'sonner';

export default function PsychologistTeamsPage() {
  const { getPendingTeams, error } = usePsychologistReview();
  const [teams, setTeams] = useState<TeamWithReviewStatus[]>([]);
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  useEffect(() => {
    if (!mounted) return;

    const fetchTeams = async () => {
      try {
        const data = await getPendingTeams();
        setTeams(data);
      } catch (_err) {
        toast.error('Error al cargar equipos pendientes');
      }
    };

    fetchTeams();
  }, [mounted, getPendingTeams]);

  if (!mounted) {
    return (
      <DashboardLayout>
        <div className="flex min-h-[60vh] items-center justify-center">
          <div className="text-center">
            <div className="mx-auto mb-4 h-12 w-12 animate-spin rounded-full border-b-2 border-purple-600"></div>
            <p className="text-gray-600">Cargando...</p>
          </div>
        </div>
      </DashboardLayout>
    );
  }

  if (error) {
    return (
      <DashboardLayout>
        <div className="container max-w-5xl px-4 py-6 sm:px-6 lg:px-8">
          <div className="rounded-lg border border-red-200 bg-red-50 p-6">
            <div className="flex items-center gap-3">
              <AlertCircle className="h-6 w-6 text-red-600" />
              <div>
                <h2 className="font-semibold text-red-900">Error</h2>
                <p className="mt-1 text-sm text-red-800">{error}</p>
              </div>
            </div>
            <Button onClick={() => window.location.reload()} variant="outline" className="mt-4">
              Reintentar
            </Button>
          </div>
        </div>
      </DashboardLayout>
    );
  }

  const pendingTeams = teams.filter((t) => !t.latestReview);
  const reviewedTeams = teams.filter((t) => t.latestReview);

  return (
    <DashboardLayout>
      <div className="container max-w-5xl px-4 py-6 sm:px-6 lg:px-8">
        <div className="mb-8">
          <h1 className="flex items-center gap-3 text-3xl font-bold text-gray-900">
            <Brain className="h-8 w-8 text-purple-600" />
            Revisiones Psicológicas
          </h1>
          <p className="mt-2 text-gray-600">Revisa y evalúa los equipos que requieren una evaluación psicológica</p>
        </div>

        {/* Pending Teams Section */}
        <div className="mb-8">
          <div className="mb-4 flex items-center gap-2">
            <AlertTriangle className="h-5 w-5 text-orange-500" />
            <h2 className="text-xl font-semibold text-gray-900">Pendientes de Revisión ({pendingTeams.length})</h2>
          </div>

          {pendingTeams.length === 0 ? (
            <Card className="border-dashed">
              <CardContent className="pt-6">
                <div className="py-8 text-center">
                  <Brain className="mx-auto mb-3 h-12 w-12 text-gray-300" />
                  <p className="text-gray-500">No hay equipos pendientes de revisión</p>
                </div>
              </CardContent>
            </Card>
          ) : (
            <div className="grid gap-4">
              {pendingTeams.map((teamStatus) => (
                <Link key={teamStatus.team.teamId} href={`/psychologist/teams/${teamStatus.team.teamId}`}>
                  <Card className="cursor-pointer transition-all hover:border-purple-300 hover:shadow-md">
                    <CardContent className="pt-6">
                      <div className="flex items-start justify-between">
                        <div className="flex-1">
                          <h3 className="text-lg font-semibold text-gray-900">{teamStatus.team.name}</h3>
                          <p className="mt-1 text-sm text-gray-600">
                            {teamStatus.team.members.length} miembros • Compatibilidad:{' '}
                            <span className="font-semibold text-purple-600">
                              {Math.round(teamStatus.team.compatibilityScore)}%
                            </span>
                          </p>
                        </div>
                        <Button asChild variant="default" size="sm" className="bg-purple-600 hover:bg-purple-700">
                          <span>Revisar Equipo</span>
                        </Button>
                      </div>
                    </CardContent>
                  </Card>
                </Link>
              ))}
            </div>
          )}
        </div>

        {/* Reviewed Teams Section */}
        {reviewedTeams.length > 0 && (
          <div>
            <div className="mb-4 flex items-center gap-2">
              <CheckCircle2 className="h-5 w-5 text-green-500" />
              <h2 className="text-xl font-semibold text-gray-900">Revisiones Completadas ({reviewedTeams.length})</h2>
            </div>

            <div className="grid gap-4">
              {reviewedTeams.map((teamStatus) => (
                <Card key={teamStatus.team.teamId} className="opacity-75">
                  <CardContent className="pt-6">
                    <div className="flex items-start justify-between">
                      <div className="flex-1">
                        <h3 className="text-lg font-semibold text-gray-900">{teamStatus.team.name}</h3>
                        <p className="mt-1 text-sm text-gray-600">
                          {teamStatus.team.members.length} miembros • Estado:{' '}
                          <span className="font-semibold">
                            {teamStatus.latestReview?.status === 'Approved' ? '✅ Aprobado' : '⚠️ Flagged'}
                          </span>
                        </p>
                        {teamStatus.latestReview?.comment && (
                          <p className="mt-2 text-sm text-gray-600 italic">
                            &quot;{teamStatus.latestReview.comment}&quot;
                          </p>
                        )}
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          </div>
        )}
      </div>
    </DashboardLayout>
  );
}
