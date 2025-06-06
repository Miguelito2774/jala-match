'use client';

import React, { useEffect, useState } from 'react';

import { useRouter } from 'next/navigation';

import { Select } from '@/components/atoms/inputs/Select';
import { PageLoader } from '@/components/atoms/loaders/PageLoader';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Progress } from '@/components/ui/progress';
import { EmployeeLanguage, useEmployeeLanguages, useEmployeeProfile } from '@/hooks/useEmployeeProfile';
import { useProfileLoadingState } from '@/hooks/useProfileLoadingState';

import { Camera, Edit, Globe, Languages, Trash2, User } from 'lucide-react';
import { toast } from 'sonner';

const countries = [
  { value: 'Argentina', label: 'Argentina', flag: '🇦🇷' },
  { value: 'Bolivia', label: 'Bolivia', flag: '🇧🇴' },
  { value: 'Brasil', label: 'Brasil', flag: '🇧🇷' },
  { value: 'Chile', label: 'Chile', flag: '🇨🇱' },
  { value: 'Colombia', label: 'Colombia', flag: '🇨🇴' },
  { value: 'Ecuador', label: 'Ecuador', flag: '🇪🇨' },
  { value: 'Perú', label: 'Perú', flag: '🇵🇪' },
  { value: 'Uruguay', label: 'Uruguay', flag: '🇺🇾' },
  { value: 'Venezuela', label: 'Venezuela', flag: '🇻🇪' },
];

const timezones = [
  { value: 'America/La_Paz', label: 'BOT (UTC-4)', city: 'La Paz' },
  { value: 'America/Argentina/Buenos_Aires', label: 'ART (UTC-3)', city: 'Buenos Aires' },
  { value: 'America/Sao_Paulo', label: 'BRT (UTC-3)', city: 'São Paulo' },
  { value: 'America/Santiago', label: 'CLT (UTC-3)', city: 'Santiago' },
  { value: 'America/Bogota', label: 'COT (UTC-5)', city: 'Bogotá' },
];

const proficiencyLevels = [
  { value: 'A1', label: 'Principiante (A1)' },
  { value: 'A2', label: 'Básico (A2)' },
  { value: 'B1', label: 'Intermedio (B1)' },
  { value: 'B2', label: 'Intermedio Alto (B2)' },
  { value: 'C1', label: 'Avanzado (C1)' },
  { value: 'C2', label: 'Maestría (C2)' },
  { value: 'Native', label: 'Nativo' },
];

// Mapeo bidireccional: español (frontend) <-> inglés (base de datos)
const languageMapping: Record<string, string> = {
  // Español -> Inglés (para guardar en BD)
  Español: 'Spanish',
  Inglés: 'English',
  Portugués: 'Portuguese',
  Francés: 'French',
  Alemán: 'German',
  Italiano: 'Italian',
  Japonés: 'Japanese',
  Chino: 'Chinese',
  Ruso: 'Russian',
  Árabe: 'Arabic',
};

// Mapeo inverso: inglés (BD) -> español (frontend)
const reverseLanguageMapping: Record<string, string> = Object.fromEntries(
  Object.entries(languageMapping).map(([spanish, english]) => [english, spanish]),
);

const commonLanguages = [
  { value: 'Español', label: 'Español' },
  { value: 'Inglés', label: 'Inglés' },
  { value: 'Portugués', label: 'Portugués' },
  { value: 'Francés', label: 'Francés' },
  { value: 'Alemán', label: 'Alemán' },
  { value: 'Italiano', label: 'Italiano' },
  { value: 'Japonés', label: 'Japonés' },
  { value: 'Chino', label: 'Chino' },
  { value: 'Ruso', label: 'Ruso' },
  { value: 'Árabe', label: 'Árabe' },
];

