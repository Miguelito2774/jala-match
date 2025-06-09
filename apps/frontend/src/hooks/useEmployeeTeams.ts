import { useCallback, useEffect, useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';

// Types based on backend DTOs
export interface TeammateSpecializedRole {
  roleName: string;
  technicalArea: string;
  level: number;
  yearsExperience: number;
}

export interface TeammateTechnology {
  technologyName: string;
  categoryName: string;
  sfiaLevel: number;
  yearsExperience: number;
  version: string;
}

export interface TeammateLanguage {
  language: string;
  proficiency: string;
}

export interface Teammate {
  employeeProfileId: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  role: string;
  sfiaLevel: number;
  isLeader: boolean;
  country: string;
  timezone: string;
  mbti: string;
  availability: boolean;
  sfiaLevelGeneral: number;
  profilePictureUrl?: string;
  specializedRoles: TeammateSpecializedRole[];
  technologies: TeammateTechnology[];
  languages: TeammateLanguage[];
  personalInterests: string[];
}

export interface EmployeeTeam {
  teamId: string;
  teamName: string;
  creatorName: string;
  compatibilityScore: number;
  isActive: boolean;
  teammates: Teammate[];
  isCurrentUserLeader: boolean;
}

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5001';

export const useEmployeeTeams = () => {
  const { user, token } = useAuth();
  const [teams, setTeams] = useState<EmployeeTeam[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchEmployeeTeams = useCallback(async () => {
    if (!user || !token) {
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);

      const response = await fetch(`${API_BASE_URL}/api/teams/my-teams`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error(`Error: ${response.status} ${response.statusText}`);
      }

      const data: EmployeeTeam[] = await response.json();
      setTeams(data);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido al cargar equipos';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  }, [user, token]);

  const refetch = useCallback(() => {
    fetchEmployeeTeams();
  }, [fetchEmployeeTeams]);

  useEffect(() => {
    fetchEmployeeTeams();
  }, [fetchEmployeeTeams]);

  return {
    teams,
    loading,
    error,
    refetch,
  };
};
