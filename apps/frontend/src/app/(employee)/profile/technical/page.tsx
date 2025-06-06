'use client';

import React, { useEffect, useState } from 'react';

import { useRouter } from 'next/navigation';

import { Select } from '@/components/atoms/inputs/Select';
import { PageLoader } from '@/components/atoms/loaders/PageLoader';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Progress } from '@/components/ui/progress';
import { useAuth } from '@/contexts/AuthContext';
import { findSpecializedRoleId, useAvailableRolesAndAreas } from '@/hooks/useAvailableRolesAndAreas';
import { useEmployeeProfile, useEmployeeTechnologies, type EmployeeTechnology } from '@/hooks/useEmployeeProfile';
import { useProfileLoadingState } from '@/hooks/useProfileLoadingState';
import { API_BASE_URL } from '@/lib/api';

import {
  Brain,
  Cloud,
  Code,
  Cpu,
  Database,
  Download,
  Edit,
  FileJson,
  FileText,
  Plus,
  Settings,
  Star,
  Trash2,
  Upload,
  Wrench,
} from 'lucide-react';
import { toast } from 'sonner';

// Available technologies mock data
const availableTechnologies = [
  {
    id: 'f4a5b6c7-d8e9-4f0a-1b2c-3d4e5f6a7b8c',
    name: 'Go',
    categoryId: 'f3b8e8d9-0e8f-4c5c-9f9d-42a7c5e7e5a3',
    categoryName: 'Programming Languages',
  },
  {
    id: 'c5d6e7f8-a9b0-4c1d-2e3f-4a5b6c7d8e9f',
    name: 'Ruby',
    categoryId: 'f3b8e8d9-0e8f-4c5c-9f9d-42a7c5e7e5a3',
    categoryName: 'Programming Languages',
  },
  {
    id: 'b4c5d6e7-f8a9-4b0c-1d2e-3f4a5b6c7d8e',
    name: 'Swift',
    categoryId: 'f3b8e8d9-0e8f-4c5c-9f9d-42a7c5e7e5a3',
    categoryName: 'Programming Languages',
  },
  {
    id: 'a3b4c5d6-e7f8-4a9b-0c1d-2e3f4a5b6c7d',
    name: 'Java',
    categoryId: 'f3b8e8d9-0e8f-4c5c-9f9d-42a7c5e7e5a3',
    categoryName: 'Programming Languages',
  },
  {
    id: 'f2a3b4c5-d6e7-4f8a-9b0c-1d2e3f4a5b6c',
    name: 'C++',
    categoryId: 'f3b8e8d9-0e8f-4c5c-9f9d-42a7c5e7e5a3',
    categoryName: 'Programming Languages',
  },
  {
    id: 'c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f',
    name: 'JavaScript',
    categoryId: 'f3b8e8d9-0e8f-4c5c-9f9d-42a7c5e7e5a3',
    categoryName: 'Programming Languages',
  },
  {
    id: 'b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e',
    name: 'Python',
    categoryId: 'f3b8e8d9-0e8f-4c5c-9f9d-42a7c5e7e5a3',
    categoryName: 'Programming Languages',
  },
  {
    id: 'a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d',
    name: 'C#',
    categoryId: 'f3b8e8d9-0e8f-4c5c-9f9d-42a7c5e7e5a3',
    categoryName: 'Programming Languages',
  },
  {
    id: 'e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b',
    name: 'React',
    categoryId: 'a7b6c5d4-0e5f-4a3b-8c9d-1e2f3a4b5c6d',
    categoryName: 'Frameworks',
  },
  {
    id: 'd6e7f8a9-b0c1-4d2e-3f4a-5b6c7d8e9f0a',
    name: 'Node.js',
    categoryId: 'a7b6c5d4-0e5f-4a3b-8c9d-1e2f3a4b5c6d',
    categoryName: 'Frameworks',
  },
  {
    id: 'e7f8a9b0-c1d2-4e3f-4a5b-6c7d8e9f0a1b',
    name: 'Django',
    categoryId: 'a7b6c5d4-0e5f-4a3b-8c9d-1e2f3a4b5c6d',
    categoryName: 'Frameworks',
  },
  {
    id: 'f8a9b0c1-d2e3-4f4a-5b6c-7d8e9f0a1b2c',
    name: 'Flask',
    categoryId: 'a7b6c5d4-0e5f-4a3b-8c9d-1e2f3a4b5c6d',
    categoryName: 'Frameworks',
  },
  {
    id: 'a9b0c1d2-e3f4-4a5b-6c7d-8e9f0a1b2c3d',
    name: 'Spring Boot',
    categoryId: 'a7b6c5d4-0e5f-4a3b-8c9d-1e2f3a4b5c6d',
    categoryName: 'Frameworks',
  },
  {
    id: 'b0c1d2e3-f4a5-4b6c-7d8e-9f0a1b2c3d4e',
    name: 'Angular',
    categoryId: 'a7b6c5d4-0e5f-4a3b-8c9d-1e2f3a4b5c6d',
    categoryName: 'Frameworks',
  },
  {
    id: 'c1d2e3f4-a5b6-4c7d-8e9f-0a1b2c3d4e5f',
    name: 'Vue.js',
    categoryId: 'a7b6c5d4-0e5f-4a3b-8c9d-1e2f3a4b5c6d',
    categoryName: 'Frameworks',
  },
  {
    id: 'd0e1f2a3-b4c5-4d6e-7f8a-9b0c1d2e3f4a',
    name: 'AWS',
    categoryId: 'e5f6a7b8-c9d0-1e2f-3a4b-5c6d7e8f9a0b',
    categoryName: 'Cloud',
  },
  {
    id: 'e1f2a3b4-c5d6-4e7f-8a9b-0c1d2e3f4a5b',
    name: 'Azure',
    categoryId: 'e5f6a7b8-c9d0-1e2f-3a4b-5c6d7e8f9a0b',
    categoryName: 'Cloud',
  },
  {
    id: 'b8c9d0e1-f2a3-4b4c-5d6e-7f8a9b0c1d2e',
    name: 'Docker',
    categoryId: 'b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e',
    categoryName: 'DevOps',
  },
  {
    id: 'c9d0e1f2-a3b4-4c5d-6e7f-8a9b0c1d2e3f',
    name: 'Kubernetes',
    categoryId: 'b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e',
    categoryName: 'DevOps',
  },
  {
    id: 'f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c',
    name: 'SQL Server',
    categoryId: 'c1d2e3f4-a5b6-4c7d-8e9f-0a1b2c3d4e5f',
    categoryName: 'Databases',
  },
  {
    id: 'a7b8c9d0-e1f2-4a3b-4c5d-6e7f8a9b0c1d',
    name: 'MongoDB',
    categoryId: 'c1d2e3f4-a5b6-4c7d-8e9f-0a1b2c3d4e5f',
    categoryName: 'Databases',
  },
];

