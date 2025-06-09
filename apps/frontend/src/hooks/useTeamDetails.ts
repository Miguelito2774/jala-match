import { useCallback, useEffect, useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';

// Types for team details (extended from employee teams)
export interface TeamMemberDetail {
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
  specializedRoles: Array<{
    roleName: string;
    technicalArea: string;
    level: number;
    yearsExperience: number;
  }>;
  technologies: Array<{
    technologyName: string;
    categoryName: string;
    sfiaLevel: number;
    yearsExperience: number;
    version: string;
  }>;
  languages: Array<{
    language: string;
    proficiency: string;
  }>;
  personalInterests: string[];
}

export interface TeamDetails {
  teamId: string;
  teamName: string;
  creatorName: string;
  compatibilityScore: number;
  isActive: boolean;
  createdAt: string;
  description?: string;
  teammates: TeamMemberDetail[];
  isCurrentUserLeader: boolean;
  teamLeader?: TeamMemberDetail;
  teamStats: {
    totalMembers: number;
    averageSfiaLevel: number;
    uniqueTechnologies: number;
    uniqueLanguages: number;
    countriesRepresented: number;
    mbtiDistribution: Record<string, number>;
  };
}

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5001';

export const useTeamDetails = (teamId: string) => {
  const { user, token } = useAuth();
  const [teamDetails, setTeamDetails] = useState<TeamDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchTeamDetails = useCallback(async () => {
    if (!user || !token || !teamId) {
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);

      // First get the basic team info from my-teams endpoint
      const teamsResponse = await fetch(`${API_BASE_URL}/api/teams/my-teams`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!teamsResponse.ok) {
        throw new Error(`Error: ${teamsResponse.status} ${teamsResponse.statusText}`);
      }

      const teams = await teamsResponse.json();
      const team = teams.find((t: any) => t.teamId === teamId);

      if (!team) {
        throw new Error('Equipo no encontrado o no tienes acceso a él');
      }

      // Calculate team statistics
      const teammates = team.teammates || [];
      const teamStats = {
        totalMembers: teammates.length,
        averageSfiaLevel:
          teammates.length > 0
            ? teammates.reduce((sum: number, member: any) => sum + (member.sfiaLevelGeneral || 0), 0) / teammates.length
            : 0,
        uniqueTechnologies: new Set(
          teammates.flatMap((member: any) => member.technologies?.map((tech: any) => tech.technologyName) || []),
        ).size,
        uniqueLanguages: new Set(
          teammates.flatMap((member: any) => member.languages?.map((lang: any) => lang.language) || []),
        ).size,
        countriesRepresented: new Set(teammates.map((member: any) => member.country)).size,
        mbtiDistribution: teammates.reduce((acc: Record<string, number>, member: any) => {
          if (member.mbti) {
            acc[member.mbti] = (acc[member.mbti] || 0) + 1;
          }
          return acc;
        }, {}),
      };

      const teamLeader = teammates.find((member: any) => member.isLeader);

      const teamDetails: TeamDetails = {
        ...team,
        createdAt: new Date().toISOString(), // We don't have this from the current endpoint
        teamStats,
        teamLeader,
      };

      setTeamDetails(teamDetails);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido al cargar detalles del equipo';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  }, [user, token, teamId]);

  const refetch = useCallback(() => {
    fetchTeamDetails();
  }, [fetchTeamDetails]);

  useEffect(() => {
    fetchTeamDetails();
  }, [fetchTeamDetails]);

  return {
    teamDetails,
    loading,
    error,
    refetch,
  };
};
