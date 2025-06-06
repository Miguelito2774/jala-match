'use client';

import { useEffect } from 'react';

import { useRouter } from 'next/navigation';

import { useAuth } from '@/contexts/AuthContext';

export const DashboardRedirect = () => {
  const { user, isLoading, isAuthenticated } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!isLoading && isAuthenticated && user) {
      const redirectPath = getDashboardForRole(user.role);
      router.replace(redirectPath);
    }
  }, [user, isLoading, isAuthenticated]);

  const getDashboardForRole = (role: string) => {
    switch (role) {
      case 'Employee':
        return '/employee/profile';
      case 'Manager':
        return '/manager/team-builder';
      case 'Admin':
        return '/admin/invitations';
      default:
        return '/login';
    }
  };

  // Mostrar loading mientras se procesa la redirección
  return (
    <div className="flex min-h-screen items-center justify-center">
      <div className="flex flex-col items-center space-y-4">
        <div className="h-12 w-12 animate-spin rounded-full border-b-2 border-blue-600"></div>
        <p className="text-gray-600">{isLoading ? 'Cargando...' : 'Redirigiendo...'}</p>
      </div>
    </div>
  );
};
