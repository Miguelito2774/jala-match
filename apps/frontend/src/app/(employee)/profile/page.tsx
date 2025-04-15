import { ProfileSections } from '@/components/organisms/sections/ProfileSections';
import { DashboardLayout } from '@/components/templates/DashboardLayout';

export default function ProfilePage() {
  return (
    <DashboardLayout>
      <div className="space-y-8">
        <div className="border-b border-gray-200 pb-4">
          <h1 className="text-2xl font-bold text-gray-900">Mi Perfil</h1>
          <p className="mt-1 text-sm text-gray-500">Completa tu perfil para ser considerado en los equipos</p>
        </div>

        {/* Profile sections component */}
        <ProfileSections />
      </div>
    </DashboardLayout>
  );
}
