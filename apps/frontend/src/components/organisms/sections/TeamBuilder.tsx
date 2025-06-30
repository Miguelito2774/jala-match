'use client';

import { useEffect, useState } from 'react';

import { Button } from '@/components/atoms/buttons/Button';
import { Select } from '@/components/atoms/inputs/Select';
import { Badge } from '@/components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Slider } from '@/components/ui/slider';
import { useAuth } from '@/contexts/AuthContext';
import { useRoles } from '@/hooks/useRoles';
import { useTeamGenerator } from '@/hooks/useTeamGenerator';
import { TeamWeights } from '@/hooks/useTeams';
import { useTechnologies } from '@/hooks/useTechnologies';
import { useWeightCriteria } from '@/hooks/useWeigthCriteria';

import { Loader2 } from 'lucide-react';

import { TeamResultsPage } from './TeamResultsPage';

interface SelectOption {
  value: string | number;
  label: string;
}

export const TeamBuilder = () => {
  const { roles, loading: loadingRoles } = useRoles();
  const { technologies, loading: loadingTech } = useTechnologies();
  const { criteria, loading: loadingCriteria } = useWeightCriteria();
  const { generateTeam, loading: generatingTeam, generatedTeam } = useTeamGenerator();

  const [teamName, setTeamName] = useState<string>('');
  const [teamSize, setTeamSize] = useState<number>(3);
  const [teamRoles, setTeamRoles] = useState<{ role: string; area: string; level: string }[]>(
    Array(teamSize)
      .fill(null)
      .map(() => ({
        role: '',
        area: '',
        level: '',
      })),
  );
  const [selectedTechnologies, setSelectedTechnologies] = useState<string[]>([]);
  const [sfiaLevel, setSfiaLevel] = useState<number>(3);
  const [weights, setWeights] = useState<Record<string, number>>({});
  const [showWeightsDialog, setShowWeightsDialog] = useState(false);
  const [showResults, setShowResults] = useState(false);

  const [selectedCategory, setSelectedCategory] = useState<string>('');
  const [categoryOptions, setCategoryOptions] = useState<SelectOption[]>([]);
  const [availableTechOptions, setAvailableTechOptions] = useState<SelectOption[]>([]);
  const { user } = useAuth();

  const creatorId = user!.id;

  useEffect(() => {
    if (criteria.length > 0) {
      const initialWeights = criteria.reduce(
        (acc, criterion) => {
          acc[criterion.id] = criterion.defaultValue;
          return acc;
        },
        {} as Record<string, number>,
      );
      setWeights(initialWeights);
    }
  }, [criteria]);

  useEffect(() => {
    if (roles.length > 0) {
      const initialRoles = Array(teamSize)
        .fill(null)
        .map(() => ({
          role: roles[0]?.role || '',
          area: roles[0]?.areas?.[0] || '',
          level: roles[0]?.levels?.[0] || '',
        }));
      setTeamRoles(initialRoles);
    } else if (!loadingRoles && roles.length === 0) {
      const emptyRoles = Array(teamSize)
        .fill(null)
        .map(() => ({
          role: '',
          area: '',
          level: '',
        }));
      setTeamRoles(emptyRoles);
    }
  }, [teamSize, roles, loadingRoles]);

  useEffect(() => {
    if (technologies.length > 0) {
      const uniqueCategories = Array.from(new Set(technologies.map((tech) => tech.categoryName)));
      const catOptions = uniqueCategories.map((category) => ({
        value: category,
        label: category,
      }));
      setCategoryOptions(catOptions);
    }
  }, [technologies]);

  useEffect(() => {
    if (!selectedCategory) {
      const allTechs = technologies.map((tech) => ({
        value: tech.name,
        label: tech.name,
      }));
      setAvailableTechOptions(allTechs);
    } else {
      const techsInCategory = technologies
        .filter((tech) => tech.categoryName === selectedCategory)
        .map((tech) => ({
          value: tech.name,
          label: tech.name,
        }));
      setAvailableTechOptions(techsInCategory);
    }
  }, [selectedCategory, technologies]);

  const totalWeight = Object.values(weights).reduce((sum, weight) => sum + weight, 0);

  const handleTeamSizeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const size = parseInt(e.target.value, 10);
    if (!isNaN(size) && size > 0) {
      setTeamSize(size);
      if (size > teamRoles.length) {
        const defaultRole = roles.length > 0 ? roles[0]?.role || '' : '';
        const defaultLevel = roles.length > 0 ? roles[0]?.levels?.[0] || '' : '';
        const newRoles = [...teamRoles];
        for (let i = teamRoles.length; i < size; i++) {
          newRoles.push({ role: defaultRole, area: '', level: defaultLevel });
        }
        setTeamRoles(newRoles);
      } else {
        setTeamRoles(teamRoles.slice(0, size));
      }
    }
  };

  const updateRoleAtIndex = (index: number, field: 'role' | 'area' | 'level', value: string) => {
    const updatedRoles = [...teamRoles];
    updatedRoles[index] = { ...updatedRoles[index], [field]: value };
    setTeamRoles(updatedRoles);
  };

  const updateWeight = (id: string, value: number) => {
    const totalOtherWeights = Object.entries(weights)
      .filter(([key]) => key !== id)
      .reduce((sum, [_, val]) => sum + val, 0);
    const maxAllowed = 100 - totalOtherWeights;
    const newValue = Math.min(value, maxAllowed);
    setWeights({
      ...weights,
      [id]: newValue,
    });
  };

  const resetToDefaultWeights = () => {
    if (criteria.length > 0) {
      const defaultWeights = criteria.reduce(
        (acc, criterion) => {
          acc[criterion.id] = criterion.defaultValue;
          return acc;
        },
        {} as Record<string, number>,
      );
      setWeights(defaultWeights);
    }
  };

  const handleTechSelectionChange = (selected: SelectOption | SelectOption[] | null) => {
    const selectedOptions = Array.isArray(selected) ? selected : selected ? [selected] : [];

    const techNames = selectedOptions.map((option) => option.value.toString());

    setSelectedTechnologies(techNames);
  };

  const handleGenerateTeams = async () => {
    if (!user) {
      return;
    }

    const params = {
      CreatorId: creatorId,
      TeamSize: teamSize,
      Requirements: teamRoles.map(({ role, area, level }) => ({
        Role: role,
        Area: area,
        Level: level,
      })),
      Technologies: selectedTechnologies,
      SfiaLevel: sfiaLevel,
      Weights: {
        SfiaWeight: weights.sfiaWeight || 0,
        TechnicalWeight: weights.technicalWeight || 0,
        PsychologicalWeight: weights.psychologicalWeight || 0,
        ExperienceWeight: weights.experienceWeight || 0,
        LanguageWeight: weights.languageWeight || 0,
        InterestsWeight: weights.interestsWeight || 0,
        TimezoneWeight: weights.timezoneWeight || 0,
      },
      Availability: true,
    };

    const result = await generateTeam(params);
    if (result) {
      setShowResults(true);
    }
  };

  const handleTeamCreationSuccess = () => {
    setShowResults(false);
  };

  const isFormValid = totalWeight === 100 && selectedTechnologies.length > 0;

  const formattedWeights: TeamWeights = {
    sfiaWeight: weights.sfiaWeight || 0,
    technicalWeight: weights.technicalWeight || 0,
    psychologicalWeight: weights.psychologicalWeight || 0,
    experienceWeight: weights.experienceWeight || 0,
    languageWeight: weights.languageWeight || 0,
    interestsWeight: weights.interestsWeight || 0,
    timezoneWeight: weights.timezoneWeight || 0,
  };

  if (loadingRoles || loadingTech || loadingCriteria) {
    return (
      <div className="flex items-center justify-center p-8">
        <Loader2 className="text-secondary h-8 w-8 animate-spin" />
        <span className="ml-2 text-gray-600">Cargando configuración...</span>
      </div>
    );
  }

  if (showResults && generatedTeam) {
    return (
      <TeamResultsPage
        teamData={generatedTeam}
        formData={{
          creatorId: creatorId,
          teamName: teamName || 'Equipo Generado',
          requiredTechnologies: selectedTechnologies,
          weights: formattedWeights,
        }}
        onBack={() => setShowResults(false)}
        onSuccess={handleTeamCreationSuccess}
      />
    );
  }

  return (
    <div className="space-y-8">
      <div className="rounded-lg bg-white p-6 shadow">
        <div className="space-y-6">
          <div>
            <h2 className="mb-4 text-lg font-medium text-gray-900">Requisitos del Equipo</h2>
            <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
              {/* Team Name Field */}
              <div>
                <label className="mb-1 block text-sm font-medium text-gray-700">Nombre del Equipo</label>
                <Input
                  type="text"
                  value={teamName}
                  onChange={(e) => setTeamName(e.target.value)}
                  placeholder="Ingrese el nombre del equipo"
                  className="w-full"
                />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium text-gray-700">Tamaño del Equipo</label>
                <Input type="number" min={1} value={teamSize} onChange={handleTeamSizeChange} className="w-full" />
              </div>

              <div className="grid grid-cols-1 gap-4 md:col-span-2 md:grid-cols-2">
                <div>
                  <label className="mb-1 block text-sm font-medium text-gray-700">Categoría de Tecnología</label>
                  <Select
                    options={categoryOptions}
                    value={categoryOptions.find((opt) => opt.value === selectedCategory)}
                    onChange={(selected) => {
                      if (selected && !Array.isArray(selected)) {
                        setSelectedCategory(selected.value.toString());
                        setSelectedTechnologies([]);
                      } else {
                        setSelectedCategory('');
                        setSelectedTechnologies([]);
                      }
                    }}
                    placeholder="Seleccionar categoría..."
                  />
                </div>
                <div>
                  <label className="mb-1 block text-sm font-medium text-gray-700">Tecnologías</label>
                  <Select
                    options={availableTechOptions}
                    isMulti
                    value={availableTechOptions.filter((opt) => selectedTechnologies.includes(opt.value.toString()))}
                    onChange={handleTechSelectionChange}
                    placeholder="Seleccionar tecnologías..."
                  />
                </div>
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium text-gray-700">Nivel SFIA Mínimo</label>
                <Select
                  options={[
                    { value: 1, label: 'SFIA 1 - Básico' },
                    { value: 2, label: 'SFIA 2 - Principiante' },
                    { value: 3, label: 'SFIA 3 - Intermedio' },
                    { value: 4, label: 'SFIA 4 - Avanzado' },
                    { value: 5, label: 'SFIA 5 - Experto' },
                    { value: 6, label: 'SFIA 6 - Líder' },
                    { value: 7, label: 'SFIA 7 - Estratega' },
                  ]}
                  onChange={(selected) => {
                    if (selected && !Array.isArray(selected)) {
                      setSfiaLevel(selected.value as number);
                    }
                  }}
                  defaultValue={{ value: 3, label: 'SFIA 3 - Intermedio' }}
                />
              </div>

              <div className="flex items-end">
                <Button onClick={() => setShowWeightsDialog(true)} variant="secondary" className="w-full">
                  Configurar Criterios de Compatibilidad ({totalWeight}%)
                </Button>
              </div>
            </div>
          </div>
          <div className="border-t border-gray-200 pt-6">
            <h2 className="mb-4 text-lg font-medium text-gray-900">Roles y Niveles Requeridos</h2>
            <div className="space-y-4">
              {teamRoles.map((roleObj, index) => {
                const roleOptions = roles.map((r) => ({
                  value: r.role || '',
                  label: r.role || 'Sin nombre',
                }));

                const selectedRole = roles.find((r) => r.role === roleObj.role) || roles[0];
                const areaOptions = selectedRole?.areas?.map((a) => ({ value: a, label: a })) || [];
                const levelOptions = selectedRole?.levels?.map((l) => ({ value: l, label: l })) || [];

                return (
                  <div
                    key={index}
                    className={`grid grid-cols-1 gap-2 md:grid-cols-3 md:gap-4 ${index < teamRoles.length - 1 ? 'mb-4 border-b border-gray-200 pb-4' : ''}`}
                  >
                    <div className="space-y-1">
                      <label className="text-xs font-medium text-gray-500 md:hidden">Rol</label>
                      <Select
                        options={roleOptions}
                        value={roleOptions.find((opt) => opt.value === roleObj.role) || undefined}
                        onChange={(selected) => {
                          if (selected && !Array.isArray(selected)) {
                            updateRoleAtIndex(index, 'role', selected.value.toString());
                          }
                        }}
                        placeholder={roles.length > 0 ? 'Seleccionar rol...' : 'Cargando roles...'}
                        isDisabled={roles.length === 0}
                      />
                    </div>

                    <div className="space-y-1">
                      <label className="text-xs font-medium text-gray-500 md:hidden">Área</label>
                      <Select
                        options={areaOptions}
                        value={areaOptions.find((opt) => opt.value === roleObj.area)}
                        onChange={(selected) => {
                          if (selected && !Array.isArray(selected)) {
                            updateRoleAtIndex(index, 'area', selected.value.toString());
                          }
                        }}
                        placeholder="Seleccionar área..."
                      />
                    </div>

                    <div className="space-y-1">
                      <label className="text-xs font-medium text-gray-500 md:hidden">Nivel</label>
                      <Select
                        options={levelOptions}
                        value={levelOptions.find((opt) => opt.value === roleObj.level)}
                        onChange={(selected) => {
                          if (selected && !Array.isArray(selected)) {
                            updateRoleAtIndex(index, 'level', selected.value.toString());
                          }
                        }}
                        placeholder="Seleccionar nivel..."
                      />
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
          <Button
            onClick={handleGenerateTeams}
            className="flex items-center bg-blue-600 hover:bg-blue-700"
            disabled={generatingTeam || !isFormValid}
          >
            {generatingTeam ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Buscando a los mejores candidatos...
              </>
            ) : (
              'Generar Equipos con IA'
            )}
          </Button>
        </div>
      </div>

      <Dialog open={showWeightsDialog} onOpenChange={setShowWeightsDialog}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Configurar Criterios de Compatibilidad</DialogTitle>
          </DialogHeader>
          <div className="py-4">
            {criteria.map((criterion, index) => (
              <div key={`criterion-${criterion.id || criterion.name || index}`} className="mb-6">
                <div className="mb-2 flex items-center justify-between">
                  <label className="text-sm font-medium text-gray-700">{criterion.name}</label>
                  <Badge variant={totalWeight === 100 ? 'success' : 'secondary'}>{weights[criterion.id] || 0}%</Badge>
                </div>
                <Slider
                  value={[weights[criterion.id] || 0]}
                  min={0}
                  max={100}
                  step={1}
                  onValueChange={(value) => updateWeight(criterion.id, value[0])}
                  className="w-full"
                />
              </div>
            ))}
            <div className={`text-center font-medium ${totalWeight === 100 ? 'text-green-600' : 'text-red-600'}`}>
              Total: {totalWeight}% {totalWeight === 100 ? '' : '(debe ser exactamente 100%)'}
            </div>
            <div className="mt-4 flex justify-end space-x-4">
              <Button variant="secondary" onClick={resetToDefaultWeights} className="ml-2">
                Restablecer valores por defecto
              </Button>
              <Button onClick={() => setShowWeightsDialog(false)}>Cerrar</Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
};
