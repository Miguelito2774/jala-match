'use client';

import { useState } from 'react';

import { Button } from '@/components/atoms/buttons/Button';
import { Select } from '@/components/atoms/inputs/Select';

export const TeamBuilder = () => {
  const [roles, setRoles] = useState<string[]>([]);
  const [technologies, setTechnologies] = useState<string[]>([]);
  const [sfiaLevel, setSfiaLevel] = useState<number>(3);
  const [availability, setAvailability] = useState<boolean>(true);

  const handleGenerateTeams = () => {
    console.log({ roles, technologies, sfiaLevel, availability }); // Only for testing
  };

  return (
    <div className="rounded-lg bg-white p-6 shadow">
      <div className="space-y-6">
        <div>
          <h2 className="mb-4 text-lg font-medium text-gray-900">Requisitos del Equipo</h2>
          <div className="gap-6 md:grid-cols-2 grid grid-cols-1">
            <div>
              <label className="mb-1 text-sm font-medium text-gray-700 block">Roles Requeridos</label>
              <Select
                options={[
                  { value: 'developer', label: 'Desarrollador' },
                  { value: 'qa', label: 'QA' },
                  { value: 'devops', label: 'DevOps' },
                ]}
                isMulti
                onChange={(selected) => setRoles(Array.isArray(selected) ? selected.map((s) => s.value) : [])}
              />
            </div>

            <div>
              <label className="mb-1 text-sm font-medium text-gray-700 block">Tecnologías</label>
              <Select
                options={[
                  { value: 'react', label: 'React' },
                  { value: 'node', label: 'Node.js' },
                  { value: 'python', label: 'Python' },
                ]}
                isMulti
                onChange={(selected) => setTechnologies(Array.isArray(selected) ? selected.map((s) => s.value) : [])}
              />
            </div>

            <div>
              <label className="mb-1 text-sm font-medium text-gray-700 block">Nivel SFIA Mínimo</label>
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
                onChange={(selected) => setSfiaLevel(selected.value)}
                defaultValue={{ value: 3, label: 'SFIA 3 - Intermedio' }}
              />
            </div>

            <div>
              <label className="mb-1 text-sm font-medium text-gray-700 block">Disponibilidad</label>
              <Select
                options={[
                  { value: true, label: 'Disponible' },
                  { value: false, label: 'No Disponible' },
                ]}
                onChange={(selected) => setAvailability(selected.value)}
                defaultValue={{ value: true, label: 'Disponible' }}
              />
            </div>
          </div>
        </div>

        <div className="border-gray-200 pt-6 border-t">
          <h2 className="mb-4 text-lg font-medium text-gray-900">Criterios de Compatibilidad</h2>
          <div className="space-y-4">
            <div>
              <label className="mb-1 text-sm font-medium text-gray-700 block">Compatibilidad Técnica: 40%</label>
              <input
                type="range"
                min="0"
                max="100"
                defaultValue="40"
                className="h-2 rounded-lg bg-gray-200 w-full cursor-pointer appearance-none"
              />
            </div>
            <div>
              <label className="mb-1 text-sm font-medium text-gray-700 block">
                Compatibilidad Psicológica (MBTI): 35%
              </label>
              <input
                type="range"
                min="0"
                max="100"
                defaultValue="35"
                className="h-2 rounded-lg bg-gray-200 w-full cursor-pointer appearance-none"
              />
            </div>
            <div>
              <label className="mb-1 text-sm font-medium text-gray-700 block">Compatibilidad por Intereses: 25%</label>
              <input
                type="range"
                min="0"
                max="100"
                defaultValue="25"
                className="h-2 rounded-lg bg-gray-200 w-full cursor-pointer appearance-none"
              />
            </div>
          </div>
        </div>

        <div className="flex justify-end">
          <Button onClick={handleGenerateTeams} className="bg-secondary hover:bg-secondary-dark">
            Generar Equipos
          </Button>
        </div>
      </div>
    </div>
  );
};
