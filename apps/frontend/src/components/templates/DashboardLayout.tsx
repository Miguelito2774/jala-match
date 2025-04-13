'use client';

import { Header } from '@/components/organisms/header/Header';
import { Sidebar } from '@/components/organisms/sidebar/Sidebar';

export const DashboardLayout = ({
  children,
  role = 'employee',
}: {
  children: React.ReactNode;
  role?: 'employee' | 'manager' | 'admin';
}) => {
  return (
    <div className="min-h-screen bg-gray-50">
      <Sidebar role={role} />
      <div className="flex flex-1 flex-col md:pl-64">
        <Header />
        <main className="flex-1 pb-8">
          <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">{children}</div>
        </main>
      </div>
    </div>
  );
};
