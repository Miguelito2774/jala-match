import { useCallback, useEffect, useState } from 'react';

export interface TeamMember {
  employeeProfileId: string;
  name: string;
  role: string;
  sfiaLevel: number;
  isLeader: boolean;
}

export interface TeamAnalysis {
  strengths: string[];
  weaknesses: string[];
  compatibility: string;
}

export interface TeamWeights {
  sfiaWeight: number;
  technicalWeight: number;
  psychologicalWeight: number;
  experienceWeight: number;
  languageWeight: number;
  interestsWeight: number;
  timezoneWeight: number;
}

export interface Team {
  teamId: string;
  name: string;
  creatorId: string;
  compatibilityScore: number;
  members: TeamMember[];
  requiredTechnologies: string[];
  analysis: TeamAnalysis;
  weights: TeamWeights;
  createdAt: string;
  isActive: boolean;
}

export interface CreateTeamRequest {
  name: string;
  creatorId: string;
  members: TeamMember[];
  leaderId: string;
  analysis: TeamAnalysis;
  compatibilityScore: number;
  weights: TeamWeights;
  requiredTechnologies: string[];
}

export const useTeams = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [teams, setTeams] = useState<Team[]>([]);
  const [currentTeam, setCurrentTeam] = useState<Team | null>(null);

  const apiBaseUrl = 'http://localhost:5001/api/teams';

  // Memoizar las funciones para prevenir renders innecesarios
  const handleError = useCallback((error: unknown) => {
    if (error instanceof Error) {
      setError(error.message);
    } else {
      setError('Ocurrió un error inesperado');
    }
  }, []);

  const getAllTeams = useCallback(
    async (abortController?: AbortController): Promise<Team[]> => {
      setLoading(true);
      try {
        const response = await fetch(`${apiBaseUrl}/all`, {
          signal: abortController?.signal,
        });

        if (!response.ok) throw new Error(`Error: ${response.status}`);

        const data = await response.json();
        setTeams(data);
        return data;
      } catch (err) {
        if (!abortController?.signal.aborted) handleError(err);
        return [];
      } finally {
        setLoading(false);
      }
    },
    [apiBaseUrl, handleError],
  );

  const createTeam = useCallback(
    async (teamData: CreateTeamRequest): Promise<Team | null> => {
      setLoading(true);
      try {
        const response = await fetch(`${apiBaseUrl}/create`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(teamData),
        });

        if (!response.ok) throw new Error(`Error: ${response.status}`);

        const data = await response.json();
        setTeams((prev) => [data, ...prev]);
        setCurrentTeam(data);
        return data;
      } catch (err) {
        handleError(err);
        return null;
      } finally {
        setLoading(false);
      }
    },
    [apiBaseUrl, handleError],
  );

  const getTeamById = useCallback(
    async (teamId: string): Promise<Team | null> => {
      setLoading(true);
      try {
        const response = await fetch(`${apiBaseUrl}/by-id/${teamId}`);

        if (!response.ok) throw new Error(`Error: ${response.status}`);

        const data = await response.json();
        setCurrentTeam(data);
        return data;
      } catch (err) {
        handleError(err);
        return null;
      } finally {
        setLoading(false);
      }
    },
    [apiBaseUrl, handleError],
  );

  const deleteTeam = useCallback(
    async (teamId: string): Promise<boolean> => {
      setLoading(true);
      try {
        const response = await fetch(`${apiBaseUrl}/${teamId}`, {
          method: 'DELETE',
        });

        if (!response.ok) throw new Error(`Error: ${response.status}`);

        setTeams((prev) => prev.filter((team) => team.teamId !== teamId));
        setCurrentTeam(null);
        return true;
      } catch (err) {
        handleError(err);
        return false;
      } finally {
        setLoading(false);
      }
    },
    [apiBaseUrl, handleError],
  );

  // Limpiar errores cuando se desmonta el componente
  useEffect(() => {
    return () => setError(null);
  }, []);

  return {
    loading,
    error,
    teams,
    currentTeam,
    createTeam,
    getAllTeams,
    getTeamById,
    deleteTeam,
    setTeams,
  };
};
