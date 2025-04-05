'use client';

import { useState } from 'react';

import { Button } from '@/components/atoms/buttons/Button';
import { ProfileSectionCard } from '@/components/molecules/cards/ProfileSectionCard';

const sections = [
  {
    id: 'general',
    title: 'Información General',
    completed: false,
    description: 'Datos personales básicos y disponibilidad',
  },
  {
    id: 'technical',
    title: 'Perfil Técnico',
    completed: false,
    description: 'Habilidades técnicas y especializaciones',
  },
  {
    id: 'experience',
    title: 'Experiencia Laboral',
    completed: false,
    description: 'Historial profesional y proyectos',
  },
  {
    id: 'interests',
    title: 'Intereses Personales',
    completed: false,
    description: 'Actividades y preferencias fuera del trabajo',
  },
];

export const ProfileSections = () => {
  const [currentSection, setCurrentSection] = useState<string | null>(null);

  return (
    <div className="space-y-6">
      <div className="gap-6 md:grid-cols-2 grid grid-cols-1">
        {sections.map((section) => (
          <ProfileSectionCard
            key={section.id}
            title={section.title}
            description={section.description}
            completed={section.completed}
            onClick={() => setCurrentSection(section.id)}
          />
        ))}
      </div>

      {currentSection && (
        <div className="mt-8 rounded-lg bg-white p-6 shadow">
          <div className="space-x-3 flex justify-end">
            <Button variant="outline" onClick={() => setCurrentSection(null)}>
              Cancelar
            </Button>
            <Button>Guardar Cambios</Button>
          </div>
        </div>
      )}

      <div className="flex justify-end">
        <Button disabled={!sections.every((s) => s.completed)}>Solicitar Verificación</Button>
      </div>
    </div>
  );
};
