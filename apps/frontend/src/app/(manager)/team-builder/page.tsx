import { TeamBuilder } from '@/components/organisms/sections/TeamBuilder';
import { ProtectedRoute } from '@/components/organisms/sidebar/ProtectedRoute';
import { DashboardLayout } from '@/components/templates/DashboardLayout';

export default function TeamBuilderPage() {
  return (
    <ProtectedRoute allowedRoles={['Manager']}>
      <DashboardLayout role="manager">
        <div className="space-y-8">
          <div className="border-b border-gray-200 pb-4">
            <h1 className="text-2xl font-bold text-gray-900">Generador de Equipos Inteligente</h1>
            <p className="mt-1 text-sm text-gray-500">
              Crea equipos optimizados con IA basados en compatibilidad técnica y humana
            </p>
          </div>

          <TeamBuilder />
        </div>
      </DashboardLayout>
    </ProtectedRoute>
  );
}
