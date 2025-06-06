'use client';

import { useState } from 'react';

import { DashboardLayout } from '@/components/templates/DashboardLayout';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { useVerificationHistory } from '@/hooks/useVerificationHistory';

import { AlertCircle, Calendar, CheckCircle, Clock, Filter, MessageSquare, Shield, User } from 'lucide-react';

const getStatusConfig = (status: string) => {
  switch (status) {
    case 'Pending':
      return {
        label: 'Pendiente',
        variant: 'secondary' as const,
        icon: Clock,
        color: 'text-yellow-600',
        badgeClass: 'bg-yellow-100 text-yellow-800 border-yellow-200',
        cardClass: 'border-l-4 border-l-yellow-400',
      };
    case 'Approved':
      return {
        label: 'Aprobado',
        variant: 'default' as const,
        icon: CheckCircle,
        color: 'text-green-600',
        badgeClass: 'bg-green-100 text-green-800 border-green-200',
        cardClass: 'border-l-4 border-l-green-400',
      };
    case 'Rejected':
      return {
        label: 'Rechazado',
        variant: 'destructive' as const,
        icon: AlertCircle,
        color: 'text-red-600',
        badgeClass: 'bg-red-100 text-red-800 border-red-200',
        cardClass: 'border-l-4 border-l-red-400',
      };
    default:
      return {
        label: 'Desconocido',
        variant: 'outline' as const,
        icon: AlertCircle,
        color: 'text-gray-600',
        badgeClass: 'bg-gray-100 text-gray-800 border-gray-200',
        cardClass: 'border-l-4 border-l-gray-400',
      };
  }
};

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('es-ES', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
};

