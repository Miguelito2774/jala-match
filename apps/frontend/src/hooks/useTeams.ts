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

export interface MoveTeamMemberRequest {
  sourceTeamId: string;
  targetTeamId: string;
  employeeProfileId: string;
}

export interface AvailableTeamForMove {
  teamId: string;
  name: string;
  currentMemberCount: number;
  hasMember: boolean;
  creatorName: string;
}

export const useTeams = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [teams, setTeams] = useState<Team[]>([]);
  const [currentTeam, setCurrentTeam] = useState<Team | null>(null);
  const [availableTeamsForMove, setAvailableTeamsForMove] = useState<AvailableTeamForMove[]>([]);

  const apiBaseUrl = 'http://localhost:5001/api/teams';

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

  const removeTeamMember = useCallback(
    async (teamId: string, employeeId: string): Promise<boolean> => {
      setLoading(true);
      try {
        const response = await fetch(`${apiBaseUrl}/${teamId}/members/${employeeId}`, {
          method: 'DELETE',
        });

        if (!response.ok) throw new Error(`Error: ${response.status}`);

        // Update local state
        if (currentTeam && currentTeam.teamId === teamId) {
          const updatedTeam = {
            ...currentTeam,
            members: currentTeam.members.filter((member) => member.employeeProfileId !== employeeId),
          };
          setCurrentTeam(updatedTeam);
        }

        setTeams((prev) =>
          prev.map((team) =>
            team.teamId === teamId
              ? { ...team, members: team.members.filter((member) => member.employeeProfileId !== employeeId) }
              : team,
          ),
        );

        return true;
      } catch (err) {
        handleError(err);
        return false;
      } finally {
        setLoading(false);
      }
    },
    [apiBaseUrl, handleError, currentTeam],
  );

  const moveTeamMember = useCallback(
    async (request: MoveTeamMemberRequest): Promise<{ sourceTeam: Team; targetTeam: Team } | null> => {
      setLoading(true);
      try {
        const response = await fetch(`${apiBaseUrl}/move-member`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(request),
        });

        if (!response.ok) throw new Error(`Error: ${response.status}`);

        const data = await response.json();

        // Update local state with both updated teams
        setTeams((prev) =>
          prev.map((team) => {
            if (team.teamId === data.sourceTeam.teamId) {
              return data.sourceTeam;
            }
            if (team.teamId === data.targetTeam.teamId) {
              return data.targetTeam;
            }
            return team;
          }),
        );

        // Update current team if it's one of the affected teams
        if (currentTeam) {
          if (currentTeam.teamId === data.sourceTeam.teamId) {
            setCurrentTeam(data.sourceTeam);
          } else if (currentTeam.teamId === data.targetTeam.teamId) {
            setCurrentTeam(data.targetTeam);
          }
        }

        return { sourceTeam: data.sourceTeam, targetTeam: data.targetTeam };
      } catch (err) {
        handleError(err);
        return null;
      } finally {
        setLoading(false);
      }
    },
    [apiBaseUrl, handleError, currentTeam],
  );

  const getAvailableTeamsForMember = useCallback(
    async (employeeId: string, excludeTeamId?: string): Promise<AvailableTeamForMove[]> => {
      setLoading(true);
      try {
        const url = new URL(`${apiBaseUrl}/available-for-member/${employeeId}`);
        if (excludeTeamId) {
          url.searchParams.append('excludeTeamId', excludeTeamId);
        }

        const response = await fetch(url.toString());

        if (!response.ok) throw new Error(`Error: ${response.status}`);

        const data: AvailableTeamForMove[] = await response.json();

        const filteredData = data.filter((team) => !team.hasMember);

        setAvailableTeamsForMove(filteredData);
        return filteredData;
      } catch (err) {
        handleError(err);
        return [];
      } finally {
        setLoading(false);
      }
    },
    [apiBaseUrl, handleError],
  );

  useEffect(() => {
    return () => setError(null);
  }, []);

  return {
    loading,
    error,
    teams,
    currentTeam,
    availableTeamsForMove,
    createTeam,
    getAllTeams,
    getTeamById,
    deleteTeam,
    removeTeamMember,
    moveTeamMember,
    getAvailableTeamsForMember,
    setTeams,
  };
};