const sfiaLevels = [
  { value: 1, label: 'Nivel 1 - Seguir', description: 'Sigue instrucciones y aplica conocimientos básicos' },
  {
    value: 2,
    label: 'Nivel 2 - Asistir',
    description: 'Asiste en actividades y aplica conocimientos bajo supervisión',
  },
  { value: 3, label: 'Nivel 3 - Aplicar', description: 'Aplica conocimientos de manera independiente' },
  { value: 4, label: 'Nivel 4 - Habilitar', description: 'Habilita a otros y toma decisiones técnicas complejas' },
  { value: 5, label: 'Nivel 5 - Asegurar', description: 'Asegura la calidad y dirección técnica' },
  { value: 6, label: 'Nivel 6 - Iniciar', description: 'Inicia y dirige proyectos estratégicos' },
  { value: 7, label: 'Nivel 7 - Establecer', description: 'Establece estrategias y políticas organizacionales' },
];

const mbtiTypes = [
  { value: 'INTJ', label: 'INTJ - Arquitecto' },
  { value: 'INTP', label: 'INTP - Pensador' },
  { value: 'ENTJ', label: 'ENTJ - Comandante' },
  { value: 'ENTP', label: 'ENTP - Innovador' },
  { value: 'INFJ', label: 'INFJ - Abogado' },
  { value: 'INFP', label: 'INFP - Mediador' },
  { value: 'ENFJ', label: 'ENFJ - Protagonista' },
  { value: 'ENFP', label: 'ENFP - Activista' },
  { value: 'ISTJ', label: 'ISTJ - Logista' },
  { value: 'ISFJ', label: 'ISFJ - Protector' },
  { value: 'ESTJ', label: 'ESTJ - Ejecutivo' },
  { value: 'ESFJ', label: 'ESFJ - Cónsul' },
  { value: 'ISTP', label: 'ISTP - Virtuoso' },
  { value: 'ISFP', label: 'ISFP - Aventurero' },
  { value: 'ESTP', label: 'ESTP - Empresario' },
  { value: 'ESFP', label: 'ESFP - Animador' },
];

// Removed experienceLevels constant and related state

