'use client';

import { FileText, HelpCircle, Settings, User, Users } from 'lucide-react';

import { ROUTES } from '../../../../routes';
import { SidebarItem } from './SidebarItem';

export const Sidebar = ({ role = 'employee' }) => {
  // Definición de items usando ROUTES
  const employeeItems = [
    { href: ROUTES.EMPLOYEE.DASHBOARD, icon: User, label: 'Inicio' },
    { href: ROUTES.EMPLOYEE.PROFILE, icon: User, label: 'Perfil' },
    { href: ROUTES.EMPLOYEE.TEAMS, icon: Users, label: 'Equipos' },
    { href: ROUTES.PUBLIC.HELP, icon: HelpCircle, label: 'Ayuda' },
  ];

  const managerItems = [...employeeItems, { href: ROUTES.MANAGER.TEAM_BUILDER, icon: Users, label: 'Crear Equipos' }];

  const adminItems = [...managerItems, { href: ROUTES.ADMIN.INVITATIONS, icon: FileText, label: 'Invitar Managers' }];

  const items = role === 'admin' ? adminItems : role === 'manager' ? managerItems : employeeItems;

  return (
    <div className="fixed inset-y-0 left-0 flex w-64 flex-col border-r border-gray-200 bg-white">
      <div className="flex flex-shrink-0 items-center px-4 py-5">
        <h1 className="text-primary-blue text-xl font-bold">Jala Match</h1>
      </div>
      <div className="flex flex-1 flex-col overflow-y-auto">
        <nav className="flex-1 space-y-1 px-2 pb-4">
          {items.map((item) => (
            <SidebarItem key={item.href} {...item} />
          ))}
        </nav>
        <div className="px-2 pb-4">
          <SidebarItem href={ROUTES.SETTINGS} icon={Settings} label="Configuración" />
        </div>
      </div>
    </div>
  );
};
