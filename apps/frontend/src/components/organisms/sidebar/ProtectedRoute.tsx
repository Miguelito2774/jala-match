'use client';

import { ReactNode, useEffect } from 'react';

import { useRouter } from 'next/navigation';

import { useAuth } from '@/contexts/AuthContext';

interface ProtectedRouteProps {
  children: ReactNode;
  allowedRoles?: ('Employee' | 'Manager' | 'Admin')[];
  redirectTo?: string;
}

export const ProtectedRoute = ({
  children,
  allowedRoles = ['Employee', 'Manager', 'Admin'],
  redirectTo,
}: ProtectedRouteProps) => {
  const { user, isLoading, isAuthenticated } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (isLoading) return;

    // Si no está autenticado, redirigir al login
    if (!isAuthenticated) {
      router.replace('/login');
      return;
    }

    // Si está autenticado pero no tiene el rol requerido
    if (user && !allowedRoles.includes(user.role)) {
      const defaultRedirect = getDefaultRedirectForRole(user.role);
      router.replace(redirectTo || defaultRedirect);
      return;
    }
  }, [user, isLoading, isAuthenticated, allowedRoles, redirectTo, router]);

  // Mostrar loading mientras se verifica la autenticación
  if (isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="flex flex-col items-center space-y-4">
          <div className="h-12 w-12 animate-spin rounded-full border-b-2 border-blue-600"></div>
          <p className="text-gray-600">Verificando permisos...</p>
        </div>
      </div>
    );
  }

  // Si no está autenticado o no tiene permisos, no mostrar nada
  if (!isAuthenticated || !user || !allowedRoles.includes(user.role)) {
    return null;
  }

  return <>{children}</>;
};

function getDefaultRedirectForRole(role: string): string {
  switch (role) {
    case 'Employee':
      return '/employee/profile';
    case 'Manager':
      return '/teams';
    case 'Admin':
      return '/admin/invitations';
    default:
      return '/login';
  }
}