export default function GeneralInfoPage() {
  const router = useRouter();
  const { profile, loading: profileLoading, updateGeneralInfo } = useEmployeeProfile();
  const { languages, loading: languagesLoading, addLanguage, updateLanguage, deleteLanguage } = useEmployeeLanguages();

  // Usar el hook de loading state optimizado
  const loadingState = useProfileLoadingState(
    profileLoading,
    languagesLoading,
    false, // technologies not needed here
    false, // experiences not needed here
    false, // interests not needed here
    !!profile,
  );

  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [country, setCountry] = useState('');
  const [timezone, setTimezone] = useState('');
  const [profilePictureUrl, setProfilePictureUrl] = useState('');

  // Language form state
  const [newLanguage, setNewLanguage] = useState('');
  const [newProficiency, setNewProficiency] = useState('');
  const [editingId, setEditingId] = useState<string | null>(null);

  // Delete dialog state
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [languageToDelete, setLanguageToDelete] = useState<string | null>(null);

  const calculateProgress = () => {
    const fields = [firstName, lastName, country, timezone];
    const filled = fields.filter((f) => f && f.trim() !== '').length;
    return (filled / fields.length) * 100;
  };

  const progress = calculateProgress();

  useEffect(() => {
    if (profile?.generalInfo) {
      const info = profile.generalInfo;
      setFirstName(info.firstName || '');
      setLastName(info.lastName || '');
      setCountry(info.country || '');
      setTimezone(info.timezone || '');
      setProfilePictureUrl(info.profilePictureUrl || '');
    }
  }, [profile]);

  const getInitials = (first: string, last: string) =>
    `${first?.charAt(0) || ''}${last?.charAt(0) || ''}`.toUpperCase();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const payload = {
      firstName,
      lastName,
      country,
      timezone,
      profilePictureUrl: profilePictureUrl || null,
    };

    try {
      if (profile) {
        await updateGeneralInfo(payload);
      }
      router.push('/profile/technical');
      router.refresh();
    } catch (_err) {
      // Error handling could be improved with proper user notification
    }
  };

  // Language functions
  const handleAddLanguage = async () => {
    if (!newLanguage || !newProficiency) {
      toast.error('Por favor complete todos los campos');
      return;
    }

    try {
      // Convertir idioma a inglés para guardar en BD
      const languageInEnglish = languageMapping[newLanguage] || newLanguage;
      await addLanguage({ language: languageInEnglish, proficiency: newProficiency });
      setNewLanguage('');
      setNewProficiency('');
      toast.success('Idioma agregado correctamente');
    } catch (_err) {
      toast.error('Error al agregar el idioma');
    }
  };

  const handleUpdateLanguage = async () => {
    if (!editingId || !newLanguage || !newProficiency) {
      toast.error('Por favor complete todos los campos');
      return;
    }

    try {
      // Convertir idioma a inglés para guardar en BD
      const languageInEnglish = languageMapping[newLanguage] || newLanguage;
      await updateLanguage(editingId, {
        language: languageInEnglish,
        proficiency: newProficiency,
      });
      setEditingId(null);
      setNewLanguage('');
      setNewProficiency('');
      toast.success('Idioma actualizado correctamente');
    } catch (_err) {
      toast.error('Error al actualizar el idioma');
    }
  };

  const handleEditLanguage = (lang: EmployeeLanguage) => {
    // Convertir idioma de inglés (BD) a español (frontend) para editar
    const languageInSpanish = reverseLanguageMapping[lang.language] || lang.language;
    setNewLanguage(languageInSpanish);
    setNewProficiency(lang.proficiency);
    setEditingId(lang.id);
  };

  const handleDeleteClick = (id: string) => {
    setLanguageToDelete(id);
    setIsDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (!languageToDelete) return;

    try {
      await deleteLanguage(languageToDelete);
      toast.success('Idioma eliminado correctamente');
    } catch (_err) {
      toast.error('Error al eliminar el idioma');
    } finally {
      setIsDeleteDialogOpen(false);
      setLanguageToDelete(null);
    }
  };

  const handleCancelEdit = () => {
    setEditingId(null);
    setNewLanguage('');
    setNewProficiency('');
  };

  const getProficiencyLabel = (value: string) => {
    const level = proficiencyLevels.find((l) => l.value === value);
    return level ? level.label : value;
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100 p-4 md:p-6">
      <div className="mx-auto max-w-4xl space-y-6">
        {/* Mostrar loader durante la carga inicial */}
        {loadingState.isInitialLoad && (
          <PageLoader title="Cargando perfil..." subtitle="Obteniendo su información personal" />
        )}

        {/* Mostrar contenido solo cuando esté listo */}
        {loadingState.showContent && (
          <>
            <div className="space-y-2 text-center">
              <h1 className="text-3xl font-bold text-slate-900">Información General</h1>
              <p className="text-slate-600">Complete su información general para continuar</p>
              <div className="mx-auto max-w-md">
                <div className="mb-2 flex justify-between text-sm text-slate-600">
                  <span>Progreso del perfil</span>
                  <span>{Math.round(progress)}%</span>
                </div>
                <Progress value={progress} className="h-2" />
              </div>
            </div>

            <form onSubmit={handleSubmit} className="space-y-6">
              <Card className="border-0 bg-white/70 shadow-sm backdrop-blur-sm">
                <CardHeader className="pb-4">
                  <div className="flex items-center gap-2">
                    <User className="h-5 w-5 text-slate-600" />
                    <CardTitle>Información Personal</CardTitle>
                  </div>
                  <CardDescription>Datos básicos de identificación</CardDescription>
                </CardHeader>
                <CardContent className="space-y-6">
                  <div className="flex items-center gap-6">
                    <div className="relative">
                      <Avatar className="h-20 w-20 border-4 border-white shadow-lg">
                        <AvatarImage src={profilePictureUrl} />
                        <AvatarFallback className="bg-gradient-to-br from-blue-500 to-purple-600 text-lg font-semibold text-white">
                          {getInitials(firstName, lastName)}
                        </AvatarFallback>
                      </Avatar>
                      <Button
                        type="button"
                        size="sm"
                        variant="outline"
                        className="absolute -right-2 -bottom-2 h-8 w-8 rounded-full bg-white p-0 shadow-sm"
                      >
                        <Camera className="h-4 w-4" />
                      </Button>
                    </div>
                    <div className="flex-1">
                      <Label htmlFor="profilePicture">URL de Foto de Perfil</Label>
                      <Input
                        id="profilePicture"
                        type="url"
                        placeholder="https://ejemplo.com/mi-foto.jpg"
                        value={profilePictureUrl}
                        onChange={(e) => setProfilePictureUrl(e.target.value)}
                        className="mt-1"
                      />
                    </div>
                  </div>

                  <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                    <div>
                      <Label htmlFor="firstName">Nombre *</Label>
                      <Input
                        id="firstName"
                        required
                        placeholder="Ingrese su nombre"
                        value={firstName}
                        onChange={(e) => setFirstName(e.target.value)}
                      />
                    </div>
                    <div>
                      <Label htmlFor="lastName">Apellido *</Label>
                      <Input
                        id="lastName"
                        required
                        placeholder="Ingrese su apellido"
                        value={lastName}
                        onChange={(e) => setLastName(e.target.value)}
                      />
                    </div>
                  </div>
                </CardContent>
              </Card>

              <Card className="overflow-visible border-0 bg-white/70 shadow-sm backdrop-blur-sm">
                <CardHeader className="pb-4">
                  <div className="flex items-center gap-2">
                    <Globe className="h-5 w-5 text-slate-600" />
                    <CardTitle>Ubicación y Zona Horaria</CardTitle>
                  </div>
                  <CardDescription>Seleccione su país y zona horaria</CardDescription>
                </CardHeader>
                <CardContent className="grid gap-6 overflow-visible md:grid-cols-2">
                  <div className="space-y-2">
                    <Label>País *</Label>
                    <Select
                      options={countries}
                      value={countries.find((opt) => opt.value === country)}
                      onChange={(selected) => setCountry((selected as any)?.value ?? '')}
                      placeholder="Seleccionar país..."
                      formatOptionLabel={(option: any) =>
                        option ? (
                          <div className="flex items-center gap-2">
                            <span>{option?.flag}</span>
                            <span>{option?.label}</span>
                          </div>
                        ) : null
                      }
                    />
                  </div>
                  <div className="space-y-2">
                    <Label>Zona Horaria *</Label>
                    <Select
                      options={timezones}
                      value={timezones.find((opt) => opt.value === timezone)}
                      onChange={(selected) => setTimezone((selected as any)?.value ?? '')}
                      placeholder="Seleccionar zona horaria..."
                      formatOptionLabel={(option: any) =>
                        option ? (
                          <div className="flex flex-col">
                            <span>{option?.label}</span>
                            <span className="text-xs text-slate-500">{option?.city}</span>
                          </div>
                        ) : null
                      }
                    />
                  </div>
                </CardContent>
              </Card>

              {/* Enhanced Languages Section */}
              <Card className="border-0 bg-white/70 shadow-sm backdrop-blur-sm">
                <CardHeader className="pb-4">
                  <div className="flex items-center gap-2">
                    <Languages className="h-5 w-5 text-slate-600" />
                    <CardTitle>Idiomas</CardTitle>
                  </div>
                  <CardDescription>Agregue sus idiomas y nivel de competencia</CardDescription>
                </CardHeader>
                <CardContent className="space-y-4">
                  {languages.length > 0 ? (
                    <div className="space-y-3">
                      {languages.map((lang) => (
                        <div
                          key={lang.id}
                          className="flex items-center justify-between rounded-lg border bg-slate-50 p-3"
                        >
                          <div>
                            <span className="font-medium">{lang.language}</span>
                            <span className="text-slate-600"> - {getProficiencyLabel(lang.proficiency)}</span>
                          </div>
                          <div className="flex gap-1">
                            <Button type="button" variant="ghost" size="sm" onClick={() => handleEditLanguage(lang)}>
                              <Edit className="h-4 w-4" />
                            </Button>
                            <Button type="button" variant="ghost" size="sm" onClick={() => handleDeleteClick(lang.id)}>
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          </div>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <p className="py-3 text-center text-slate-500">
                      {languagesLoading ? 'Cargando idiomas...' : 'No hay idiomas registrados'}
                    </p>
                  )}

                  <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                    <div>
                      <Label>Idioma</Label>
                      <Select
                        options={commonLanguages}
                        value={commonLanguages.find((opt) => opt.value === newLanguage)}
                        onChange={(selected) => setNewLanguage((selected as any)?.value ?? '')}
                        placeholder="Seleccionar idioma..."
                      />
                    </div>
                    <div>
                      <Label>Nivel de Competencia</Label>
                      <Select
                        options={proficiencyLevels}
                        value={proficiencyLevels.find((opt) => opt.value === newProficiency)}
                        onChange={(selected) => setNewProficiency((selected as any)?.value ?? '')}
                        placeholder="Seleccionar nivel..."
                      />
                    </div>
                  </div>

                  <div className="flex justify-end gap-2">
                    {editingId && (
                      <Button type="button" variant="outline" onClick={handleCancelEdit}>
                        Cancelar
                      </Button>
                    )}
                    <Button
                      type="button"
                      onClick={editingId ? handleUpdateLanguage : handleAddLanguage}
                      disabled={!newLanguage || !newProficiency || languagesLoading}
                    >
                      {editingId ? 'Actualizar Idioma' : 'Agregar Idioma'}
                    </Button>
                  </div>
                </CardContent>
              </Card>

              <div className="flex justify-between">
                <Button type="button" variant="outline" onClick={() => router.push('/profile')}>
                  Volver
                </Button>
                <Button type="submit" className="px-6">
                  Guardar y Continuar
                </Button>
              </div>
            </form>
          </>
        )}
      </div>

      <AlertDialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <AlertDialogContent className="fixed top-1/2 left-1/2 z-[100] w-full max-w-md -translate-x-1/2 -translate-y-1/2 rounded-lg border bg-white p-6 shadow-lg">
          <AlertDialogHeader>
            <AlertDialogTitle className="text-lg font-semibold text-gray-900">¿Eliminar idioma?</AlertDialogTitle>
            <AlertDialogDescription className="mt-2 text-sm text-gray-600">
              Esta acción no se puede deshacer. ¿Estás seguro de que quieres eliminar este idioma?
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter className="mt-6 flex justify-end gap-3">
            <AlertDialogCancel className="rounded-md border border-gray-300 px-4 py-2 text-gray-700 hover:bg-gray-50">
              Cancelar
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDelete}
              className="rounded-md bg-red-600 px-4 py-2 text-white hover:bg-red-700 focus:ring-2 focus:ring-red-600 focus:ring-offset-2"
            >
              Eliminar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Overlay for AlertDialog */}
      {isDeleteDialogOpen && <div className="fixed inset-0 z-[99] bg-black/50" />}
    </div>
  );
}
