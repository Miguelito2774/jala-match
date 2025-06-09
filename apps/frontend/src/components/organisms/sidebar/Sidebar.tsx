'use client';

import { useState } from 'react';

import { useRouter } from 'next/navigation';

import { useAuth } from '@/contexts/AuthContext';
import { cn } from '@/lib/utils';

import { HelpCircle, LogOut, Menu, Settings, Shield, UserCircle, Users, Wrench, X } from 'lucide-react';

import { SidebarItem } from './SidebarItem';

interface NavItem {
  href: string;
  icon: typeof Users;
  label: string;
  roles: ('Employee' | 'Manager' | 'Admin')[];
}

const NAV_ITEMS: NavItem[] = [
  // Rutas de empleado
  {
    href: 'profile',
    icon: UserCircle,
    label: 'Mi Perfil',
    roles: ['Employee'],
  },
  {
    href: '/verifications',
    icon: Shield,
    label: 'Mis Verificaciones',
    roles: ['Employee'],
  },
  {
    href: '/my-teams',
    icon: Users,
    label: 'Mis Equipos',
    roles: ['Employee'],
  },

  // Rutas de manager
  {
    href: '/teams',
    icon: Users,
    label: 'Mis Equipos',
    roles: ['Manager'],
  },
  {
    href: '/team-builder',
    icon: Wrench,
    label: 'Generador de Equipos',
    roles: ['Manager'],
  },
  {
    href: '/profile-verifications',
    icon: UserCircle,
    label: 'Verificar Perfiles',
    roles: ['Manager'],
  },

  // Rutas de admin

  // Rutas compartidas
  {
    href: '/settings',
    icon: Settings,
    label: 'Configuración',
    roles: ['Employee', 'Manager', 'Admin'],
  },
];

export const Sidebar = () => {
  const [isOpen, setIsOpen] = useState(false);
  const router = useRouter();
  const { user, logout, isAuthenticated } = useAuth();

  if (!isAuthenticated || !user) {
    return null;
  }

  const filteredNavItems = NAV_ITEMS.filter((item) => item.roles.includes(user.role));

  const handleLogout = () => {
    logout();
    setIsOpen(false);
    router.push('/login');
  };

  return (
    <>
      {/* Mobile menu button */}
      <button
        className="fixed top-4 left-4 z-40 rounded-md bg-white p-2 text-gray-500 shadow-lg md:hidden"
        onClick={() => setIsOpen(!isOpen)}
        aria-label="Toggle sidebar"
      >
        {isOpen ? <X size={24} /> : <Menu size={24} />}
      </button>

      {/* Sidebar */}
      <aside
        className={cn(
          'fixed inset-y-0 left-0 z-50 w-64 transform bg-white shadow-xl transition-all duration-300 ease-in-out',
          isOpen ? 'translate-x-0' : '-translate-x-full',
          'md:translate-x-0',
        )}
      >
        <div className="flex h-full flex-col">
          {/* Header */}
          <div className="flex flex-shrink-0 items-center px-4 py-5">
            <h1 className="text-xl font-bold text-blue-600">Jala Match</h1>
          </div>

          {/* User info */}
          <div className="border-b border-gray-200 px-4 py-3">
            <div className="flex items-center space-x-3">
              <div className="flex-shrink-0">
                <UserCircle className="h-8 w-8 text-gray-400" />
              </div>
              <div className="min-w-0 flex-1">
                <p className="truncate text-sm font-medium text-gray-900">{user.email}</p>
                <p className="text-xs text-gray-500">
                  {user.role === 'Employee' && 'Empleado'}
                  {user.role === 'Manager' && 'Manager'}
                  {user.role === 'Admin' && 'Administrador'}
                </p>
              </div>
            </div>

            {/* Employee profile status */}
            {user.role === 'Employee' && (
              <div className="mt-2">
                <span
                  className={cn(
                    'inline-flex items-center rounded-full px-2 py-1 text-xs font-medium',
                    user.isProfileVerified
                      ? 'bg-green-100 text-green-800'
                      : user.hasProfile
                        ? 'bg-yellow-100 text-yellow-800'
                        : 'bg-red-100 text-red-800',
                  )}
                >
                  {user.isProfileVerified
                    ? 'Perfil Verificado'
                    : user.hasProfile
                      ? 'Pendiente Verificación'
                      : 'Perfil Incompleto'}
                </span>
              </div>
            )}
          </div>

          {/* Navigation */}
          <div className="flex flex-1 flex-col overflow-y-auto pb-10">
            <nav className="flex-1 space-y-1 px-2 py-4">
              {filteredNavItems.map((item) => (
                <SidebarItem
                  key={item.href}
                  href={item.href}
                  icon={item.icon}
                  label={item.label}
                  onNavigate={() => setIsOpen(false)}
                />
              ))}
            </nav>

            {/* FAQ Link */}
            <div className="px-2 pb-2">
              <SidebarItem
                href="/faq"
                icon={HelpCircle}
                label="Preguntas Frecuentes"
                onNavigate={() => setIsOpen(false)}
              />
            </div>

            {/* Logout button */}
            <div className="px-2 pb-4">
              <button
                onClick={handleLogout}
                className="flex w-full items-center rounded-md px-2 py-2 text-sm font-medium text-gray-600 transition-colors hover:bg-gray-50 hover:text-gray-900"
              >
                <LogOut className="mr-3 h-5 w-5" />
                Cerrar Sesión
              </button>
            </div>
          </div>
        </div>
      </aside>

      {/* Mobile overlay */}
      {isOpen && <div className="fixed inset-0 z-20 bg-black/50 md:hidden" onClick={() => setIsOpen(false)} />}
    </>
  );
};
