import { useState } from 'react';

interface ReanalyzeTeamParams {
  TeamId: string;
  MemberIds: string[];
  LeaderId: string;
}

interface TeamMember {
  id: string;
  name: string;
  role: string;
  sfia_level: number;
}

interface RecommendedMember {
  id: string;
  name: string;
  compatibility_score: number;
  analysis: string;
  potential_conflicts: string[];
  team_impact: string;
}

interface TeamData {
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
  recommended_Members: RecommendedMember[];
}

export const useTeamReanalyzer = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [reanalyzedTeam, setReanalyzedTeam] = useState<TeamData | null>(null);

  const reanalyzeTeam = async (params: ReanalyzeTeamParams) => {
    setLoading(true);
    setError(null);

    try {
      const response = await fetch('http://localhost:5001/api/teams/reanalyze', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(params),
      });

      if (!response.ok) throw new Error('Error reanalyzing team');

      const data = await response.json();
      setReanalyzedTeam(data);
      return data;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido');
      return null;
    } finally {
      setLoading(false);
    }
  };

  return { reanalyzeTeam, loading, error, reanalyzedTeam };
};
