import { useCallback, useEffect, useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';
import { API_BASE_URL } from '@/lib/api';

export interface ProfileVerification {
  id: string;
  sfiaProposed: number;
  status: 'Pending' | 'Approved' | 'Rejected';
  notes?: string;
  requestedAt: string;
  reviewedAt?: string;
  reviewerName?: string;
  reviewerEmail?: string;
}

// Mapeo de números/strings a strings para el status
const mapVerificationStatus = (status: number | string): 'Pending' | 'Approved' | 'Rejected' => {
  // Si es string (del enum serializado)
  if (typeof status === 'string') {
    switch (status) {
      case 'NotRequested':
      case 'Pending':
        return 'Pending';
      case 'Approved':
        return 'Approved';
      case 'Rejected':
        return 'Rejected';
      default:
        return 'Pending';
    }
  }

  // Si es número
  switch (status) {
    case 0:
      return 'Pending'; // NotRequested pero cuando hay solicitud es Pending
    case 1:
      return 'Pending';
    case 2:
      return 'Approved';
    case 3:
      return 'Rejected';
    default:
      return 'Pending';
  }
};

interface UseVerificationHistoryReturn {
  verifications: ProfileVerification[];
  loading: boolean;
  error: string | null;
  refetch: () => void;
}

export const useVerificationHistory = (): UseVerificationHistoryReturn => {
  const { token, user } = useAuth();
  const [verifications, setVerifications] = useState<ProfileVerification[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchVerificationHistory = useCallback(async () => {
    if (!token || !user?.id) {
      setError('Usuario no autenticado');
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);

      const response = await fetch(`${API_BASE_URL}/employee-profiles/user/${user.id}/verification-history`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Error al obtener el historial de verificaciones');
      }

      const data = await response.json();

      // Mapear los datos del backend para convertir solo el status
      const mappedVerifications = data.map((verification: any) => ({
        ...verification,
        status: mapVerificationStatus(verification.status),
      }));

      setVerifications(mappedVerifications);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido');
    } finally {
      setLoading(false);
    }
  }, [token, user?.id]);

  useEffect(() => {
    fetchVerificationHistory();
  }, [fetchVerificationHistory]);

  return {
    verifications,
    loading,
    error,
    refetch: fetchVerificationHistory,
  };
};
