import { useCallback, useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';
import { API_BASE_URL } from '@/lib/api';

export interface PendingVerificationDto {
  employeeProfileId: string;
  employeeName: string;
  employeeEmail: string;
  requestedAt: string;
  country: string;
  timezone: string;
  sfiaLevelGeneral: number;
  specializedRoles: string[];
  yearsExperienceTotal: number;
  profilePictureUrl?: string;
}

export interface PendingVerificationsResponse {
  pendingVerifications: PendingVerificationDto[];
  totalCount: number;
}

export interface ProfileForVerificationDto {
  employeeProfileId: string;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  country: string;
  timezone: string;
  sfiaLevelGeneral: number;
  mbti: string;
  requestedAt: string;
  specializedRoles: SpecializedRoleForVerificationDto[];
  workExperiences: WorkExperienceSummaryDto[];
  technologies: TechnologyForVerificationDto[];
  totalYearsExperience: number;
  totalProjects: number;
  profilePictureUrl?: string;
}

export interface SpecializedRoleForVerificationDto {
  roleName: string;
  technicalAreaName: string;
  level: number;
  yearsExperience: number;
}

export interface WorkExperienceSummaryDto {
  projectName: string;
  description?: string;
  startDate: string;
  endDate?: string;
  mainTechnologies: string[];
  durationMonths: number;
}

export interface TechnologyForVerificationDto {
  technologyName: string;
  categoryName: string;
  sfiaLevel: number;
  yearsExperience: number;
}

export interface ApproveProfileVerificationRequest {
  employeeProfileId: string;
  sfiaProposed?: number;
  notes?: string;
}

export interface RejectProfileVerificationRequest {
  employeeProfileId: string;
  notes: string;
}

export interface VerificationDecisionResponse {
  success: boolean;
  message: string;
  newStatus: string;
}

export const useProfileVerifications = () => {
  const { token } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Función para obtener verificaciones pendientes
  const getPendingVerifications = useCallback(
    async (pageSize = 20, pageNumber = 1): Promise<PendingVerificationsResponse> => {
      if (!token) throw new Error('No authentication token');

      setLoading(true);
      setError(null);

      try {
        const response = await fetch(
          `${API_BASE_URL}/api/profile-verifications/pending?pageSize=${pageSize}&pageNumber=${pageNumber}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
              'Content-Type': 'application/json',
            },
          },
        );

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        return response.json();
      } catch (err) {
        const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
        setError(errorMessage);
        throw err;
      } finally {
        setLoading(false);
      }
    },
    [token],
  );

  // Función para obtener detalles de un perfil para verificación
  const getProfileForVerification = useCallback(
    async (employeeProfileId: string): Promise<ProfileForVerificationDto> => {
      if (!token) throw new Error('No authentication token');
      if (!employeeProfileId) throw new Error('Employee profile ID is required');

      setLoading(true);
      setError(null);

      try {
        const response = await fetch(`${API_BASE_URL}/api/profile-verifications/${employeeProfileId}`, {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        });

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        return response.json();
      } catch (err) {
        const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
        setError(errorMessage);
        throw err;
      } finally {
        setLoading(false);
      }
    },
    [token],
  );

  // Función para aprobar perfil
  const approveProfile = useCallback(
    async (
      employeeProfileId: string,
      request: ApproveProfileVerificationRequest,
    ): Promise<VerificationDecisionResponse> => {
      if (!token) throw new Error('No authentication token');

      setLoading(true);
      setError(null);

      try {
        const response = await fetch(`${API_BASE_URL}/api/profile-verifications/${employeeProfileId}/approve`, {
          method: 'POST',
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(request),
        });

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        return response.json();
      } catch (err) {
        const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
        setError(errorMessage);
        throw err;
      } finally {
        setLoading(false);
      }
    },
    [token],
  );

  // Función para rechazar perfil
  const rejectProfile = useCallback(
    async (
      employeeProfileId: string,
      request: RejectProfileVerificationRequest,
    ): Promise<VerificationDecisionResponse> => {
      if (!token) throw new Error('No authentication token');

      setLoading(true);
      setError(null);

      try {
        const response = await fetch(`${API_BASE_URL}/api/profile-verifications/${employeeProfileId}/reject`, {
          method: 'POST',
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(request),
        });

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        return response.json();
      } catch (err) {
        const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
        setError(errorMessage);
        throw err;
      } finally {
        setLoading(false);
      }
    },
    [token],
  );

  return {
    loading,
    error,
    getPendingVerifications,
    getProfileForVerification,
    approveProfile,
    rejectProfile,
  };
};
