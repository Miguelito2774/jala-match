import { FileText, HelpCircle, Settings, User, Users } from 'lucide-react';

import { SidebarItem } from './SidebarItem';

export const Sidebar = ({ role = 'employee' }) => {
  const employeeItems = [
    { href: '/employee/dashboard', icon: User, label: 'Inicio' },
    { href: '/employee/profile', icon: User, label: 'Perfil' },
    { href: '/employee/teams', icon: Users, label: 'Equipos' },
    { href: '/help', icon: HelpCircle, label: 'Ayuda' },
  ];

  const managerItems = [...employeeItems, { href: '/manager/team-builder', icon: Users, label: 'Crear Equipos' }];

  const adminItems = [...managerItems, { href: '/admin/invitations', icon: FileText, label: 'Invitar Managers' }];

  const items = role === 'admin' ? adminItems : role === 'manager' ? managerItems : employeeItems;

  return (
    <div className="md:fixed md:inset-y-0 md:flex md:w-64 md:flex-col hidden">
      <div className="border-gray-200 bg-white pt-5 flex flex-grow flex-col overflow-y-auto border-r">
        <div className="px-4 flex flex-shrink-0 items-center">
          <h1 className="text-xl font-bold text-blue-600">Jala Match</h1>
        </div>
        <div className="mt-5 flex flex-1 flex-col">
          <nav className="space-y-1 px-2 pb-4 flex-1">
            {items.map((item) => (
              <SidebarItem key={item.href} {...item} />
            ))}
          </nav>
        </div>
        <div className="px-2 pb-4">
          <SidebarItem href="/settings" icon={Settings} label="Configuración" />
        </div>
      </div>
    </div>
  );
};
