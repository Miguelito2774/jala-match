'use client';

import React, { useState } from 'react';

import { useRouter } from 'next/navigation';

import { Select } from '@/components/atoms/inputs/Select';
import { TextArea } from '@/components/atoms/inputs/TextArea';
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
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Progress } from '@/components/ui/progress';
import { PersonalInterest, usePersonalInterests } from '@/hooks/useEmployeeProfile';
import { useProfileLoadingState } from '@/hooks/useProfileLoadingState';
import { buttonStyles } from '@/lib/buttonStyles';

import { Download, Edit, Heart, Plus, Trash2, Upload } from 'lucide-react';
import { toast } from 'sonner';

const frequencyOptions = [
  { value: 'diaria', label: 'Diaria' },
  { value: 'semanal', label: 'Semanal' },
  { value: '2-3-semana', label: '2-3 veces por semana' },
  { value: '3-4-semana', label: '3-4 veces por semana' },
  { value: 'mensual', label: 'Mensual' },
  { value: 'ocasional', label: 'Ocasional' },
];

const interestLevelOptions = [
  { value: '1', label: '1 - Muy Bajo' },
  { value: '2', label: '2 - Bajo' },
  { value: '3', label: '3 - Moderado' },
  { value: '4', label: '4 - Alto' },
  { value: '5', label: '5 - Muy Alto' },
];

const commonInterests = [
  'Ajedrez',
  'Lectura',
  'Música',
  'Deportes',
  'Cocina',
  'Fotografía',
  'Jardinería',
  'Escritura',
  'Videojuegos',
  'Yoga',
  'Senderismo',
  'Ciclismo',
  'Teatro',
  'Cine',
  'Viajes',
  'Programación',
];

interface InterestFormData {
  name: string;
  sessionDurationMinutes: number;
  frequency: string;
  interestLevel: string;
}

interface JsonImportItem extends InterestFormData {
  selected: boolean;
}

