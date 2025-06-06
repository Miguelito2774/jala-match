'use client';

import { useState } from 'react';

import { useRouter } from 'next/navigation';

import { Button } from '@/components/atoms/buttons/Button';
import { ProfileSectionCard } from '@/components/molecules/cards/ProfileSectionCard';
import { useAuth } from '@/contexts/AuthContext';
import {
  useEmployeeLanguages,
  useEmployeeProfile,
  useEmployeeTechnologies,
  usePersonalInterests,
  useWorkExperiences,
} from '@/hooks/useEmployeeProfile';

export const ProfileSections = () => {
  const router = useRouter();
  const { user } = useAuth();
  const { profile, loading: profileLoading, requestVerification } = useEmployeeProfile();
  const { languages, loading: languagesLoading } = useEmployeeLanguages();
  const { technologies, loading: technologiesLoading } = useEmployeeTechnologies();
  const { experiences, loading: experiencesLoading } = useWorkExperiences();
  const { interests, loading: interestsLoading } = usePersonalInterests();

  const [isRequesting, setIsRequesting] = useState(false);
  const [verificationError, setVerificationError] = useState<string | null>(null);

  const getSectionProgress = () => {
    const generalCompleted = profile?.generalInfo
      ? !!(
          profile.generalInfo.firstName &&
          profile.generalInfo.lastName &&
          profile.generalInfo.country &&
          profile.generalInfo.timezone
        )
      : false;

    const technicalCompleted = profile?.technicalProfile
      ? !!(
          profile.technicalProfile.sfiaLevelGeneral &&
          profile.technicalProfile.mbti &&
          profile.technicalProfile.specializedRoles &&
          profile.technicalProfile.specializedRoles.length > 0
        )
      : false;

    const experienceCompleted = experiences.length > 0;
    const interestsCompleted = interests.length > 0;

    return {
      general: generalCompleted,
      technical: technicalCompleted,
      experience: experienceCompleted,
      interests: interestsCompleted,
    };
  };

  const progress = getSectionProgress();
  const allSectionsCompleted = Object.values(progress).every(Boolean);
  const isLoading = profileLoading || languagesLoading || technologiesLoading || experiencesLoading || interestsLoading;

  const sections = [
    {
      id: 'general',
      title: 'Información General',
      completed: progress.general,
      description: 'Datos personales básicos y disponibilidad',
      route: '/profile/general',
    },
    {
      id: 'technical',
      title: 'Perfil Técnico',
      completed: progress.technical,
      description: 'Habilidades técnicas y especializaciones',
      route: '/profile/technical',
    },
    {
      id: 'experience',
      title: 'Experiencia Laboral',
      completed: progress.experience,
      description: 'Historial profesional y proyectos',
      route: '/profile/experience',
    },
    {
      id: 'interests',
      title: 'Intereses Personales',
      completed: progress.interests,
      description: 'Actividades y preferencias fuera del trabajo',
      route: '/profile/interests',
    },
  ];

  const handleVerificationRequest = async () => {
    if (!user?.id) return;

    try {
      setIsRequesting(true);
      setVerificationError(null);
      await requestVerification();
      // Mostrar mensaje de éxito o redirigir
    } catch (error) {
      setVerificationError(error instanceof Error ? error.message : 'Error al solicitar verificación');
    } finally {
      setIsRequesting(false);
    }
  };

  const getCompletionPercentage = () => {
    const completedSections = Object.values(progress).filter(Boolean).length;
    return Math.round((completedSections / sections.length) * 100);
  };

  if (isLoading) {
    return (
      <div className="flex min-h-[400px] items-center justify-center">
        <div className="flex flex-col items-center space-y-4">
          <div className="h-12 w-12 animate-spin rounded-full border-b-2 border-blue-600"></div>
          <p className="text-gray-600">Cargando información del perfil...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Progress Overview */}
      <div className="bg-card rounded-lg border p-6">
        <div className="mb-4 flex items-center justify-between">
          <div>
            <h3 className="text-lg font-semibold">Progreso del Perfil</h3>
            <p className="text-muted-foreground text-sm">{getCompletionPercentage()}% completado</p>
          </div>
          <div className="text-right">
            <div className="text-primary text-2xl font-bold">
              {Object.values(progress).filter(Boolean).length}/{sections.length}
            </div>
            <p className="text-muted-foreground text-sm">secciones</p>
          </div>
        </div>
        <div className="h-2 w-full rounded-full bg-gray-200">
          <div
            className="bg-primary h-2 rounded-full transition-all duration-300"
            style={{ width: `${getCompletionPercentage()}%` }}
          ></div>
        </div>
      </div>

      {/* Profile Status */}
      {profile && (
        <div className="bg-card rounded-lg border p-4">
          <div className="flex items-center space-x-2">
            {profile.verificationStatus === 2 ? (
              // Aprobado
              <>
                <div className="h-3 w-3 rounded-full bg-green-500"></div>
                <span className="text-sm font-medium text-green-700">Perfil Verificado</span>
              </>
            ) : profile.verificationStatus === 1 && profile.hasVerificationRequests && allSectionsCompleted ? (
              // Verificación realmente solicitada y pendiente
              <>
                <div className="h-3 w-3 rounded-full bg-blue-500"></div>
                <span className="text-sm font-medium text-blue-700">Solicitud Pendiente</span>
              </>
            ) : profile.verificationStatus === 1 && profile.hasVerificationRequests && !allSectionsCompleted ? (
              // Verificación solicitada pero perfil incompleto
              <>
                <div className="h-3 w-3 rounded-full bg-yellow-500"></div>
                <span className="text-sm font-medium text-yellow-700">Complete su perfil para verificación</span>
              </>
            ) : profile.verificationStatus === 3 ? (
              // Rechazado
              <>
                <div className="h-3 w-3 rounded-full bg-red-500"></div>
                <span className="text-sm font-medium text-red-700">Verificación Rechazada</span>
              </>
            ) : allSectionsCompleted ? (
              // Perfil completo, listo para verificación (nunca ha solicitado verificación)
              <>
                <div className="h-3 w-3 rounded-full bg-emerald-500"></div>
                <span className="text-sm font-medium text-emerald-700">Listo para Verificación</span>
              </>
            ) : (
              // Perfil incompleto
              <>
                <div className="h-3 w-3 rounded-full bg-gray-400"></div>
                <span className="text-sm font-medium text-gray-600">Perfil Incompleto</span>
              </>
            )}
          </div>
        </div>
      )}

      {/* Sections Grid */}
      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        {sections.map((section) => (
          <ProfileSectionCard
            key={section.id}
            title={section.title}
            description={section.description}
            completed={section.completed}
            onClick={() => router.push(section.route)}
          />
        ))}
      </div>

      {/* Action Buttons */}
      <div className="flex flex-col space-y-4">
        {verificationError && (
          <div className="rounded-md border border-red-200 bg-red-50 p-3">
            <p className="text-sm text-red-600">{verificationError}</p>
          </div>
        )}

        <div className="flex items-center justify-between">
          <div className="text-muted-foreground text-sm">
            {profile && profile.verificationStatus === 2 ? (
              <span>Su perfil ha sido verificado exitosamente</span>
            ) : profile &&
              profile.verificationStatus === 1 &&
              profile.hasVerificationRequests &&
              allSectionsCompleted ? (
              <span>Su solicitud está pendiente</span>
            ) : profile &&
              profile.verificationStatus === 1 &&
              profile.hasVerificationRequests &&
              !allSectionsCompleted ? (
              <span>Complete todas las secciones para que su solicitud sea procesada</span>
            ) : profile && profile.verificationStatus === 3 ? (
              <span>Su solicitud fue rechazada. Puede solicitar verificación nuevamente</span>
            ) : profile && allSectionsCompleted ? (
              <span>Su perfil está completo y listo para verificación</span>
            ) : allSectionsCompleted ? (
              <span>Su perfil está completo y listo para verificación</span>
            ) : (
              <span>Complete todas las secciones para solicitar verificación</span>
            )}
          </div>

          <div className="flex space-x-3">
            {!profile && (
              <Button variant="outline" onClick={() => router.push('/profile/general')}>
                Comenzar Perfil
              </Button>
            )}
            {profile && profile.verificationStatus === 1 && profile.hasVerificationRequests && allSectionsCompleted && (
              <Button variant="outline" disabled>
                Solicitud Pendiente
              </Button>
            )}
            {profile &&
              profile.verificationStatus === 1 &&
              profile.hasVerificationRequests &&
              !allSectionsCompleted && (
                <Button variant="outline" onClick={() => router.push('/profile/general')}>
                  Completar Perfil
                </Button>
              )}
            {profile && profile.verificationStatus === 2 && (
              <Button variant="outline" disabled>
                Perfil Verificado
              </Button>
            )}
            {profile && profile.verificationStatus === 3 && (
              <Button onClick={handleVerificationRequest} disabled={!allSectionsCompleted || isRequesting}>
                {isRequesting ? 'Solicitando...' : 'Re-solicitar Verificación'}
              </Button>
            )}
            {profile &&
              profile.verificationStatus === 1 &&
              !profile.hasVerificationRequests &&
              allSectionsCompleted && (
                <Button onClick={handleVerificationRequest} disabled={isRequesting}>
                  {isRequesting ? 'Solicitando...' : 'Solicitar Verificación'}
                </Button>
              )}
            {profile && !profile.hasVerificationRequests && !allSectionsCompleted && (
              <Button variant="outline" onClick={() => router.push('/profile/general')}>
                Completar Perfil
              </Button>
            )}
          </div>
        </div>
      </div>

      {/* Quick Stats */}
      {profile && (
        <div className="grid grid-cols-2 gap-4 border-t pt-6 md:grid-cols-4">
          <div className="text-center">
            <div className="text-primary text-2xl font-bold">{languages.length}</div>
            <div className="text-muted-foreground text-sm">Idiomas</div>
          </div>
          <div className="text-center">
            <div className="text-primary text-2xl font-bold">{technologies.length}</div>
            <div className="text-muted-foreground text-sm">Tecnologías</div>
          </div>
          <div className="text-center">
            <div className="text-primary text-2xl font-bold">{experiences.length}</div>
            <div className="text-muted-foreground text-sm">Proyectos</div>
          </div>
          <div className="text-center">
            <div className="text-primary text-2xl font-bold">{interests.length}</div>
            <div className="text-muted-foreground text-sm">Intereses</div>
          </div>
        </div>
      )}
    </div>
  );
};
