'use client';

import { useState } from 'react';

import { DashboardLayout } from '@/components/templates/DashboardLayout';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useAuth } from '@/contexts/AuthContext';
import { useCreateInvitation } from '@/hooks/useCreateInvitation';

import { Copy, Link, Shield, UserPlus } from 'lucide-react';
import { toast } from 'sonner';

export default function AdminDashboard() {
  const { user: _user } = useAuth();
  const [email, setEmail] = useState('');
  const { createInvitation, isLoading } = useCreateInvitation();
  const [generatedLinks, setGeneratedLinks] = useState<Array<{ email: string; link: string; createdAt: Date }>>([]);

  const handleCreateInvitation = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!email) {
      toast.error('Por favor, ingresa un email');
      return;
    }

    try {
      const invitationToken = await createInvitation(email, 'Manager');
      const fullLink = `${window.location.origin}/register?invitation=${invitationToken}&role=manager`;

      const newLink = {
        email,
        link: fullLink,
        createdAt: new Date(),
      };

      setGeneratedLinks([newLink, ...generatedLinks]);

      toast.success('Link de invitación generado exitosamente');
      setEmail('');
    } catch (error: any) {
      toast.error(error.message || 'Error al crear la invitación');
    }
  };

  const copyToClipboard = async (link: string) => {
    try {
      await navigator.clipboard.writeText(link);
      toast.success('Link copiado al portapapeles');
    } catch (_error) {
      toast.error('Error al copiar el link');
    }
  };

  return (
    <DashboardLayout>
      <div className="space-y-8">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Panel de Administración</h1>
            <p className="text-muted-foreground">Gestiona invitaciones y usuarios de la plataforma</p>
          </div>
          <Badge variant="outline" className="border-purple-300 text-purple-700">
            <Shield className="mr-1 h-3 w-3" />
            Administrador
          </Badge>
        </div>

        {/* Create Invitation Form */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <UserPlus className="h-5 w-5" />
              Generar Link de Invitación
            </CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleCreateInvitation} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="email">Email del destinatario (Manager)</Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="manager@empresa.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>
              <Button type="submit" disabled={isLoading} className="w-full md:w-auto">
                {isLoading ? 'Generando...' : 'Generar Link de Invitación para Manager'}
              </Button>
            </form>
          </CardContent>
        </Card>

        {/* Generated Links */}
        {generatedLinks.length > 0 && (
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Link className="h-5 w-5" />
                Links Generados ({generatedLinks.length})
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {generatedLinks.map((linkData, index) => (
                  <div key={index} className="rounded-lg border p-4">
                    <div className="flex items-start justify-between gap-4">
                      <div className="min-w-0 flex-1 space-y-2">
                        <div className="flex items-center gap-2">
                          <Badge variant="outline">Manager</Badge>
                          <span className="text-sm font-medium">{linkData.email}</span>
                        </div>
                        <div className="rounded bg-gray-50 p-2">
                          <code className="text-xs break-all text-gray-600">{linkData.link}</code>
                        </div>
                        <p className="text-muted-foreground text-xs">Generado: {linkData.createdAt.toLocaleString()}</p>
                      </div>
                      <div className="flex gap-2">
                        <Button variant="outline" size="sm" onClick={() => copyToClipboard(linkData.link)}>
                          <Copy className="h-4 w-4" />
                        </Button>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        )}
      </div>
    </DashboardLayout>
  );
}