const VerificationCard = ({ verification }: { verification: any }) => {
  const statusConfig = getStatusConfig(verification.status);
  const StatusIcon = statusConfig.icon;

  return (
    <Card className={`w-full ${statusConfig.cardClass}`}>
      <CardHeader className="pb-3">
        <div className="flex items-center justify-between">
          <CardTitle className="flex items-center gap-2 text-lg">
            <Shield className="h-5 w-5" />
            {verification.sfiaProposed > 0 ? (
              `Verificación SFIA Nivel ${verification.sfiaProposed}`
            ) : (
              <>
                Verificación de Perfil
                <span className="text-sm font-normal text-gray-600">(SFIA no especificado)</span>
              </>
            )}
          </CardTitle>
          <Badge variant={statusConfig.variant} className={`flex items-center gap-1 ${statusConfig.badgeClass}`}>
            <StatusIcon className="h-3 w-3" />
            {statusConfig.label}
          </Badge>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="grid grid-cols-1 gap-4 text-sm md:grid-cols-2">
          <div className="flex items-center gap-2">
            <Calendar className="text-muted-foreground h-4 w-4" />
            <span className="text-muted-foreground">Solicitado:</span>
            <span className="font-medium">{formatDate(verification.requestedAt)}</span>
          </div>

          {verification.reviewedAt && verification.reviewedAt !== verification.requestedAt && (
            <div className="flex items-center gap-2">
              <Calendar className="text-muted-foreground h-4 w-4" />
              <span className="text-muted-foreground">Revisado:</span>
              <span className="font-medium">{formatDate(verification.reviewedAt)}</span>
            </div>
          )}

          {verification.reviewerName && (
            <div className="flex items-center gap-2">
              <User className="text-muted-foreground h-4 w-4" />
              <span className="text-muted-foreground">Revisor:</span>
              <span className="font-medium">{verification.reviewerName}</span>
            </div>
          )}
        </div>

        {verification.notes && (
          <div className="border-t pt-4">
            <div className="flex items-start gap-2">
              <MessageSquare className="text-muted-foreground mt-0.5 h-4 w-4" />
              <div className="space-y-1">
                <span className="text-muted-foreground text-sm font-medium">Notas del revisor:</span>
                <p className="bg-muted rounded-md p-3 text-sm">{verification.notes}</p>
              </div>
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
};

const LoadingSkeleton = () => (
  <Card className="w-full">
    <CardHeader className="pb-3">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Skeleton className="h-5 w-5" />
          <Skeleton className="h-6 w-48" />
        </div>
        <Skeleton className="h-6 w-20" />
      </div>
    </CardHeader>
    <CardContent className="space-y-4">
      <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
        <Skeleton className="h-4 w-full" />
        <Skeleton className="h-4 w-full" />
      </div>
      <div className="border-t pt-4">
        <Skeleton className="mb-2 h-4 w-32" />
        <Skeleton className="h-16 w-full" />
      </div>
    </CardContent>
  </Card>
);

export default function VerificationsPage() {
  const { verifications, loading, error, refetch } = useVerificationHistory();
  const [statusFilter, setStatusFilter] = useState<'all' | 'Pending' | 'Approved' | 'Rejected'>('all');

  const filteredVerifications = verifications.filter((verification) => {
    if (statusFilter === 'all') return true;
    return verification.status === statusFilter;
  });

  const getFilterButtonVariant = (filter: string) => {
    return statusFilter === filter ? 'default' : 'outline';
  };

  if (loading) {
    return (
      <DashboardLayout>
        <div className="space-y-6">
          <div className="space-y-2">
            <h1 className="text-3xl font-bold tracking-tight">Mis Verificaciones</h1>
            <p className="text-muted-foreground">Historial completo de tus solicitudes de verificación de perfil</p>
          </div>
          <div className="space-y-4">
            {[...Array(3)].map((_, index) => (
              <LoadingSkeleton key={index} />
            ))}
          </div>
        </div>
      </DashboardLayout>
    );
  }

  if (error) {
    return (
      <DashboardLayout>
        <Card className="border-destructive">
          <CardContent className="flex items-center gap-2 p-6">
            <AlertCircle className="text-destructive h-5 w-5" />
            <div>
              <h3 className="text-destructive font-semibold">Error al cargar verificaciones</h3>
              <p className="text-muted-foreground mt-1 text-sm">{error}</p>
              <button onClick={refetch} className="text-primary mt-2 text-sm hover:underline">
                Intentar nuevamente
              </button>
            </div>
          </CardContent>
        </Card>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout>
      <div className="space-y-6">
        <div className="space-y-2">
          <h1 className="text-3xl font-bold tracking-tight">Mis Verificaciones</h1>
          <p className="text-muted-foreground">Historial completo de tus solicitudes de verificación de perfil</p>
        </div>

        {/* Filtros */}
        <div className="flex flex-wrap gap-2">
          <Button
            variant={getFilterButtonVariant('all')}
            size="sm"
            onClick={() => setStatusFilter('all')}
            className="flex items-center gap-2"
          >
            <Filter className="h-4 w-4" />
            Todas ({verifications.length})
          </Button>
          <Button
            variant={getFilterButtonVariant('Pending')}
            size="sm"
            onClick={() => setStatusFilter('Pending')}
            className="flex items-center gap-2"
          >
            <Clock className="h-4 w-4" />
            Pendientes ({verifications.filter((v) => v.status === 'Pending').length})
          </Button>
          <Button
            variant={getFilterButtonVariant('Approved')}
            size="sm"
            onClick={() => setStatusFilter('Approved')}
            className="flex items-center gap-2"
          >
            <CheckCircle className="h-4 w-4" />
            Aprobadas ({verifications.filter((v) => v.status === 'Approved').length})
          </Button>
          <Button
            variant={getFilterButtonVariant('Rejected')}
            size="sm"
            onClick={() => setStatusFilter('Rejected')}
            className="flex items-center gap-2"
          >
            <AlertCircle className="h-4 w-4" />
            Rechazadas ({verifications.filter((v) => v.status === 'Rejected').length})
          </Button>
        </div>

        {filteredVerifications.length === 0 ? (
          <Card>
            <CardContent className="flex flex-col items-center justify-center p-8 text-center">
              <Shield className="text-muted-foreground mb-4 h-12 w-12" />
              <h3 className="mb-2 text-lg font-semibold">
                {statusFilter === 'all'
                  ? 'No tienes verificaciones'
                  : `No tienes verificaciones ${statusFilter === 'Pending' ? 'pendientes' : statusFilter === 'Approved' ? 'aprobadas' : 'rechazadas'}`}
              </h3>
              <p className="text-muted-foreground max-w-md">
                {statusFilter === 'all'
                  ? 'Aún no has solicitado ninguna verificación de perfil. Completa tu perfil y solicita una verificación para validar tu nivel SFIA.'
                  : `No hay verificaciones con estado ${statusFilter === 'Pending' ? 'pendiente' : statusFilter === 'Approved' ? 'aprobado' : 'rechazado'}.`}
              </p>
            </CardContent>
          </Card>
        ) : (
          <div className="space-y-4">
            {filteredVerifications.map((verification) => (
              <VerificationCard key={verification.id} verification={verification} />
            ))}
          </div>
        )}
      </div>
    </DashboardLayout>
  );
}
