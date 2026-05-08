'use client';

import * as React from 'react';
import { useEffect, useState } from 'react';

import { useRouter } from 'next/navigation';

import { TextArea } from '@/components/atoms/inputs/TextArea';
import { DashboardLayout } from '@/components/templates/DashboardLayout';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import { SubmitReviewRequest, usePsychologistReview } from '@/hooks/usePsychologistReview';
import { Team, TeamMember, useTeams } from '@/hooks/useTeams';

import { AlertCircle, ArrowLeft, Brain, CheckCircle, Send, ShieldAlert } from 'lucide-react';
import { toast } from 'sonner';

interface TeamPageProps {
  params: Promise<{
    teamId: string;
  }>;
}

interface TeamAnalysisExtended {
  reasoning?: string;
  potentialRisks?: string[];
  [key: string]: unknown;
}

export default function PsychologistTeamPage({ params }: TeamPageProps) {
  const router = useRouter();
  const { getTeamById, loading: teamLoading, error: teamError } = useTeams();
  const { submitReview, loading: reviewLoading, error: reviewError } = usePsychologistReview();
  const [team, setTeam] = useState<Team | null>(null);
  const [comment, setComment] = useState('');
  const [status, setStatus] = useState<'Approved' | 'FlaggedConcerns'>('Approved');

  const unwrappedParams = React.use(params);
  const { teamId } = unwrappedParams;

  useEffect(() => {
    const fetchTeam = async () => {
      try {
        const data = await getTeamById(teamId);
        setTeam(data);
      } catch (err) {
        toast.error('Error al cargar el equipo');
      }
    };
    fetchTeam();
  }, [getTeamById, teamId]);

  const handleBack = () => router.push('/psychologist/teams');

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!comment.trim()) {
      toast.error('Por favor ingresa un comentario o justificación');
      return;
    }

    const payload: SubmitReviewRequest = { comment, status };
    const success = await submitReview(teamId, payload);

    if (success) {
      toast.success('Revisión enviada correctamente');
      router.push('/psychologist/teams');
    } else {
      toast.error('Error al enviar la revisión', {
        description: reviewError || 'Ocurrió un error inesperado',
      });
    }
  };

  const getInitials = (name: string) =>
    name
      .split(' ')
      .map((part) => part[0])
      .join('')
      .toUpperCase();

  if (teamLoading && !team) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center p-4">
        <div className="mb-4 h-12 w-12 animate-spin rounded-full border-b-2 border-purple-600"></div>
        <p className="text-gray-600">Cargando información del equipo...</p>
      </div>
    );
  }

  if (teamError || !team) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center p-4">
        <AlertCircle className="mb-4 h-12 w-12 text-red-500" />
        <h2 className="mb-2 text-xl font-semibold text-gray-900">Error al cargar el equipo</h2>
        <Button onClick={handleBack} className="mt-4 bg-purple-600 hover:bg-purple-700">
          Volver
        </Button>
      </div>
    );
  }

  const analysis = team.analysis as unknown as TeamAnalysisExtended | undefined;

  return (
    <DashboardLayout>
      <div className="container max-w-5xl px-4 py-6 sm:px-6 lg:px-8">
        <div className="mb-6">
          <Button variant="outline" size="sm" className="flex items-center gap-1" onClick={handleBack}>
            <ArrowLeft className="h-4 w-4" />
            Volver
          </Button>
        </div>

        <div className="grid gap-6 lg:grid-cols-3">
          <div className="space-y-6 lg:col-span-2">
            <Card>
              <CardHeader className="bg-purple-50">
                <CardTitle className="flex items-center gap-2 text-2xl">
                  <Brain className="h-6 w-6 text-purple-600" />
                  {team.name}
                </CardTitle>
                <CardDescription>
                  Revisión Psicológica - Compatibilidad generada por IA: {Math.round(team.compatibilityScore)}%
                </CardDescription>
              </CardHeader>
              <CardContent className="pt-6">
                <h3 className="mb-4 text-lg font-semibold text-gray-900">
                  Miembros del equipo ({team.members.length})
                </h3>
                <div className="grid gap-4 sm:grid-cols-2">
                  {team.members.map((member: TeamMember) => (
                    <div key={member.employeeProfileId} className="flex items-center gap-3 rounded-lg border p-3">
                      <Avatar className="h-10 w-10">
                        <AvatarImage src={member.profilePictureUrl || undefined} alt={member.name} />
                        <AvatarFallback>{getInitials(member.name)}</AvatarFallback>
                      </Avatar>
                      <div>
                        <p className="text-sm leading-none font-medium text-gray-900">{member.name}</p>
                        <p className="mt-1 text-xs text-gray-500">
                          {member.role} • Nivel {member.sfiaLevel}
                        </p>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>

            {analysis && (
              <Card>
                <CardHeader>
                  <CardTitle className="text-lg">Análisis de IA</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4 text-sm text-gray-700">
                    {analysis.reasoning && (
                      <div>
                        <h4 className="font-semibold text-gray-900">Razonamiento</h4>
                        <p className="mt-1">{analysis.reasoning}</p>
                      </div>
                    )}
                    {analysis.potentialRisks && analysis.potentialRisks.length > 0 && (
                      <div>
                        <h4 className="font-semibold text-gray-900">Riesgos Potenciales</h4>
                        <ul className="mt-1 list-disc pl-5">
                          {analysis.potentialRisks.map((risk: string, idx: number) => (
                            <li key={idx}>{risk}</li>
                          ))}
                        </ul>
                      </div>
                    )}
                  </div>
                </CardContent>
              </Card>
            )}
          </div>

          <div>
            <Card className="sticky top-6">
              <CardHeader>
                <CardTitle className="text-xl">Evaluación</CardTitle>
                <CardDescription>Emite tu dictamen psicológico para este equipo.</CardDescription>
              </CardHeader>
              <CardContent>
                <form onSubmit={handleSubmit} className="space-y-6">
                  <div className="space-y-3">
                    <Label>Estado de Evaluación</Label>
                    <RadioGroup
                      value={status}
                      onValueChange={(val: string) => setStatus(val as 'Approved' | 'FlaggedConcerns')}
                      className="grid gap-3"
                    >
                      <div>
                        <RadioGroupItem value="Approved" id="approved" className="peer sr-only" />
                        <Label
                          htmlFor="approved"
                          className="border-muted bg-popover hover:bg-accent hover:text-accent-foreground flex cursor-pointer items-center justify-between rounded-md border-2 p-4 peer-data-[state=checked]:border-green-500 peer-data-[state=checked]:bg-green-50 [&:has([data-state=checked])]:border-green-500"
                        >
                          <div className="flex items-center gap-2">
                            <CheckCircle className="h-5 w-5 text-green-600" />
                            Aprobado
                          </div>
                        </Label>
                      </div>
                      <div>
                        <RadioGroupItem value="FlaggedConcerns" id="flagged" className="peer sr-only" />
                        <Label
                          htmlFor="flagged"
                          className="border-muted bg-popover hover:bg-accent hover:text-accent-foreground flex cursor-pointer items-center justify-between rounded-md border-2 p-4 peer-data-[state=checked]:border-red-500 peer-data-[state=checked]:bg-red-50 [&:has([data-state=checked])]:border-red-500"
                        >
                          <div className="flex items-center gap-2">
                            <ShieldAlert className="h-5 w-5 text-red-600" />
                            Con Observaciones
                          </div>
                        </Label>
                      </div>
                    </RadioGroup>
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="comment">Justificación / Comentarios</Label>
                    <TextArea
                      id="comment"
                      placeholder="Escribe tus observaciones psicológicas sobre la compatibilidad del equipo..."
                      className="min-h-[150px] resize-none"
                      value={comment}
                      onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) => setComment(e.target.value)}
                    />
                  </div>

                  <Button type="submit" className="w-full bg-purple-600 hover:bg-purple-700" disabled={reviewLoading}>
                    {reviewLoading ? (
                      <span className="flex items-center gap-2">
                        <div className="h-4 w-4 animate-spin rounded-full border-b-2 border-white"></div>
                        Enviando...
                      </span>
                    ) : (
                      <span className="flex items-center gap-2">
                        <Send className="h-4 w-4" />
                        Enviar Evaluación
                      </span>
                    )}
                  </Button>
                </form>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
