import { useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';

interface TeamGeneratorParams {
  CreatorId: string;
  TeamSize: number;
  Requirements: {
    Role: string;
    Area: string;
    Level: string;
  }[];
  Technologies: string[];
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
}

export interface TeamMember {
  id: string;
  name: string;
  role: string;
  sfia_Level: number;
}

export interface RecommendedMember {
  id: string;
  name: string;
  compatibility_Score: number;
  analysis: string;
  potential_Conflicts: string[];
  team_Impact: string;
}

export interface GeneratedTeamResponse {
  teams: {
    team_Id: string;
    members: TeamMember[];
  }[];
  recommended_Leader: {
    id: string;
    name: string;
    rationale: string;
  };
  team_Analysis: {
    strengths: string[];
    weaknesses: string[];
    compatibility: string;
  };
  compatibility_Score: number;
  recommended_members: RecommendedMember[];
}

export const useTeamGenerator = () => {
  const { token } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [generatedTeam, setGeneratedTeam] = useState<GeneratedTeamResponse | null>(null);

  const generateTeam = async (params: TeamGeneratorParams) => {
    setLoading(true);
    setError(null);

    try {
      const response = await fetch('http://localhost:5001/api/teams/generate', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        body: JSON.stringify(params),
      });

      if (!response.ok) throw new Error('Error generating team');

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

  return { generateTeam, loading, error, generatedTeam };
};
