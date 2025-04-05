import { TeamBuilder } from '@/components/organisms/sections/TeamBuilder';
import { DashboardLayout } from '@/components/templates/DashboardLayout';

export default function TeamBuilderPage() {
  return (
    <DashboardLayout role="manager">
      <div className="space-y-8">
        <div className="border-gray-200 pb-4 border-b">
          <h1 className="text-2xl font-bold text-gray-900">Generador de Equipos</h1>
          <p className="mt-1 text-sm text-gray-500">Crea equipos optimizados con IA basados en compatibilidad</p>
        </div>

        <TeamBuilder />
      </div>
    </DashboardLayout>
  );
}
