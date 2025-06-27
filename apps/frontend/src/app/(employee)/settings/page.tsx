'use client';

import { useState } from 'react';

import { Switch } from '@/components/atoms/inputs/Switch';
import { DashboardLayout } from '@/components/templates/DashboardLayout';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from '@/components/ui/alert-dialog';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useAuth } from '@/contexts/AuthContext';
import { usePrivacySettings } from '@/hooks/usePrivacySettings';

import { AlertTriangle, Download, FileText, Info, Shield, Trash2 } from 'lucide-react';

interface ConsentSettings {
  teamMatchingAnalysis: boolean;
  lastUpdated: string;
  version: string;
}

export default function SettingsPage() {
  const { user } = useAuth();
  const { consentSettings, loading, updateConsentSettings, exportUserData, requestDataDeletion } = usePrivacySettings();

  const [showConsentHistory, setShowConsentHistory] = useState(false);
  const [showDataDeletionDialog, setShowDataDeletionDialog] = useState(false);
  const [isRequestingDeletion, setIsRequestingDeletion] = useState(false);

  const handleConsentChange = async (key: keyof ConsentSettings, value: boolean) => {
    await updateConsentSettings(key, value);
  };

  const handleDataExportRequest = async () => {
    await exportUserData();
  };

  const handleDataDeletionRequest = async () => {
    setIsRequestingDeletion(true);
    try {
      const result = await requestDataDeletion(['profile', 'technologies', 'experiences', 'interests', 'languages']);

      if (result) {
        setShowDataDeletionDialog(false);
      }
    } finally {
      setIsRequestingDeletion(false);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  if (!user) {
    return null;
  }

  if (loading) {
    return (
      <DashboardLayout>
        <div className="flex min-h-96 items-center justify-center">
          <div className="text-center">
            <div className="mx-auto h-8 w-8 animate-spin rounded-full border-b-2 border-blue-600"></div>
            <p className="mt-2 text-sm text-gray-600">Cargando configuración de privacidad...</p>
          </div>
        </div>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout>
      <div className="space-y-8">
        <div className="border-b border-gray-200 pb-4">
          <h1 className="text-2xl font-bold text-gray-900">Configuración de Privacidad</h1>
          <p className="mt-1 text-sm text-gray-500">Gestiona tu privacidad y el uso de tus datos personales</p>
        </div>

        <Tabs defaultValue="consents" className="space-y-6">
          <TabsList className="grid w-full grid-cols-3">
            <TabsTrigger value="consents">Consentimientos</TabsTrigger>
            <TabsTrigger value="data-export">Exportar Datos</TabsTrigger>
            <TabsTrigger value="data-deletion">Reiniciar Perfil</TabsTrigger>
          </TabsList>

          <TabsContent value="consents" className="space-y-6">
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <div>
                    <CardTitle className="flex items-center gap-2">
                      <Shield className="h-5 w-5 text-blue-600" />
                      Gestión de Consentimientos
                    </CardTitle>
                    <CardDescription>Controla cómo se utilizan tus datos personales</CardDescription>
                  </div>
                  <Dialog open={showConsentHistory} onOpenChange={setShowConsentHistory}>
                    <DialogTrigger asChild>
                      <Button variant="outline" size="sm">
                        <FileText className="mr-2 h-4 w-4" />
                        Historial
                      </Button>
                    </DialogTrigger>
                    <DialogContent>
                      <DialogHeader>
                        <DialogTitle>Historial de Consentimientos</DialogTitle>
                        <DialogDescription>Registro de cambios en tus preferencias de privacidad</DialogDescription>
                      </DialogHeader>
                      <div className="space-y-4">
                        <div className="text-sm text-gray-600">
                          <p>
                            <strong>Última actualización:</strong> {formatDate(consentSettings.lastUpdated)}
                          </p>
                          <p>
                            <strong>Versión:</strong> {consentSettings.version}
                          </p>
                        </div>
                      </div>
                    </DialogContent>
                  </Dialog>
                </div>
              </CardHeader>
              <CardContent className="space-y-6">
                <div className="space-y-4">
                  <div className="flex items-center justify-between rounded-lg border p-4">
                    <div className="flex-1">
                      <h3 className="font-medium">Análisis para Formación de Equipos</h3>
                      <p className="text-sm text-gray-600">
                        Permite que el sistema de IA te incluya en sugerencias automáticas de equipos y análisis de
                        compatibilidad
                      </p>
                    </div>
                    <Switch
                      checked={consentSettings.teamMatchingAnalysis}
                      onCheckedChange={(checked: boolean) => handleConsentChange('teamMatchingAnalysis', checked)}
                    />
                  </div>
                </div>

                <div className="flex items-center gap-2 rounded-lg bg-blue-50 p-4">
                  <Info className="h-4 w-4 text-blue-600" />
                  <p className="text-sm text-blue-800">
                    Puedes cambiar estos consentimientos en cualquier momento. Los cambios se aplicarán inmediatamente a
                    futuros procesamientos.
                  </p>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="data-export" className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Download className="h-5 w-5 text-green-600" />
                  Exportación de Datos
                </CardTitle>
                <CardDescription>Descarga una copia de todos tus datos personales</CardDescription>
              </CardHeader>
              <CardContent className="space-y-6">
                <div className="flex items-center justify-between rounded-lg border p-4">
                  <div className="flex-1">
                    <h3 className="font-medium">Descargar Mis Datos</h3>
                    <p className="text-sm text-gray-600">
                      Incluye perfil, tecnologías, experiencias, intereses y participación en equipos
                    </p>
                  </div>
                  <Button onClick={handleDataExportRequest}>
                    <Download className="mr-2 h-4 w-4" />
                    Descargar Datos
                  </Button>
                </div>

                <div className="flex items-center gap-2 rounded-lg bg-green-50 p-4">
                  <Info className="h-4 w-4 text-green-600" />
                  <p className="text-sm text-green-800">
                    La descarga incluye todos tus datos en formato JSON legible. Es una descarga directa e inmediata.
                  </p>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="data-deletion" className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Trash2 className="h-5 w-5 text-orange-600" />
                  Reiniciar Perfil
                </CardTitle>
                <CardDescription>Limpia toda tu información de perfil para empezar de nuevo</CardDescription>
              </CardHeader>
              <CardContent className="space-y-6">
                <div className="flex items-center gap-2 rounded-lg bg-orange-50 p-4">
                  <AlertTriangle className="h-4 w-4 text-orange-600" />
                  <p className="text-sm text-orange-800">
                    <strong>Atención:</strong> Esta acción limpiará toda tu información de perfil, pero podrás volver a
                    completarla cuando quieras.
                  </p>
                </div>

                <div className="flex items-center justify-between rounded-lg border p-4">
                  <div className="flex-1">
                    <h3 className="font-medium">Reiniciar Perfil Completo</h3>
                    <p className="text-sm text-gray-600">Limpia toda tu información para comenzar de nuevo</p>
                  </div>
                  <AlertDialog open={showDataDeletionDialog} onOpenChange={setShowDataDeletionDialog}>
                    <AlertDialogTrigger asChild>
                      <Button className="bg-red-600 text-white hover:bg-red-700 focus:ring-red-500">
                        <Trash2 className="mr-2 h-4 w-4" />
                        Reiniciar Perfil
                      </Button>
                    </AlertDialogTrigger>
                    <AlertDialogContent className="fixed top-1/2 left-1/2 z-[100] w-full max-w-md -translate-x-1/2 -translate-y-1/2 rounded-lg border bg-white p-6 shadow-lg">
                      <AlertDialogHeader>
                        <AlertDialogTitle className="text-lg font-semibold text-gray-900">
                          ¿Reiniciar tu perfil?
                        </AlertDialogTitle>
                        <AlertDialogDescription className="mt-2 text-sm text-gray-600">
                          Esta acción limpiará la siguiente información de tu perfil:
                        </AlertDialogDescription>
                        <div className="mt-2">
                          <ul className="list-inside list-disc space-y-1 text-sm text-gray-600">
                            <li>Información de perfil profesional</li>
                            <li>Tecnologías y niveles de experiencia</li>
                            <li>Idiomas y niveles de competencia</li>
                            <li>Experiencias laborales</li>
                            <li>Intereses personales</li>
                          </ul>
                          <p className="mt-3 text-sm font-medium text-gray-600">
                            Tu perfil volverá al estado inicial y podrás completarlo nuevamente cuando desees. Esta
                            acción es <strong>inmediata y reversible</strong>.
                          </p>
                        </div>
                      </AlertDialogHeader>
                      <AlertDialogFooter className="mt-6 flex justify-end gap-3">
                        <AlertDialogCancel className="rounded-md border border-gray-300 px-4 py-2 text-gray-700 hover:bg-gray-50">
                          Cancelar
                        </AlertDialogCancel>
                        <AlertDialogAction
                          onClick={handleDataDeletionRequest}
                          disabled={isRequestingDeletion}
                          className="rounded-md bg-red-600 px-4 py-2 text-white hover:bg-red-700 focus:ring-2 focus:ring-red-600 focus:ring-offset-2"
                        >
                          {isRequestingDeletion ? 'Reiniciando...' : 'Confirmar Reinicio'}
                        </AlertDialogAction>
                      </AlertDialogFooter>
                    </AlertDialogContent>
                  </AlertDialog>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </div>
    </DashboardLayout>
  );
}
