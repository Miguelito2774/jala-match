'use client';

import { useEffect, useState } from 'react';

import { Button } from '@/components/atoms/buttons/Button';
import { Select } from '@/components/atoms/inputs/Select';
import { Badge } from '@/components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Slider } from '@/components/ui/slider';
import { useRoles } from '@/hooks/useRoles';
import { useTeamGenerator } from '@/hooks/useTeamGenerator';
import { useTechnologies } from '@/hooks/useTechnologies';
import { useWeightCriteria } from '@/hooks/useWeigthCriteria';

import { Loader2 } from 'lucide-react';

import { TeamResultsPage } from './TeamResultsPage';

// Define TypeScript interfaces para mayor seguridad en tipos
interface SelectOption {
  value: string | number;
  label: string;
}

export const TeamBuilder = () => {
  // Obtener datos de la API
  const { roles, loading: loadingRoles } = useRoles();
  const { technologies, loading: loadingTech } = useTechnologies();
  const { criteria, loading: loadingCriteria } = useWeightCriteria();
  const { generateTeam, loading: generatingTeam, generatedTeam } = useTeamGenerator();

  // Estados internos
  const [teamSize, setTeamSize] = useState<number>(3);
  const [teamRoles, setTeamRoles] = useState<{ role: string; level: string }[]>([]);
  const [selectedTechnologies, setSelectedTechnologies] = useState<string[]>([]);
  const [sfiaLevel, setSfiaLevel] = useState<number>(3);
  const [weights, setWeights] = useState<Record<string, number>>({});
  const [showWeightsDialog, setShowWeightsDialog] = useState(false);
  const [showResults, setShowResults] = useState(false);

  // Estados para selección de tecnologías por categoría
  const [selectedCategory, setSelectedCategory] = useState<string>('');
  const [categoryOptions, setCategoryOptions] = useState<SelectOption[]>([]);
  const [availableTechOptions, setAvailableTechOptions] = useState<SelectOption[]>([]);

  // Inicializa los weights desde los criterios
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

  // Inicializa los roles del equipo según el tamaño
  useEffect(() => {
    if (roles.length > 0) {
      const initialRoles = Array(teamSize)
        .fill(null)
        .map(() => ({
          role: roles[0]?.role || '',
          level: roles[0]?.levels?.[0] || '',
        }));
      setTeamRoles(initialRoles);
    }
  }, [teamSize, roles]);

  // Extrae las categorías únicas y configura las opciones
  useEffect(() => {
    if (technologies.length > 0) {
      const uniqueCategories = Array.from(new Set(technologies.map((tech) => tech.categoryName)));
      const catOptions = uniqueCategories.map((category) => ({
        value: category,
        label: category,
      }));
      setCategoryOptions(catOptions);
      // No establecemos categoría por defecto para mostrar todas las tecnologías inicialmente
    }
  }, [technologies]);

  // Y modificar este efecto para manejar el caso sin categoría seleccionada:
  useEffect(() => {
    if (!selectedCategory) {
      // Mostrar todas las tecnologías cuando no hay categoría seleccionada
      const allTechs = technologies.map((tech) => ({
        value: tech.name,
        label: tech.name,
      }));
      setAvailableTechOptions(allTechs);
    } else {
      // Mostrar solo tecnologías de la categoría seleccionada
      const techsInCategory = technologies
        .filter((tech) => tech.categoryName === selectedCategory)
        .map((tech) => ({
          value: tech.name,
          label: tech.name,
        }));
      setAvailableTechOptions(techsInCategory);
    }
  }, [selectedCategory, technologies]);

  // Calcula el total de los weights
  const totalWeight = Object.values(weights).reduce((sum, weight) => sum + weight, 0);

  // Maneja el cambio de tamaño del equipo
  const handleTeamSizeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const size = parseInt(e.target.value, 10);
    if (!isNaN(size) && size > 0) {
      setTeamSize(size);
      if (size > teamRoles.length) {
        const defaultRole = roles.length > 0 ? roles[0]?.role || '' : '';
        const defaultLevel = roles.length > 0 ? roles[0]?.levels?.[0] || '' : '';
        const newRoles = [...teamRoles];
        for (let i = teamRoles.length; i < size; i++) {
          newRoles.push({ role: defaultRole, level: defaultLevel });
        }
        setTeamRoles(newRoles);
      } else {
        setTeamRoles(teamRoles.slice(0, size));
      }
    }
  };

  // Actualiza el rol en una posición específica
  const updateRoleAtIndex = (index: number, field: 'role' | 'level', value: string) => {
    const updatedRoles = [...teamRoles];
    updatedRoles[index] = { ...updatedRoles[index], [field]: value };
    setTeamRoles(updatedRoles);
  };

  // Actualiza el peso de un criterio
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

  // Maneja el cambio en la selección de tecnologías
  const handleTechSelectionChange = (selected: SelectOption | SelectOption[] | null) => {
    const selectedOptions = Array.isArray(selected) ? selected : selected ? [selected] : [];
    // Extraer los nombres de las tecnologías seleccionadas
    const techNames = selectedOptions.map((option) => option.value.toString());
    // Actualizar el estado de tecnologías seleccionadas
    setSelectedTechnologies(techNames);

    console.log('Selected technologies updated:', techNames);
  };

  // Maneja el cambio de categoría
  const handleCategoryChange = (selected: SelectOption | null) => {
    if (selected) {
      setSelectedCategory(selected.value.toString());
      // Al cambiar la categoría, reiniciamos las tecnologías seleccionadas
      setSelectedTechnologies([]);
    } else {
      setSelectedCategory('');
      setSelectedTechnologies([]);
    }
  };

  // Maneja la generación de equipos
  const handleGenerateTeams = async () => {
    console.log('Generating team with technologies:', selectedTechnologies);

    const creatorId = '3fa85f64-5717-4562-b3fc-2c963f66afa6';
    const params = {
      creatorId,
      teamSize,
      roles: teamRoles,
      technologies: selectedTechnologies,
      sfiaLevel,
      weights: {
        sfiaWeight: weights.sfiaWeight || 0,
        technicalWeight: weights.technicalWeight || 0,
        psychologicalWeight: weights.psychologicalWeight || 0,
        experienceWeight: weights.experienceWeight || 0,
        languageWeight: weights.languageWeight || 0,
        interestsWeight: weights.interestsWeight || 0,
        timezoneWeight: weights.timezoneWeight || 0,
      },
    };

    const result = await generateTeam(params);
    if (result) {
      setShowResults(true);
    }
  };

  // El formulario es válido si el total de pesos es 100 y hay al menos una tecnología seleccionada
  const isFormValid = totalWeight === 100 && selectedTechnologies.length > 0;

  // Estado de carga
  if (loadingRoles || loadingTech || loadingCriteria) {
    return (
      <div className="flex items-center justify-center p-8">
        <Loader2 className="text-secondary h-8 w-8 animate-spin" />
        <span className="ml-2 text-gray-600">Cargando configuración...</span>
      </div>
    );
  }

  // Mostrar resultados si el equipo fue generado
  if (showResults && generatedTeam) {
    return <TeamResultsPage teamData={generatedTeam} onBack={() => setShowResults(false)} />;
  }

  return (
    <div className="space-y-8">
      <div className="rounded-lg bg-white p-6 shadow">
        <div className="space-y-6">
          <div>
            <h2 className="mb-4 text-lg font-medium text-gray-900">Requisitos del Equipo</h2>
            <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
              <div>
                <label className="mb-1 block text-sm font-medium text-gray-700">Tamaño del Equipo</label>
                <Input type="number" min={1} value={teamSize} onChange={handleTeamSizeChange} className="w-full" />
              </div>
              {/* Selección de Tecnologías por Categoría */}
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
              {/* Botón para configurar criterios */}
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
              {teamRoles.map((roleObj, index) => (
                <div key={index} className="grid grid-cols-1 gap-4 md:grid-cols-2">
                  <div>
                    <Select
                      options={roles.map((r) => ({ value: r.role, label: r.role }))}
                      onChange={(selected) => {
                        if (selected && !Array.isArray(selected)) {
                          updateRoleAtIndex(index, 'role', selected.value.toString());
                        }
                      }}
                      defaultValue={{ value: roleObj.role, label: roleObj.role }}
                    />
                  </div>
                  <div>
                    <Select
                      options={
                        roles
                          .find((r) => r.role === roleObj.role)
                          ?.levels.map((level) => ({
                            value: level,
                            label: level,
                          })) || []
                      }
                      onChange={(selected) => {
                        if (selected && !Array.isArray(selected)) {
                          updateRoleAtIndex(index, 'level', selected.value.toString());
                        }
                      }}
                      defaultValue={{ value: roleObj.level, label: roleObj.level }}
                      placeholder="Seleccionar nivel..."
                    />
                  </div>
                </div>
              ))}
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
                Generando...
              </>
            ) : (
              'Generar Equipos'
            )}
          </Button>
        </div>
      </div>

      {/* Diálogo para configurar criterios */}
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
