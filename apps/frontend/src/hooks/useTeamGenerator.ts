import { useState } from 'react';

interface Role {
  role: string;
  level: string;
}

interface Weights {
  sfiaWeight: number;
  technicalWeight: number;
  psychologicalWeight: number;
  experienceWeight: number;
  languageWeight: number;
  interestsWeight: number;
  timezoneWeight: number;
}

interface TeamGeneratorParams {
  creatorId: string;
  teamSize: number;
  roles: Role[];
  technologies: string[];
  sfiaLevel: number;
  weights: Weights;
}

interface TeamMember {
  id: string;
  name: string;
  role: string;
  sfia_level: number;
}

interface GeneratedTeamResponse {
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
}

export const useTeamGenerator = () => {
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
