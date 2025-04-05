import { Button } from '@/components/atoms/buttons/Button';

import { UserCircle } from 'lucide-react';

export const Header = () => {
  return (
    <header className="bg-white shadow-sm">
      <div className="max-w-7xl px-4 py-4 sm:px-6 lg:px-8 mx-auto flex items-center justify-between">
        <h1 className="text-lg font-semibold text-gray-900">Dashboard</h1>
        <div className="space-x-4 flex items-center">
          <Button variant="ghost" size="sm">
            <UserCircle className="h-5 w-5 text-gray-500" />
            <span className="ml-2">Mi Perfil</span>
          </Button>
        </div>
      </div>
    </header>
  );
};
