'use client';

import { useEffect, useState } from 'react';

import Link from 'next/link';

import { PageLoader } from '@/components/atoms/loaders/PageLoader';
import { ProtectedRoute } from '@/components/organisms/sidebar/ProtectedRoute';
import { DashboardLayout } from '@/components/templates/DashboardLayout';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { useAuth } from '@/contexts/AuthContext';
import { useProfileVerifications, type PendingVerificationsResponse } from '@/hooks/useProfileVerifications';

import { Briefcase, Clock, Eye, MapPin, User } from 'lucide-react';

export default function ProfileVerificationsPage() {
  const { user } = useAuth();
  const [currentPage, setCurrentPage] = useState(1);
  const [data, setData] = useState<PendingVerificationsResponse | null>(null);
  const pageSize = 10;

  const { loading, error, getPendingVerifications } = useProfileVerifications();

  useEffect(() => {
    const loadData = async () => {
      try {
        const result = await getPendingVerifications(pageSize, currentPage);
        setData(result);
      } catch (_err) {
        // Error is handled by the hook
      }
    };

    if (user?.role === 'Manager') {
      loadData();
    }
  }, [currentPage, pageSize, user, getPendingVerifications]);

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

  if (isLoading) {
    return (
      <ProtectedRoute allowedRoles={['Manager']}>
        <DashboardLayout>
          <PageLoader title="Cargando verificaciones..." subtitle="Obteniendo perfiles pendientes de verificación" />
        </DashboardLayout>
      </ProtectedRoute>
    );
  }

  if (error) {
    return (
      <ProtectedRoute allowedRoles={['Manager']}>
        <DashboardLayout>
          <div className="container mx-auto px-4 py-8">
            <div className="text-center">
              <h1 className="text-2xl font-bold text-red-600">Error</h1>
              <p className="mt-2 text-gray-600">Hubo un error al cargar las verificaciones pendientes.</p>
              <Button onClick={() => window.location.reload()} className="mt-4">
                Reintentar
              </Button>
            </div>
          </div>
        </DashboardLayout>
      </ProtectedRoute>
    );
  }

  const totalPages = Math.ceil((data?.totalCount || 0) / pageSize);

  return (
    <ProtectedRoute allowedRoles={['Manager']}>
      <DashboardLayout>
        <div className="container mx-auto px-4 py-8">
          <div className="mb-8">
            <h1 className="text-3xl font-bold text-gray-900">Verificaciones de Perfiles</h1>
            <p className="mt-2 text-gray-600">Gestiona las solicitudes de verificación de perfiles de empleados</p>
            <div className="mt-4 flex items-center gap-4">
              <Badge variant="secondary" className="flex items-center gap-2">
                <Clock className="h-4 w-4" />
                {data?.totalCount || 0} pendientes
              </Badge>
            </div>
          </div>

          {data?.pendingVerifications.length === 0 ? (
            <Card>
              <CardContent className="py-12 text-center">
                <User className="mx-auto mb-4 h-12 w-12 text-gray-400" />
                <h3 className="mb-2 text-lg font-semibold text-gray-900">No hay verificaciones pendientes</h3>
                <p className="text-gray-600">Todas las solicitudes de verificación han sido procesadas.</p>
              </CardContent>
            </Card>
          ) : (
            <>
              <div className="grid gap-6">
                {data?.pendingVerifications.map((verification) => (
                  <Card key={verification.employeeProfileId} className="transition-shadow hover:shadow-lg">
                    <CardHeader>
                      <div className="flex items-center justify-between">
                        <div className="flex items-center gap-3">
                          <Avatar className="h-12 w-12">
                            <AvatarImage
                              src={verification.profilePictureUrl || undefined}
                              alt={verification.employeeName}
                            />
                            <AvatarFallback className="bg-blue-500 text-sm font-semibold text-white">
                              {verification.employeeName
                                .split(' ')
                                .map((n) => n[0])
                                .join('')
                                .toUpperCase()}
                            </AvatarFallback>
                          </Avatar>
                          <div>
                            <CardTitle className="text-xl">{verification.employeeName}</CardTitle>
                            <CardDescription className="mt-1 flex items-center gap-2">
                              <span>{verification.employeeEmail}</span>
                            </CardDescription>
                          </div>
                        </div>
                        <Link href={`/profile-verifications/${verification.employeeProfileId}`}>
                          <Button variant="outline" size="sm" className="flex items-center gap-2">
                            <Eye className="h-4 w-4" />
                            Ver Detalles
                          </Button>
                        </Link>
                      </div>
                    </CardHeader>
                    <CardContent>
                      <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
                        <div className="flex items-center gap-2">
                          <MapPin className="h-4 w-4 text-gray-500" />
                          <span className="text-sm">
                            {verification.country} ({verification.timezone})
                          </span>
                        </div>
                        <div className="flex items-center gap-2">
                          <Badge variant="outline">SFIA {verification.sfiaLevelGeneral}</Badge>
                        </div>
                        <div className="flex items-center gap-2">
                          <Briefcase className="h-4 w-4 text-gray-500" />
                          <span className="text-sm">{verification.yearsExperienceTotal} años de experiencia</span>
                        </div>
                        <div className="flex items-center gap-2">
                          <Clock className="h-4 w-4 text-gray-500" />
                          <span className="text-sm">
                            {new Date(verification.requestedAt).toLocaleDateString('es-ES')}
                          </span>
                        </div>
                      </div>

                      {verification.specializedRoles.length > 0 && (
                        <div className="mt-4">
                          <h4 className="mb-2 text-sm font-medium text-gray-700">Roles especializados:</h4>
                          <div className="flex flex-wrap gap-2">
                            {verification.specializedRoles.map((role, index) => (
                              <Badge key={index} variant="secondary" className="text-xs">
                                {role}
                              </Badge>
                            ))}
                          </div>
                        </div>
                      )}
                    </CardContent>
                  </Card>
                ))}
              </div>

              {/* Paginación */}
              {totalPages > 1 && (
                <div className="mt-8 flex items-center justify-center gap-2">
                  <Button
                    variant="outline"
                    onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
                    disabled={currentPage === 1}
                  >
                    Anterior
                  </Button>
                  <span className="text-sm text-gray-600">
                    Página {currentPage} de {totalPages}
                  </span>
                  <Button
                    variant="outline"
                    onClick={() => setCurrentPage((prev) => Math.min(prev + 1, totalPages))}
                    disabled={currentPage === totalPages}
                  >
                    Siguiente
                  </Button>
                </div>
              )}
            </>
          )}
        </div>
      </DashboardLayout>
    </ProtectedRoute>
  );
}
