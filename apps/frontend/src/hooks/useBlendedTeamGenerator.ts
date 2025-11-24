import { useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';

import { GeneratedTeamResponse } from './useTeamGenerator';

interface BlendedTeamGeneratorParams {
  CreatorId: string;
  TeamSize: number;
  Technologies: string[];
  ProjectComplexity: 'Low' | 'Medium' | 'High';
  SfiaLevel: number;
  Weights: {
    SfiaWeight: number;
    TechnicalWeight: number;
    PsychologicalWeight: number;
    ExperienceWeight: number;
    LanguageWeight: number;
    InterestsWeight: number;
    TimezoneWeight: number;
  };
  Availability: boolean;
}

export const useBlendedTeamGenerator = () => {
  const { token } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [generatedTeam, setGeneratedTeam] = useState<GeneratedTeamResponse | null>(null);

  const generateBlendedTeam = async (params: BlendedTeamGeneratorParams) => {
    setLoading(true);
    setError(null);

    try {
      const response = await fetch('http://localhost:5001/api/teams/generate-blended', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        body: JSON.stringify(params),
      });

      if (!response.ok) throw new Error('Error generating blended team');

      const data = await response.json();
      setGeneratedTeam(data);
      return data;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido');
      return null;
    } finally {
      setLoading(false);
    }
  };

  return { generateBlendedTeam, loading, error, generatedTeam };
};