export default function PersonalInterestsPage() {
  const router = useRouter();
  const { interests, loading, addInterest, updateInterest, deleteInterest } = usePersonalInterests();
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [interestToDelete, setInterestToDelete] = useState<string | null>(null);

  // Confirmation dialog for incomplete fields
  const [isConfirmDialogOpen, setIsConfirmDialogOpen] = useState(false);
  const [incompleteFieldsList, setIncompleteFieldsList] = useState<string[]>([]);

  // Form state
  const [formData, setFormData] = useState<InterestFormData>({
    name: '',
    sessionDurationMinutes: 60,
    frequency: '',
    interestLevel: '',
  });

  // Editing state
  const [editingId, setEditingId] = useState<string | null>(null);
  const [isFormOpen, setIsFormOpen] = useState(false);

  // JSON Import state
  const [jsonImportOpen, setJsonImportOpen] = useState(false);
  const [jsonInput, setJsonInput] = useState('');
  const [importPreview, setImportPreview] = useState<JsonImportItem[]>([]);
  const [isImporting, setIsImporting] = useState(false);

  // Track changes for dynamic button
  const [hasChanges, setHasChanges] = useState(false);

  // Track when interests are added/updated/deleted
  const trackChange = () => {
    setHasChanges(true);
    // Reset to false after 30 seconds
    setTimeout(() => setHasChanges(false), 30000);
  };

  // Check for incomplete fields and generate suggestions
  const getIncompleteSuggestions = () => {
    const incomplete = [];
    if (interests.length === 0) incomplete.push('al menos un interés personal');
    return incomplete;
  };

  // Handle navigation with suggestions
  const handleFinish = () => {
    const incompleteFields = getIncompleteSuggestions();
    if (incompleteFields.length > 0) {
      setIncompleteFieldsList(incompleteFields);
      setIsConfirmDialogOpen(true);
      return;
    }
    router.push('/profile/');
  };

  // Confirm navigation with incomplete fields
  const confirmFinish = () => {
    setIsConfirmDialogOpen(false);
    router.push('/profile/');
  };

  // Loading state management
  const loadingState = useProfileLoadingState(
    false, // profileLoading - not used in this page
    false, // languagesLoading - not used in this page
    false, // technologiesLoading - not used in this page
    false, // experiencesLoading - not used in this page
    loading, // interestsLoading
    interests.length > 0, // hasData
  );

  // Early return for loading state
  if (!loadingState.showContent) {
    return (
      <PageLoader
        title="Cargando intereses personales..."
        subtitle="Por favor espere mientras cargamos su información"
      />
    );
  }

  const calculateProgress = () => {
    return interests.length > 0 ? 100 : 0;
  };

  const progress = calculateProgress();

  const resetForm = () => {
    setFormData({
      name: '',
      sessionDurationMinutes: 60,
      frequency: '',
      interestLevel: '',
    });
    setEditingId(null);
    setIsFormOpen(false);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.name || !formData.frequency || !formData.interestLevel) {
      toast.error('Por favor complete todos los campos obligatorios');
      return;
    }

    try {
      if (editingId) {
        await updateInterest(editingId, {
          name: formData.name,
          sessionDurationMinutes: formData.sessionDurationMinutes,
          frequency: formData.frequency,
          interestLevel: formData.interestLevel,
        });
        toast.success('Interés actualizado correctamente');
      } else {
        await addInterest({
          name: formData.name,
          sessionDurationMinutes: formData.sessionDurationMinutes,
          frequency: formData.frequency,
          interestLevel: formData.interestLevel,
        });
        toast.success('Interés agregado correctamente');
      }
      trackChange();
      resetForm();
    } catch (_error) {
      // Error handling could be improved with proper user notification
      toast.error(editingId ? 'Error al actualizar el interés' : 'Error al agregar el interés');
    }
  };

  const handleEdit = (interest: PersonalInterest) => {
    setFormData({
      name: interest.name,
      sessionDurationMinutes: interest.sessionDurationMinutes,
      frequency: interest.frequency,
      interestLevel: interest.interestLevel,
    });
    setEditingId(interest.id);
    setIsFormOpen(true);
  };

  const handleDeleteClick = (id: string) => {
    setInterestToDelete(id);
    setIsDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (!interestToDelete) return;

    try {
      await deleteInterest(interestToDelete);
      trackChange();
      toast.success('Interés eliminado correctamente');
    } catch (_error) {
      // Error handling could be improved with proper user notification
      toast.error('Error al eliminar el interés');
    } finally {
      setIsDeleteDialogOpen(false);
      setInterestToDelete(null);
    }
  };

  const getInterestLevelLabel = (level: string) => {
    const option = interestLevelOptions.find((opt) => opt.value === level);
    return option ? option.label : level;
  };

  const getFrequencyLabel = (freq: string) => {
    const option = frequencyOptions.find((opt) => opt.value === freq);
    return option ? option.label : freq;
  };

  const formatDuration = (minutes: number) => {
    if (minutes < 60) return `${minutes} min`;
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    return remainingMinutes > 0 ? `${hours}h ${remainingMinutes}min` : `${hours}h`;
  };

  // JSON Import Functions
  const handleJsonUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (e) => {
      const content = e.target?.result as string;
      setJsonInput(content);
      parseJsonPreview(content);
    };
    reader.readAsText(file);
  };

  const parseJsonPreview = (jsonString: string) => {
    try {
      const data = JSON.parse(jsonString);
      const items = Array.isArray(data) ? data : [data];

      const preview: JsonImportItem[] = items.map((item, index) => ({
        name: item.name || `Interés ${index + 1}`,
        sessionDurationMinutes: parseInt(item.sessionDurationMinutes) || 60,
        frequency: item.frequency || '',
        interestLevel: item.interestLevel || '3',
        selected: true,
      }));

      setImportPreview(preview);
      toast.success(`${preview.length} intereses encontrados en el JSON`);
    } catch (_error) {
      toast.error('Error al procesar el JSON. Verifique el formato.');
      setImportPreview([]);
    }
  };

  const handleJsonTextChange = (value: string) => {
    setJsonInput(value);
    if (value.trim()) {
      parseJsonPreview(value);
    } else {
      setImportPreview([]);
    }
  };

  const toggleImportSelection = (index: number) => {
    setImportPreview((prev) => prev.map((item, i) => (i === index ? { ...item, selected: !item.selected } : item)));
  };

  const handleImportConfirm = async () => {
    const selectedItems = importPreview.filter((item) => item.selected);
    if (selectedItems.length === 0) {
      toast.error('Seleccione al menos un interés para importar');
      return;
    }

    setIsImporting(true);
    try {
      for (const item of selectedItems) {
        await addInterest({
          name: item.name,
          sessionDurationMinutes: item.sessionDurationMinutes,
          frequency: item.frequency,
          interestLevel: item.interestLevel,
        });
      }
      trackChange();
      toast.success(`${selectedItems.length} intereses importados correctamente`);
      setJsonImportOpen(false);
      setJsonInput('');
      setImportPreview([]);
    } catch (_error) {
      toast.error('Error al importar los intereses');
    } finally {
      setIsImporting(false);
    }
  };

  const generateSampleJson = () => {
    const sample = [
      {
        name: 'Ajedrez',
        sessionDurationMinutes: 60,
        frequency: '3-4-semana',
        interestLevel: '4',
      },
      {
        name: 'Lectura',
        sessionDurationMinutes: 90,
        frequency: 'diaria',
        interestLevel: '5',
      },
    ];
    return JSON.stringify(sample, null, 2);
  };

  const downloadSampleJson = () => {
    const sample = generateSampleJson();
    const blob = new Blob([sample], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'sample-interests.json';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100 p-4 md:p-6">
      <div className="mx-auto max-w-4xl space-y-6">
        <div className="space-y-2 text-center">
          <h1 className="text-3xl font-bold text-slate-900">Intereses Personales</h1>
          <p className="text-slate-600">Agregue sus intereses personales y hobbies</p>
          <div className="mx-auto max-w-md">
            <div className="mb-2 flex justify-between text-sm text-slate-600">
              <span>Progreso del perfil</span>
              <span>{Math.round(progress)}%</span>
            </div>
            <Progress value={progress} className="h-2" />
          </div>
        </div>

        <Card className="border-0 bg-white/70 shadow-sm backdrop-blur-sm">
          <CardHeader className="pb-4">
            <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
              <div className="flex items-center gap-2">
                <Heart className="h-5 w-5 text-slate-600" />
                <CardTitle>Mis Intereses</CardTitle>
              </div>
              <div className="flex flex-col gap-2 sm:flex-row">
                <Dialog open={jsonImportOpen} onOpenChange={setJsonImportOpen}>
                  <DialogTrigger asChild>
                    <Button variant="outline" size="sm" className={buttonStyles.utility}>
                      <Upload className="mr-2 h-4 w-4" />
                      Importar JSON
                    </Button>
                  </DialogTrigger>
                  <DialogContent className="mx-4 max-h-[80vh] max-w-2xl overflow-y-auto sm:mx-auto">
                    <DialogHeader>
                      <DialogTitle>Importar Intereses desde JSON</DialogTitle>
                      <DialogDescription>
                        Cargue un archivo JSON o pegue el contenido para importar múltiples intereses
                      </DialogDescription>
                    </DialogHeader>
                    <div className="space-y-4">
                      <div className="flex gap-2">
                        <div className="flex-1">
                          <Input
                            type="file"
                            accept=".json"
                            onChange={handleJsonUpload}
                            className="file:mr-2 file:rounded file:border-0 file:bg-slate-100 file:px-2 file:py-1 file:text-sm"
                          />
                        </div>
                        <Button variant="outline" onClick={downloadSampleJson} className={buttonStyles.utility}>
                          <Download className="mr-2 h-4 w-4" />
                          Ejemplo
                        </Button>
                      </div>
                      <div className="space-y-2">
                        <Label>O pegue el JSON aquí:</Label>
                        <TextArea
                          placeholder="Pegue su JSON aquí..."
                          value={jsonInput}
                          onChange={(e) => handleJsonTextChange(e.target.value)}
                          className="min-h-[200px] font-mono text-sm"
                        />
                      </div>
                      {importPreview.length > 0 && (
                        <div className="space-y-3">
                          <Label>Seleccione los intereses a importar:</Label>
                          <div className="max-h-[300px] space-y-2 overflow-y-auto">
                            {importPreview.map((item, index) => (
                              <div key={index} className="flex items-center space-x-3 rounded-lg border p-3">
                                <Checkbox
                                  checked={item.selected}
                                  onCheckedChange={() => toggleImportSelection(index)}
                                />
                                <div className="flex-1">
                                  <div className="font-medium">{item.name}</div>
                                  <div className="text-sm text-slate-600">
                                    {formatDuration(item.sessionDurationMinutes)} • {getFrequencyLabel(item.frequency)}{' '}
                                    • {getInterestLevelLabel(item.interestLevel)}
                                  </div>
                                </div>
                              </div>
                            ))}
                          </div>
                          <div className="flex justify-end gap-2">
                            <Button
                              variant="outline"
                              onClick={() => setJsonImportOpen(false)}
                              className={buttonStyles.outline}
                            >
                              Cancelar
                            </Button>
                            <Button
                              onClick={handleImportConfirm}
                              disabled={isImporting}
                              className={buttonStyles.primary}
                            >
                              {isImporting
                                ? 'Importando...'
                                : `Importar ${importPreview.filter((i) => i.selected).length} intereses`}
                            </Button>
                          </div>
                        </div>
                      )}
                    </div>
                  </DialogContent>
                </Dialog>

                <Dialog open={isFormOpen} onOpenChange={setIsFormOpen}>
                  <DialogTrigger asChild>
                    <Button size="sm" className={buttonStyles.secondary}>
                      <Plus className="mr-2 h-4 w-4" />
                      Agregar Interés
                    </Button>
                  </DialogTrigger>
                  <DialogContent className="mx-4 sm:mx-auto">
                    <DialogHeader>
                      <DialogTitle>{editingId ? 'Editar Interés' : 'Agregar Nuevo Interés'}</DialogTitle>
                      <DialogDescription>Complete la información del interés personal</DialogDescription>
                    </DialogHeader>
                    <div className="space-y-4">
                      <div className="space-y-2">
                        <Label htmlFor="name">Nombre del Interés *</Label>
                        <div className="space-y-3">
                          {/* Selector de intereses comunes */}
                          <div>
                            <Label className="text-sm text-slate-600">Intereses Populares</Label>
                            <div className="mt-1 flex flex-wrap gap-2">
                              {commonInterests.map((interest) => (
                                <Button
                                  key={interest}
                                  type="button"
                                  variant={formData.name === interest ? 'default' : 'outline'}
                                  size="sm"
                                  onClick={() => setFormData((prev) => ({ ...prev, name: interest }))}
                                  className="h-8 text-xs"
                                >
                                  {interest}
                                </Button>
                              ))}
                            </div>
                          </div>

                          {/* Input para interés personalizado */}
                          <div>
                            <Label htmlFor="custom-interest" className="text-sm text-slate-600">
                              O escriba un interés personalizado
                            </Label>
                            <Input
                              id="custom-interest"
                              value={formData.name}
                              onChange={(e) => setFormData((prev) => ({ ...prev, name: e.target.value }))}
                              placeholder="Escriba su interés personalizado..."
                              className="mt-1"
                            />
                          </div>
                        </div>

                        {formData.name && !commonInterests.includes(formData.name) && (
                          <div className="flex items-center gap-2 text-sm text-blue-600">
                            <Badge variant="outline" className="text-xs">
                              Personalizado
                            </Badge>
                            <span>&quot;{formData.name}&quot; será agregado como interés personalizado</span>
                          </div>
                        )}
                      </div>

                      <div className="space-y-2">
                        <Label htmlFor="duration">Duración de Sesión (minutos)</Label>
                        <Input
                          id="duration"
                          type="number"
                          min="15"
                          max="480"
                          placeholder="60"
                          value={formData.sessionDurationMinutes}
                          onChange={(e) =>
                            setFormData((prev) => ({
                              ...prev,
                              sessionDurationMinutes: parseInt(e.target.value) || 60,
                            }))
                          }
                        />
                        <p className="mt-1 text-sm text-slate-500">Tiempo típico que dedica por sesión</p>
                      </div>

                      <div className="space-y-2">
                        <Label>Frecuencia *</Label>
                        <Select
                          options={frequencyOptions}
                          value={frequencyOptions.find((opt) => opt.value === formData.frequency)}
                          onChange={(selected) =>
                            setFormData((prev) => ({
                              ...prev,
                              frequency: (selected as any)?.value ?? '',
                            }))
                          }
                          placeholder="Seleccionar frecuencia..."
                          isSearchable={true}
                        />
                      </div>

                      <div className="space-y-2">
                        <Label>Nivel de Interés (Escala Likert 1-5) *</Label>
                        <Select
                          options={interestLevelOptions}
                          value={interestLevelOptions.find((opt) => opt.value === formData.interestLevel)}
                          onChange={(selected) =>
                            setFormData((prev) => ({
                              ...prev,
                              interestLevel: (selected as any)?.value ?? '',
                            }))
                          }
                          placeholder="Seleccionar nivel..."
                          isSearchable={true}
                        />
                      </div>

                      <div className="flex justify-end gap-2 pt-4">
                        <Button type="button" variant="outline" onClick={resetForm} className={buttonStyles.outline}>
                          Cancelar
                        </Button>
                        <Button onClick={handleSubmit} className={buttonStyles.primary}>
                          {editingId ? 'Actualizar' : 'Agregar'} Interés
                        </Button>
                      </div>
                    </div>
                  </DialogContent>
                </Dialog>
              </div>
            </div>
            <CardDescription>
              {interests.length > 0 ? `${interests.length} intereses registrados` : 'No hay intereses registrados'}
            </CardDescription>
          </CardHeader>
          <CardContent>
            {loading ? (
              <div className="py-8 text-center text-slate-500">Cargando intereses...</div>
            ) : interests.length > 0 ? (
              <div className="grid gap-4 md:grid-cols-2">
                {interests.map((interest) => (
                  <div key={interest.id} className="rounded-lg border bg-slate-50 p-4">
                    <div className="mb-3 flex items-start justify-between">
                      <h3 className="text-lg font-semibold">{interest.name}</h3>
                      <div className="flex gap-1">
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => handleEdit(interest)}
                          className={buttonStyles.utility}
                        >
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => handleDeleteClick(interest.id)}
                          className={buttonStyles.danger}
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </div>
                    </div>

                    <div className="space-y-2">
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">Duración:</span>
                        <Badge variant="secondary">{formatDuration(interest.sessionDurationMinutes)}</Badge>
                      </div>
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">Frecuencia:</span>
                        <Badge variant="outline">{getFrequencyLabel(interest.frequency)}</Badge>
                      </div>
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">Nivel de Interés:</span>
                        <Badge variant={parseInt(interest.interestLevel) >= 4 ? 'default' : 'secondary'}>
                          {getInterestLevelLabel(interest.interestLevel)}
                        </Badge>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="py-12 text-center">
                <Heart className="mx-auto mb-4 h-12 w-12 text-slate-300" />
                <h3 className="mb-2 text-lg font-medium text-slate-600">No hay intereses registrados</h3>
                <p className="mb-4 text-slate-500">
                  Agregue sus intereses personales y hobbies para completar su perfil
                </p>
                <Button onClick={() => setIsFormOpen(true)} className={buttonStyles.secondary}>
                  <Plus className="mr-2 h-4 w-4" />
                  Agregar Primer Interés
                </Button>
              </div>
            )}
          </CardContent>
        </Card>

        <div className="mt-6 flex justify-between">
          <Button variant="outline" onClick={() => router.push('/profile')} className={buttonStyles.outline}>
            Volver
          </Button>
          <div className="flex gap-2">
            <Button
              variant="outline"
              onClick={() => router.push('/profile/experience')}
              className={buttonStyles.navigation}
            >
              Anterior
            </Button>
            <Button
              onClick={() => {
                handleFinish();
                if (interests.length > 0) {
                  toast.success('Perfil completado exitosamente');
                }
              }}
              className={buttonStyles.finish}
            >
              {hasChanges ? 'Guardar y Finalizar' : 'Finalizar Perfil'}
            </Button>
          </div>
        </div>
      </div>

      {/* Confirmation Dialog for Incomplete Fields */}
      <AlertDialog open={isConfirmDialogOpen} onOpenChange={setIsConfirmDialogOpen}>
        <AlertDialogContent className="fixed top-1/2 left-1/2 z-[100] w-full max-w-md -translate-x-1/2 -translate-y-1/2 rounded-lg border bg-white p-6 shadow-lg">
          <AlertDialogHeader>
            <AlertDialogTitle className="text-lg font-semibold text-gray-900">
              ¿Continuar sin completar?
            </AlertDialogTitle>
            <AlertDialogDescription className="mt-2 text-sm text-gray-600">
              Te falta completar: {incompleteFieldsList.join(', ')}. ¿Igual quieres continuar?
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter className="mt-6 flex justify-end gap-3">
            <AlertDialogCancel className={buttonStyles.outline}>Volver a completar</AlertDialogCancel>
            <AlertDialogAction onClick={confirmFinish} className={buttonStyles.primary}>
              Continuar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Delete Interest Dialog */}
      <AlertDialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <AlertDialogContent className="fixed top-1/2 left-1/2 z-[100] w-full max-w-md -translate-x-1/2 -translate-y-1/2 rounded-lg border bg-white p-6 shadow-lg">
          <AlertDialogHeader>
            <AlertDialogTitle className="text-lg font-semibold text-gray-900">¿Eliminar interés?</AlertDialogTitle>
            <AlertDialogDescription className="mt-2 text-sm text-gray-600">
              Esta acción no se puede deshacer. ¿Estás seguro de que quieres eliminar este interés?
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter className="mt-6 flex justify-end gap-3">
            <AlertDialogCancel className={buttonStyles.outline}>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} className={buttonStyles.danger}>
              Eliminar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Overlay for AlertDialogs */}
      {(isDeleteDialogOpen || isConfirmDialogOpen) && <div className="fixed inset-0 z-[99] bg-black/50" />}
    </div>
  );
}
