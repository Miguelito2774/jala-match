'use client';

import { useState } from 'react';

import { cn } from '@/lib/utils';

import { Menu, Users, X } from 'lucide-react';

import { ROUTES } from '../../../../routes';
import { SidebarItem } from './SidebarItem';

export const Sidebar = () => {
  const [isOpen, setIsOpen] = useState(false);

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
          <div className="flex flex-shrink-0 items-center px-4 py-5">
            <h1
              className={cn(
                'text-primary-blue text-xl font-bold transition-all duration-300',
                isOpen ? 'ml-12' : 'ml-0',
              )}
            >
              Jala Match
            </h1>
          </div>
          <div className="flex flex-1 flex-col overflow-y-auto pb-10">
            <nav className="flex-1 space-y-1 px-2 py-4">
              <SidebarItem
                href={ROUTES.MANAGER.TEAM_BUILDER}
                icon={Users}
                label="Crear Equipos"
                onNavigate={() => setIsOpen(false)}
              />
              <SidebarItem
                href={ROUTES.MANAGER.TEAM_LIST}
                icon={Users}
                label="Mis Equipos"
                onNavigate={() => setIsOpen(false)}
              />
            </nav>
          </div>
        </div>
      </aside>

      {/* Mobile overlay */}
      {isOpen && <div className="fixed inset-0 z-20 bg-black/50 md:hidden" onClick={() => setIsOpen(false)} />}
    </>
  );
};
