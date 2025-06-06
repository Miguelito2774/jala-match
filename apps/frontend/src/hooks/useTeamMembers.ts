import { useCallback, useMemo, useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';

import { toast } from 'sonner';

export interface TeamMemberRecommendation {
  employee_id: string;
  name: string;
  role: string;
  area: string;
  technologies: string[];
  sfia_level: number;
  compatibility_score: number;
  analysis: string;
}

export interface MemberToAdd {
  employeeProfileId: string;
  name: string;
  role: string;
  sfiaLevel: number;
  isLeader: boolean;
}

export interface FindTeamMemberRequest {
  TeamId: string;
  Role: string;
  Area: string;
  Level: string;
  Technologies: string[];
}

export interface TeamMemberUpdateRequest {
  teamId: string;
  members: MemberToAdd[];
}

export const useTeamMembers = () => {
  const { token } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [recommendations, setRecommendations] = useState<TeamMemberRecommendation[]>([]);

  const authHeaders = useMemo(
    () => ({
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    }),
    [token],
  );

  const findTeamMembers = useCallback(
    async (request: FindTeamMemberRequest): Promise<TeamMemberRecommendation[]> => {
      setLoading(true);
      setError(null);

      try {
        const response = await fetch('http://localhost:5001/api/teams/find', {
          method: 'POST',
          headers: authHeaders,
          body: JSON.stringify(request),
        });

        if (!response.ok) {
          throw new Error(`Error: ${response.status}`);
        }

        const data = await response.json();
        setRecommendations(data);
        return data;
      } catch (err) {
        const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
        setError(errorMessage);
        return [];
      } finally {
        setLoading(false);
      }
    },
    [authHeaders],
  );

  const addTeamMembers = useCallback(
    async (request: TeamMemberUpdateRequest): Promise<boolean> => {
      setLoading(true);
      setError(null);

      try {
        const response = await fetch('http://localhost:5001/api/teams/add', {
          method: 'POST',
          headers: authHeaders,
          body: JSON.stringify(request),
        });

        if (!response.ok) {
          throw new Error(`Error: ${response.status}`);
        }

        toast.success('Miembros agregados correctamente');
        return true;
      } catch (err) {
        const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
        setError(errorMessage);
        toast.error(`Error al añadir miembros: ${errorMessage}`);
        return false;
      } finally {
        setLoading(false);
      }
    },
    [authHeaders],
  );

  return {
    loading,
    error,
    recommendations,
    findTeamMembers,
    addTeamMembers,
  };
};
