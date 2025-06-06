'use client';

import { useEffect, useState } from 'react';

import { useParams, useRouter } from 'next/navigation';

import { TextArea } from '@/components/atoms/inputs/TextArea';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Separator } from '@/components/ui/separator';
import { Skeleton } from '@/components/ui/skeleton';
import { useAuth } from '@/contexts/AuthContext';
import { useProfileVerifications, type ProfileForVerificationDto } from '@/hooks/useProfileVerifications';

import { ArrowLeft, Briefcase, Calendar, CheckCircle, Clock, Code, MapPin, Star, User, XCircle } from 'lucide-react';
import { toast } from 'sonner';

const getLevelLabel = (level: number): string => {
  const levelMap: Record<number, string> = {
    0: 'Junior',
    1: 'Staff',
    2: 'Senior',
    3: 'Architect',
  };
  return levelMap[level] || 'Sin nivel';
};

export default function ProfileVerificationDetailPage() {
  const params = useParams();
  const router = useRouter();
  const { user } = useAuth();
  const employeeProfileId = params.employeeProfileId as string;

  const [notes, setNotes] = useState('');
  const [sfiaProposed, setSfiaProposed] = useState<number | undefined>();
  const [isProcessing, setIsProcessing] = useState(false);
  const [profile, setProfile] = useState<ProfileForVerificationDto | null>(null);

  const { loading, error, getProfileForVerification, approveProfile, rejectProfile } = useProfileVerifications();

  useEffect(() => {
    const loadProfile = async () => {
      if (!employeeProfileId || !user) return;

      try {
        const profileData = await getProfileForVerification(employeeProfileId);
        setProfile(profileData);
      } catch (_err) {
        // Error is handled by the hook
      }
    };

    if (user?.role === 'Manager') {
      loadProfile();
    }
  }, [employeeProfileId, user, getProfileForVerification]);

  const isLoading = loading;

  if (!user || user.role !== 'Manager') {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-gray-900">Acceso denegado</h1>
          <p className="mt-2 text-gray-600">Solo los managers pueden acceder a esta página.</p>
        </div>
      </div>
    );
  }

  const handleApprove = async () => {
    if (!profile) return;

    setIsProcessing(true);
    try {
      await approveProfile(employeeProfileId, {
        employeeProfileId,
        sfiaProposed,
        notes: notes.trim() || undefined,
      });

      toast.success('Perfil aprobado exitosamente');
      router.push('/profile-verifications');
    } catch (_error) {
      toast.error('Error al aprobar el perfil');
    } finally {
      setIsProcessing(false);
    }
  };

  const handleReject = async () => {
    if (!profile || !notes.trim()) {
      toast.error('Las notas son requeridas para rechazar un perfil');
      return;
    }

    setIsProcessing(true);
    try {
      await rejectProfile(employeeProfileId, {
        employeeProfileId,
        notes: notes.trim(),
      });

      toast.success('Perfil rechazado exitosamente');
      router.push('/profile-verifications');
    } catch (_error) {
      toast.error('Error al rechazar el perfil');
    } finally {
      setIsProcessing(false);
    }
  };

  if (isLoading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <Skeleton className="mb-8 h-8 w-48" />
        <div className="grid gap-6">
          {Array.from({ length: 4 }).map((_, i) => (
            <Card key={i}>
              <CardHeader>
                <Skeleton className="h-6 w-48" />
                <Skeleton className="h-4 w-32" />
              </CardHeader>
              <CardContent>
                <Skeleton className="h-20 w-full" />
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    );
  }

  if (error || !profile) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-red-600">Error</h1>
          <p className="mt-2 text-gray-600">No se pudo cargar el perfil para verificación.</p>
          <Button onClick={() => router.push('/profile-verifications')} className="mt-4">
            Volver a la lista
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      {/* Header */}
      <div className="mb-8">
        <Button variant="ghost" onClick={() => router.push('/profile-verifications')} className="mb-4">
          <ArrowLeft className="mr-2 h-4 w-4" />
          Volver a la lista
        </Button>

        <div className="flex items-start justify-between">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">
              {profile.firstName} {profile.lastName}
            </h1>
            <p className="mt-1 text-gray-600">{profile.email}</p>
            <div className="mt-2 flex items-center gap-4">
              <Badge variant="outline" className="flex items-center gap-2">
                <Calendar className="h-4 w-4" />
                Solicitado: {new Date(profile.requestedAt).toLocaleDateString('es-ES')}
              </Badge>
            </div>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-8 lg:grid-cols-3">
        {/* Contenido principal */}
        <div className="space-y-6 lg:col-span-2">
          {/* Información General */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <User className="h-5 w-5" />
                Información General
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                <div>
                  <Label className="text-sm font-medium text-gray-500">País</Label>
                  <p className="mt-1 flex items-center gap-2">
                    <MapPin className="h-4 w-4 text-gray-500" />
                    {profile.country}
                  </p>
                </div>
                <div>
                  <Label className="text-sm font-medium text-gray-500">Zona Horaria</Label>
                  <p className="mt-1 flex items-center gap-2">
                    <Clock className="h-4 w-4 text-gray-500" />
                    {profile.timezone}
                  </p>
                </div>
                <div>
                  <Label className="text-sm font-medium text-gray-500">Nivel SFIA General</Label>
                  <Badge variant="outline" className="mt-1">
                    SFIA {profile.sfiaLevelGeneral}
                  </Badge>
                </div>
                <div>
                  <Label className="text-sm font-medium text-gray-500">MBTI</Label>
                  <Badge variant="secondary" className="mt-1">
                    {profile.mbti}
                  </Badge>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Roles Especializados */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Star className="h-5 w-5" />
                Rol Especializado
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {profile.specializedRoles.map(
                  (
                    role: { roleName: string; level: number; technicalAreaName: string; yearsExperience: number },
                    index: number,
                  ) => (
                    <div key={index} className="rounded-lg border p-4">
                      <div className="mb-2 flex items-center justify-between">
                        <h4 className="font-medium">{role.roleName}</h4>
                        <Badge variant="outline">{getLevelLabel(role.level)}</Badge>
                      </div>
                      <p className="mb-2 text-sm text-gray-600">{role.technicalAreaName}</p>
                      <p className="text-sm">
                        <strong>Experiencia:</strong> {role.yearsExperience} años
                      </p>
                    </div>
                  ),
                )}
              </div>
            </CardContent>
          </Card>

          {/* Experiencia Laboral */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Briefcase className="h-5 w-5" />
                Experiencia Laboral ({profile.totalProjects} proyectos)
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {profile.workExperiences.map(
                  (
                    experience: {
                      projectName: string;
                      durationMonths: number;
                      description?: string;
                      startDate: string;
                      endDate?: string;
                      mainTechnologies: string[];
                    },
                    index: number,
                  ) => (
                    <div key={index} className="rounded-lg border p-4">
                      <div className="mb-2 flex items-center justify-between">
                        <h4 className="font-medium">{experience.projectName}</h4>
                        <Badge variant="outline">{experience.durationMonths} meses</Badge>
                      </div>
                      {experience.description && <p className="mb-3 text-sm text-gray-600">{experience.description}</p>}
                      <div className="flex items-center justify-between text-sm">
                        <span>
                          {new Date(experience.startDate).toLocaleDateString('es-ES')} -{' '}
                          {experience.endDate ? new Date(experience.endDate).toLocaleDateString('es-ES') : 'Presente'}
                        </span>
                      </div>
                      {experience.mainTechnologies.length > 0 && (
                        <div className="mt-3">
                          <Label className="text-xs font-medium text-gray-500">Tecnologías principales:</Label>
                          <div className="mt-1 flex flex-wrap gap-1">
                            {experience.mainTechnologies.map((tech, techIndex) => (
                              <Badge key={techIndex} variant="secondary" className="text-xs">
                                {tech}
                              </Badge>
                            ))}
                          </div>
                        </div>
                      )}
                    </div>
                  ),
                )}
              </div>
            </CardContent>
          </Card>

          {/* Tecnologías */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Code className="h-5 w-5" />
                Tecnologías ({profile.technologies.length})
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                {profile.technologies.map(
                  (
                    tech: { technologyName: string; sfiaLevel: number; categoryName: string; yearsExperience: number },
                    index: number,
                  ) => (
                    <div key={index} className="rounded-lg border p-3">
                      <div className="mb-1 flex items-center justify-between">
                        <h4 className="text-sm font-medium">{tech.technologyName}</h4>
                        <Badge variant="outline" className="text-xs">
                          SFIA {tech.sfiaLevel}
                        </Badge>
                      </div>
                      <p className="mb-1 text-xs text-gray-600">{tech.categoryName}</p>
                      <p className="text-xs">
                        <strong>Experiencia:</strong> {tech.yearsExperience} años
                      </p>
                    </div>
                  ),
                )}
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Sidebar de acciones */}
        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>Acciones de Verificación</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div>
                <Label htmlFor="sfiaProposed">Nivel SFIA Propuesto (Opcional)</Label>
                <Input
                  id="sfiaProposed"
                  type="number"
                  min="1"
                  max="7"
                  value={sfiaProposed || ''}
                  onChange={(e) => setSfiaProposed(e.target.value ? parseInt(e.target.value) : undefined)}
                  placeholder="Ej: 3"
                />
              </div>

              <div>
                <Label htmlFor="notes">Notas</Label>
                <TextArea
                  id="notes"
                  value={notes}
                  onChange={(e) => setNotes(e.target.value)}
                  placeholder="Agregar comentarios sobre la verificación..."
                  rows={4}
                />
              </div>

              <Separator />

              <div className="space-y-3">
                <Button
                  onClick={handleApprove}
                  disabled={isProcessing}
                  className="w-full bg-green-600 hover:bg-green-700"
                >
                  <CheckCircle className="mr-2 h-4 w-4" />
                  {isProcessing ? 'Procesando...' : 'Aprobar Perfil'}
                </Button>

                <Button
                  onClick={handleReject}
                  disabled={isProcessing || !notes.trim()}
                  variant="destructive"
                  className="w-full"
                >
                  <XCircle className="mr-2 h-4 w-4" />
                  {isProcessing ? 'Procesando...' : 'Rechazar Perfil'}
                </Button>
              </div>

              {!notes.trim() && (
                <p className="text-xs text-gray-500">* Las notas son requeridas para rechazar un perfil</p>
              )}
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