const getCategoryIcon = (categoryName: string) => {
  switch (categoryName) {
    case 'Programming Languages':
      return <Code className="h-4 w-4" />;
    case 'Frameworks':
      return <Settings className="h-4 w-4" />;
    case 'Databases':
      return <Database className="h-4 w-4" />;
    case 'Cloud':
      return <Cloud className="h-4 w-4" />;
    case 'DevOps':
      return <Wrench className="h-4 w-4" />;
    default:
      return <Cpu className="h-4 w-4" />;
  }
};

export default function TechnicalProfilePage() {
  const router = useRouter();
  const { token, user } = useAuth();

  const { profile, updateTechnicalProfile, refetch: fetchProfile, loading: profileLoading } = useEmployeeProfile();
  const {
    technologies,
    addTechnology,
    importTechnologies,
    refetch: refetchTechnologies,
    loading: technologiesLoading,
  } = useEmployeeTechnologies();

  // Usar el hook de loading state optimizado
  const loadingState = useProfileLoadingState(
    profileLoading,
    false, // languagesLoading - not used in this page
    technologiesLoading,
    false, // experiencesLoading - not used in this page
    false, // interestsLoading - not used in this page
    !!profile?.technicalProfile,
  );

  // Form states
  const [sfiaLevelGeneral, setSfiaLevelGeneral] = useState<number>(1);
  const [mbti, setMbti] = useState<string>('');

  // Technology form states
  const [selectedTechnology, setSelectedTechnology] = useState<string>('');
  const [sfiaLevel, setSfiaLevel] = useState<number>(1);
  const [yearsExperience, setYearsExperience] = useState<number>(0);
  const [version, setVersion] = useState<string>('');

  // JSON Import states
  const [jsonImportOpen, setJsonImportOpen] = useState(false);
  const [jsonInput, setJsonInput] = useState('');
  const [importPreview, setImportPreview] = useState<any[]>([]);
  const [isImporting, setIsImporting] = useState(false);

  // Edit states
  const [editingTechId, setEditingTechId] = useState<string | null>(null);
  const [editingSpecId, setEditingSpecId] = useState<string | null>(null);

  // Specialization section state using hook
  const { rolesData: availableSpecs, rolesMapping } = useAvailableRolesAndAreas();
  const [selectedSpecRole, setSelectedSpecRole] = useState('');
  const [selectedSpecArea, setSelectedSpecArea] = useState('');
  const [selectedSpecLevel, setSelectedSpecLevel] = useState('');
  const [selectedSpecYears, setSelectedSpecYears] = useState<number>(0);

  // preload existing specialization
  useEffect(() => {
    const spec = profile?.technicalProfile?.specializedRoles?.[0];
    if (spec) {
      setEditingSpecId(spec.id);
      setSelectedSpecRole(spec.roleName);
      setSelectedSpecArea(spec.technicalAreaName);
      // convertir nivel numérico a etiqueta usando availableSpecs
      const list = availableSpecs.find((s) => s.role === spec.roleName)?.levels || [];
      const label = list[spec.level - 1] || (typeof spec.level === 'string' ? spec.level : list[0]);
      setSelectedSpecLevel(label);
      setSelectedSpecYears(spec.yearsExperience);
    }
  }, [profile, availableSpecs]);

  // Calculate progress
  const calculateProgress = () => {
    let completed = 0;
    const total = 3;

    if (sfiaLevelGeneral > 1) completed++;
    if (mbti) completed++;
    if (technologies.length > 0) completed++;

    return (completed / total) * 100;
  };

  const progress = calculateProgress();

  useEffect(() => {
    if (profile?.technicalProfile) {
      setSfiaLevelGeneral(profile.technicalProfile.sfiaLevelGeneral || 1);
      setMbti(profile.technicalProfile.mbti || '');
    }
  }, [profile]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!mbti) {
      toast.error('Por favor seleccione un tipo de personalidad MBTI');
      return;
    }

    try {
      await updateTechnicalProfile({
        sfiaLevelGeneral,
        mbti,
      });

      toast.success('Perfil técnico actualizado correctamente');
      router.push('/profile/experience');
    } catch {
      toast.error('Error al actualizar perfil técnico');
    }
  };

  const handleAddTechnology = async () => {
    if (!selectedTechnology) {
      toast.error('Por favor seleccione una tecnología');
      return;
    }

    const tech = availableTechnologies.find((t) => t.id === selectedTechnology);
    if (!tech) return;

    try {
      await addTechnology({
        technologyId: selectedTechnology,
        sfiaLevel,
        yearsExperience,
        version: version || 'Latest',
      });

      // Reset form
      setSelectedTechnology('');
      setSfiaLevel(1);
      setYearsExperience(0);
      setVersion('');

      toast.success(`${tech.name} agregado correctamente`);
    } catch {
      toast.error('Error al agregar la tecnología');
    }
  };

  const handleEditTechnology = (tech: EmployeeTechnology) => {
    setSelectedTechnology(tech.technologyId);
    setSfiaLevel(tech.sfiaLevel);
    setYearsExperience(tech.yearsExperience);
    setVersion(tech.version);
    setEditingTechId(tech.id);
  };

  const handleUpdateTechnology = async () => {
    if (!editingTechId || !selectedTechnology) {
      toast.error('Por favor complete todos los campos requeridos');
      return;
    }

    const tech = availableTechnologies.find((t) => t.id === selectedTechnology);
    if (!tech) return;

    try {
      // Actualizar tecnología usando la API
      const response = await fetch(`${API_BASE_URL}/employee-technologies/${editingTechId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          technologyId: selectedTechnology,
          sfiaLevel,
          yearsExperience,
          version: version || 'Latest',
        }),
      });

      if (!response.ok) {
        throw new Error('Error al actualizar la tecnología');
      }

      await refetchTechnologies();

      // Reset form
      setSelectedTechnology('');
      setSfiaLevel(1);
      setYearsExperience(0);
      setVersion('');
      setEditingTechId(null);

      toast.success(`${tech.name} actualizada correctamente`);
    } catch {
      toast.error('Error al actualizar la tecnología');
    }
  };

  const handleDeleteTechnology = async (techId: string) => {
    try {
      const response = await fetch(`${API_BASE_URL}/employee-technologies/${techId}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error('Error al eliminar la tecnología');
      }

      await refetchTechnologies();
      toast.success('Tecnología eliminada correctamente');
    } catch {
      toast.error('Error al eliminar la tecnología');
    }
  };

  // JSON Import Functions
  const handleJsonUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (e) => {
      const jsonString = e.target?.result as string;
      handleJsonTextChange(jsonString);
    };
    reader.readAsText(file);
  };

  const parseJsonPreview = (jsonString: string) => {
    try {
      const parsed = JSON.parse(jsonString);
      if (Array.isArray(parsed)) {
        const mappedData = parsed.map((item, index) => ({ ...item, selected: true, index }));
        setImportPreview(mappedData);
      } else {
        toast.error('El JSON debe contener un array de tecnologías');
        setImportPreview([]);
      }
    } catch (_error) {
      toast.error('JSON inválido. Por favor verifica el formato.');
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
      toast.error('Seleccione al menos una tecnología para importar');
      return;
    }

    setIsImporting(true);
    try {
      // Remove the selected and index properties before importing
      const cleanedItems = selectedItems.map(({ selected: _selected, index: _index, ...item }) => item);
      await importTechnologies(cleanedItems);
      toast.success(`${selectedItems.length} tecnologías importadas correctamente`);
      setJsonImportOpen(false);
      setJsonInput('');
      setImportPreview([]);
    } catch (_error) {
      toast.error('Error al importar las tecnologías');
    } finally {
      setIsImporting(false);
    }
  };

  const generateSampleJson = () => {
    const sample = [
      {
        name: 'Go',
        category: 'Programming Languages',
        sfiaLevel: 4,
        yearsExperience: 3,
        version: '1.21',
      },
      {
        name: 'Spring Boot',
        category: 'Frameworks',
        sfiaLevel: 5,
        yearsExperience: 6,
        version: '3.1',
      },
      {
        name: 'MongoDB',
        category: 'Databases',
        sfiaLevel: 3,
        yearsExperience: 2,
        version: '6.0',
      },
      {
        name: 'Azure',
        category: 'Cloud',
        sfiaLevel: 4,
        yearsExperience: 4,
        version: '2024',
      },
      {
        name: 'Docker',
        category: 'DevOps',
        sfiaLevel: 3,
        yearsExperience: 3,
        version: '24.0',
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
    a.download = 'sample-technologies.json';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  };

  const groupedTechnologies = technologies.reduce(
    (acc, tech) => {
      const category = availableTechnologies.find((t) => t.id === tech.technologyId)?.categoryName || 'Other';
      if (!acc[category]) acc[category] = [];
      acc[category].push(tech);
      return acc;
    },
    {} as Record<string, EmployeeTechnology[]>,
  );

  // Handler to delete a specialization
  const _handleDeleteSpec = async (entityId: string) => {
    try {
      await fetch(`${API_BASE_URL}/employee-profiles/user/${user?.id}/specialized-roles/${entityId}`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${token}` },
      });
      toast.success('Especialización eliminada');
      // refresh technical profile
      await fetchProfile();
    } catch {
      toast.error('Error al eliminar especialización');
    }
  };

  // Early return for loading state
  if (!loadingState.showContent) {
    return <PageLoader title="Cargando perfil técnico..." subtitle="Obteniendo stack tecnológico y especialización" />;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100 p-4 md:p-6">
      <div className="mx-auto max-w-4xl space-y-6">
        <div className="space-y-2 text-center">
          <h1 className="text-3xl font-bold text-slate-900">Perfil Técnico</h1>
          <p className="text-slate-600">Configure su stack tecnológico y nivel de competencia</p>
          <div className="mx-auto max-w-md">
            <div className="mb-2 flex justify-between text-sm text-slate-600">
              <span>Progreso del perfil</span>
              <span>{Math.round(progress)}%</span>
            </div>
            <Progress value={progress} className="h-2" />
          </div>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          {/* SFIA Level and MBTI */}
          <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
            <Card className="relative z-50 overflow-visible border-0 bg-white/70 shadow-sm">
              <CardHeader className="pb-4">
                <div className="flex items-center gap-2">
                  <Star className="h-5 w-5 text-slate-600" />
                  <CardTitle>Nivel SFIA General</CardTitle>
                </div>
                <CardDescription>Seleccione su nivel de competencia técnica general</CardDescription>
              </CardHeader>
              <CardContent className="overflow-visible">
                <div className="space-y-2">
                  <Label>Nivel SFIA *</Label>
                  <Select
                    options={sfiaLevels.map((level) => ({
                      value: level.value,
                      label: level.label,
                      description: level.description,
                    }))}
                    value={sfiaLevels.find((level) => level.value === sfiaLevelGeneral)}
                    onChange={(selected) => setSfiaLevelGeneral((selected as any)?.value ?? 1)}
                    placeholder="Seleccionar nivel SFIA..."
                    formatOptionLabel={(option: any) =>
                      option ? (
                        <div className="flex flex-col">
                          <span className="font-medium">{option.label}</span>
                          <span className="text-xs text-slate-500">{option.description}</span>
                        </div>
                      ) : null
                    }
                    className="z-50"
                  />
                </div>
              </CardContent>
            </Card>

            <Card className="z-50 border-0 bg-white/70 shadow-sm backdrop-blur-sm">
              <CardHeader className="pb-4">
                <div className="flex items-center gap-2">
                  <Brain className="h-5 w-5 text-slate-600" />
                  <CardTitle>Tipo de Personalidad MBTI</CardTitle>
                </div>
                <CardDescription>Seleccione su tipo de personalidad según el test MBTI</CardDescription>
              </CardHeader>
              <CardContent className="overflow-visible">
                <div className="space-y-2">
                  <Label>Tipo MBTI *</Label>
                  <Select
                    options={mbtiTypes}
                    value={mbtiTypes.find((type) => type.value === mbti)}
                    onChange={(selected) => setMbti((selected as any)?.value ?? '')}
                    placeholder="Seleccionar tipo MBTI..."
                  />
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Área de Especialización */}
          {!editingSpecId ? (
            <Card className="z-50 border-0 bg-white/70 shadow-sm backdrop-blur-sm">
              <CardHeader>
                <CardTitle className="text-base">Área de Especialización</CardTitle>
                <CardDescription>Seleccione rol, área y nivel de experiencia especializado</CardDescription>
              </CardHeader>
              <CardContent className="space-y-4 overflow-visible">
                {/* formulario de nueva especialización */}
                <div>
                  <Label>Rol Especializado</Label>
                  <Select
                    placeholder="Seleccionar rol..."
                    options={availableSpecs.map((s) => ({ value: s.role, label: s.role }))}
                    value={
                      availableSpecs.find((s) => s.role === selectedSpecRole) && {
                        value: selectedSpecRole,
                        label: selectedSpecRole,
                      }
                    }
                    onChange={(opt) => {
                      const role = (opt as any)?.value || '';
                      setSelectedSpecRole(role);
                      setSelectedSpecArea('');
                      setSelectedSpecLevel('');
                    }}
                  />
                </div>
                {selectedSpecRole && (
                  <div>
                    <Label>Área</Label>
                    <Select
                      placeholder="Seleccionar área..."
                      options={
                        availableSpecs
                          .find((s) => s.role === selectedSpecRole)
                          ?.areas.map((area) => ({ value: area, label: area })) || []
                      }
                      value={selectedSpecArea ? { value: selectedSpecArea, label: selectedSpecArea } : undefined}
                      onChange={(opt) => setSelectedSpecArea((opt as any)?.value || '')}
                    />
                  </div>
                )}
                {/* Nivel y Años disponibles siempre si hay rol (nuevo) o se está editando */}
                {(selectedSpecRole || editingSpecId) && (
                  <>
                    <div>
                      <Label>Nivel de Experiencia</Label>
                      <Select
                        placeholder="Seleccionar nivel..."
                        options={
                          availableSpecs
                            .find((s) => s.role === selectedSpecRole)
                            ?.levels.map((lvl) => ({ value: lvl, label: lvl })) || []
                        }
                        value={selectedSpecLevel ? { value: selectedSpecLevel, label: selectedSpecLevel } : undefined}
                        onChange={(opt) => setSelectedSpecLevel((opt as any)?.value || '')}
                        isDisabled={!!editingSpecId}
                      />
                    </div>
                    <div>
                      <Label>Años de Experiencia</Label>
                      <Input
                        type="number"
                        min={0}
                        value={selectedSpecYears}
                        onChange={(e) => setSelectedSpecYears(Number(e.target.value))}
                        placeholder="Cantidad de años"
                      />
                    </div>
                  </>
                )}
                <div className="flex justify-end">
                  <Button
                    type="button"
                    disabled={!selectedSpecRole || !selectedSpecArea || !selectedSpecLevel || selectedSpecYears <= 0}
                    onClick={async () => {
                      try {
                        if (editingSpecId) {
                          // Update existing specialization
                          const levelIndex = ['Junior', 'Staff', 'Senior', 'Architect'].indexOf(selectedSpecLevel);
                          if (levelIndex === -1) {
                            toast.error('Nivel de experiencia inválido');
                            return;
                          }

                          const url = `${API_BASE_URL}/employee-profiles/user/${user?.id}/specialized-roles/${editingSpecId}`;

                          const response = await fetch(url, {
                            method: 'PUT',
                            headers: {
                              'Content-Type': 'application/json',
                              Authorization: `Bearer ${token}`,
                            },
                            body: JSON.stringify({
                              level: levelIndex,
                              yearsExperience: selectedSpecYears,
                            }),
                          });

                          if (!response.ok) {
                            const errorData = await response.text();
                            throw new Error(`Error ${response.status}: ${errorData}`);
                          }
                        } else {
                          // Create new specialization
                          const specializedRoleId = findSpecializedRoleId(
                            rolesMapping,
                            selectedSpecRole,
                            selectedSpecArea,
                          );

                          if (!specializedRoleId) {
                            toast.error('No se pudo encontrar el ID del rol especializado');
                            return;
                          }

                          const levelIndex = ['Junior', 'Staff', 'Senior', 'Architect'].indexOf(selectedSpecLevel);
                          if (levelIndex === -1) {
                            toast.error('Nivel de experiencia inválido');
                            return;
                          }

                          const requestBody = {
                            specializedRoleId,
                            level: levelIndex,
                            yearsExperience: selectedSpecYears,
                          };

                          const url = `${API_BASE_URL}/employee-profiles/user/${user?.id}/specialized-roles`;

                          const response = await fetch(url, {
                            method: 'POST',
                            headers: {
                              'Content-Type': 'application/json',
                              Authorization: `Bearer ${token}`,
                            },
                            body: JSON.stringify(requestBody),
                          });

                          if (!response.ok) {
                            const errorData = await response.text();
                            throw new Error(`Error ${response.status}: ${errorData}`);
                          }
                        }
                        toast.success('Especialización guardada');
                        await fetchProfile();
                      } catch (error: unknown) {
                        const errorMessage = error instanceof Error ? error.message : 'Error desconocido';
                        toast.error(`Error al guardar especialización: ${errorMessage}`);
                      }
                    }}
                  >
                    {editingSpecId ? 'Actualizar Especialización' : 'Guardar Especialización'}
                  </Button>
                </div>
              </CardContent>
            </Card>
          ) : (
            <Card className="border-0 bg-white/70 shadow-sm backdrop-blur-sm">
              <CardHeader>
                <CardTitle className="text-base">Especialización Activa</CardTitle>
                <CardDescription>Sujeto a aprobación en caso de cambio</CardDescription>
              </CardHeader>
              <CardContent className="space-y-2">
                <p>
                  <strong>Rol:</strong> {selectedSpecRole}
                </p>
                <p>
                  <strong>Área:</strong> {selectedSpecArea}
                </p>
                <p>
                  <strong>Nivel:</strong> {selectedSpecLevel}
                </p>
                <p>
                  <strong>Años de experiencia:</strong> {selectedSpecYears}
                </p>
                <Button variant="outline" disabled>
                  Aprobación pendiente
                </Button>
              </CardContent>
            </Card>
          )}

          {/* Technology Stack */}
          <Card className="border-0 bg-white/70 shadow-sm backdrop-blur-sm">
            <CardHeader className="pb-4">
              <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                <div className="flex items-center gap-2">
                  <Code className="h-5 w-5 text-slate-600" />
                  <div>
                    <CardTitle>Stack Tecnológico</CardTitle>
                    <CardDescription>Gestione sus tecnologías y niveles de experiencia</CardDescription>
                  </div>
                </div>
                <div className="flex flex-col gap-2 sm:flex-row">
                  <Button type="button" variant="outline" size="sm" onClick={() => setJsonImportOpen(true)}>
                    <Upload className="mr-2 h-4 w-4" />
                    Importar JSON
                  </Button>
                </div>
              </div>
            </CardHeader>
            <CardContent className="space-y-6">
              {/* Add/Edit Technology Form */}
              <div className="rounded-lg border bg-slate-50 p-4">
                <h4 className="mb-4 font-medium">{editingTechId ? 'Editar Tecnología' : 'Agregar Nueva Tecnología'}</h4>
                <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-5">
                  <div>
                    <Label>Tecnología *</Label>
                    <Select
                      options={availableTechnologies.map((tech) => ({
                        value: tech.id,
                        label: tech.name,
                        category: tech.categoryName,
                      }))}
                      value={
                        availableTechnologies
                          .filter((tech) => tech.id === selectedTechnology)
                          .map((tech) => ({ value: tech.id, label: tech.name }))[0]
                      }
                      onChange={(selected) => setSelectedTechnology((selected as any)?.value ?? '')}
                      placeholder="Seleccionar..."
                      formatOptionLabel={(option: any) =>
                        option ? (
                          <div className="flex items-center gap-2">
                            {getCategoryIcon(option.category)}
                            <span>{option.label}</span>
                          </div>
                        ) : null
                      }
                    />
                  </div>
                  <div>
                    <Label>Nivel SFIA</Label>
                    <Select
                      options={sfiaLevels.map((level) => ({ value: level.value, label: `Nivel ${level.value}` }))}
                      value={sfiaLevels.find((level) => level.value === sfiaLevel)}
                      onChange={(selected) => setSfiaLevel((selected as any)?.value ?? 1)}
                    />
                  </div>
                  <div>
                    <Label>Años de Experiencia</Label>
                    <Input
                      type="number"
                      min="0"
                      max="50"
                      value={yearsExperience}
                      onChange={(e) => setYearsExperience(Number(e.target.value))}
                    />
                  </div>
                  <div>
                    <Label>Versión</Label>
                    <Input
                      placeholder="ej: v18, 3.9, Latest"
                      value={version}
                      onChange={(e) => setVersion(e.target.value)}
                    />
                  </div>
                </div>
                <div className="mt-4 flex justify-end gap-2">
                  {editingTechId && (
                    <Button
                      type="button"
                      variant="outline"
                      onClick={() => {
                        setEditingTechId(null);
                        setSelectedTechnology('');
                        setSfiaLevel(1);
                        setYearsExperience(0);
                        setVersion('');
                      }}
                    >
                      Cancelar
                    </Button>
                  )}
                  <Button
                    type="button"
                    onClick={editingTechId ? handleUpdateTechnology : handleAddTechnology}
                    disabled={!selectedTechnology}
                  >
                    {editingTechId ? (
                      <>
                        <Edit className="mr-2 h-4 w-4" /> Actualizar Tecnología
                      </>
                    ) : (
                      <>
                        <Plus className="mr-2 h-4 w-4" /> Agregar Tecnología
                      </>
                    )}
                  </Button>
                </div>
              </div>

              {/* Technology List */}
              <div className="space-y-4">
                {Object.keys(groupedTechnologies).length > 0 ? (
                  Object.entries(groupedTechnologies).map(([category, techs]) => (
                    <div key={category} className="space-y-3">
                      <div className="flex items-center gap-2">
                        {getCategoryIcon(category)}
                        <h4 className="font-medium text-slate-900">{category}</h4>
                        <Badge variant="secondary">{techs.length}</Badge>
                      </div>
                      <div className="grid grid-cols-1 gap-3 md:grid-cols-2 lg:grid-cols-3">
                        {techs.map((tech) => {
                          const techInfo = availableTechnologies.find((t) => t.id === tech.technologyId);
                          return (
                            <div key={tech.id} className="rounded-lg border bg-white p-4 shadow-sm">
                              <div className="flex items-start justify-between">
                                <div className="flex-1">
                                  <h5 className="font-medium text-slate-900">{techInfo?.name}</h5>
                                  <div className="mt-1 space-y-1 text-sm text-slate-600">
                                    <div>SFIA: Nivel {tech.sfiaLevel}</div>
                                    <div>{tech.yearsExperience} años de experiencia</div>
                                    {tech.version && <div>Versión: {tech.version}</div>}
                                  </div>
                                </div>
                                <div className="flex gap-1">
                                  <Button
                                    type="button"
                                    variant="ghost"
                                    size="sm"
                                    onClick={() => handleEditTechnology(tech)}
                                  >
                                    <Edit className="h-4 w-4" />
                                  </Button>
                                  <Button
                                    type="button"
                                    variant="ghost"
                                    size="sm"
                                    onClick={() => handleDeleteTechnology(tech.id)}
                                  >
                                    <Trash2 className="h-4 w-4" />
                                  </Button>
                                </div>
                              </div>
                            </div>
                          );
                        })}
                      </div>
                    </div>
                  ))
                ) : (
                  <div className="py-12 text-center text-slate-500">
                    <Code className="mx-auto mb-4 h-12 w-12 opacity-50" />
                    <p>No hay tecnologías registradas</p>
                    <p className="text-sm">Agregue tecnologías o importe desde un archivo JSON</p>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>

          <div className="flex justify-between">
            <Button type="button" variant="outline" onClick={() => router.push('/profile')}>
              Volver
            </Button>
            <div className="flex gap-2">
              <Button type="button" variant="outline" onClick={() => router.push('/profile/general')}>
                Anterior
              </Button>
              <Button type="submit">Guardar y Continuar</Button>
            </div>
          </div>
        </form>

        {/* JSON Import Dialog */}
        <Dialog open={jsonImportOpen} onOpenChange={setJsonImportOpen}>
          <DialogContent className="mx-4 max-h-[80vh] max-w-4xl overflow-hidden sm:mx-auto">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <FileJson className="h-5 w-5" />
                Importar Tecnologías desde JSON
              </DialogTitle>
              <DialogDescription>
                Importe tecnologías desde un archivo JSON o pegue el contenido directamente
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-4">
              <div className="flex items-center gap-2">
                <Button variant="outline" size="sm" onClick={downloadSampleJson} className="flex items-center gap-2">
                  <Download className="h-4 w-4" />
                  Descargar Ejemplo
                </Button>
                <Label htmlFor="tech-json-upload" className="cursor-pointer">
                  <Button variant="outline" size="sm" asChild>
                    <span className="flex items-center gap-2">
                      <FileText className="h-4 w-4" />
                      Seleccionar Archivo JSON
                    </span>
                  </Button>
                </Label>
                <Input
                  id="tech-json-upload"
                  type="file"
                  accept=".json"
                  onChange={handleJsonUpload}
                  className="hidden"
                />
              </div>

              <div className="space-y-2">
                <Label>O pegue su JSON aquí:</Label>
                <textarea
                  className="border-input bg-background min-h-[120px] w-full rounded-md border px-3 py-2 font-mono text-sm"
                  placeholder="Pegue su JSON aquí..."
                  value={jsonInput}
                  onChange={(e) => handleJsonTextChange(e.target.value)}
                />
              </div>

              {importPreview.length > 0 && (
                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <h4 className="font-medium">Seleccionar tecnologías a importar:</h4>
                    <div className="flex gap-2">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => setImportPreview((prev) => prev.map((item) => ({ ...item, selected: true })))}
                      >
                        Seleccionar Todas
                      </Button>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => setImportPreview((prev) => prev.map((item) => ({ ...item, selected: false })))}
                      >
                        Deseleccionar Todas
                      </Button>
                    </div>
                  </div>

                  <div className="max-h-60 space-y-2 overflow-y-auto">
                    {importPreview.map((tech, index) => (
                      <div key={index} className="flex items-start gap-3 rounded-lg border p-3">
                        <Checkbox checked={tech.selected} onCheckedChange={() => toggleImportSelection(index)} />
                        <div className="flex-1">
                          <h5 className="font-medium">{tech.name}</h5>
                          <div className="text-sm text-slate-600">
                            Categoría: {tech.category} | SFIA: {tech.sfiaLevel} | Experiencia: {tech.yearsExperience}
                            años
                          </div>
                          {tech.version && <div className="text-xs text-slate-500">Versión: {tech.version}</div>}
                        </div>
                      </div>
                    ))}
                  </div>

                  <div className="flex justify-end gap-2">
                    <Button variant="outline" onClick={() => setJsonImportOpen(false)}>
                      Cancelar
                    </Button>
                    <Button
                      onClick={handleImportConfirm}
                      disabled={!importPreview.some((item) => item.selected) || isImporting}
                    >
                      {isImporting
                        ? 'Importando...'
                        : `Importar ${importPreview.filter((item) => item.selected).length} Tecnología${
                            importPreview.filter((item) => item.selected).length !== 1 ? 's' : ''
                          }`}
                    </Button>
                  </div>
                </div>
              )}
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}
