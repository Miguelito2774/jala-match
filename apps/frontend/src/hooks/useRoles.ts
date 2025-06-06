import { useEffect, useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';
import { API_BASE_URL } from '@/lib/api';

interface RoleWithLevels {
  role: string;
  areas: string[];
  levels: string[];
}

export const useRoles = () => {
  const { token } = useAuth();
  const [roles, setRoles] = useState<RoleWithLevels[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchRoles = async () => {
      try {
        if (!token) {
          setError('No authentication token available');
          setLoading(false);
          return;
        }

        const response = await fetch(`${API_BASE_URL}/api/teams/available-roles`, {
          headers: { Authorization: `Bearer ${token}` },
        });

        if (!response.ok) throw new Error(`HTTP ${response.status}: ${response.statusText}`);

        const data = await response.json();

        // Validate data structure before mapping
        if (!Array.isArray(data)) {
          throw new Error('API response is not an array');
        }

        const mappedRoles = data.map((item: any) => ({
          role: item.Role || item.role || '',
          areas: item.Areas || item.areas || [],
          levels: item.Levels || item.levels || [],
        }));

        setRoles(mappedRoles);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error desconocido');
      } finally {
        setLoading(false);
      }
    };

    fetchRoles();
  }, [token]);

  return { roles, loading, error };
};
