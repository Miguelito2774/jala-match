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
    <div className="bg-gray-50 min-h-screen">
      <Sidebar role={role} />
      <div className="md:pl-64 flex flex-1 flex-col">
        <Header />
        <main className="pb-8 flex-1">
          <div className="max-w-7xl px-4 py-6 sm:px-6 lg:px-8 mx-auto">{children}</div>
        </main>
      </div>
    </div>
  );
};
