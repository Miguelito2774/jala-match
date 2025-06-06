'use client';

import { DashboardLayout } from '@/components/templates/DashboardLayout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';

import { Users } from 'lucide-react';

export default function MyTeamsPage() {
  // TO-DO
  // const { teams, loading, error } = useEmployeeTeams();

  return (
    <DashboardLayout>
      <div className="space-y-6">
        <div className="space-y-2">
          <h1 className="text-3xl font-bold tracking-tight">Mis Equipos</h1>
          <p className="text-muted-foreground">Equipos en los que participas como miembro</p>
        </div>

        {/* Placeholder content - luego reemplazarás con datos reales */}
        <div className="grid gap-6">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Users className="h-5 w-5" />
                Próximamente
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-muted-foreground">
                Aquí podrás ver todos los equipos en los que participas como miembro.
              </p>
            </CardContent>
          </Card>
        </div>
      </div>
    </DashboardLayout>
  );
}
