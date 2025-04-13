import { useEffect, useState } from 'react';

interface RoleWithLevels {
  role: string;
  levels: string[];
}

export const useRoles = () => {
  const [roles, setRoles] = useState<RoleWithLevels[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchRoles = async () => {
      try {
        const response = await fetch('http://localhost:5001/api/teams/available-roles');
        if (!response.ok) throw new Error('Error fetching roles');
        const data = await response.json();
        setRoles(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error desconocido');
      } finally {
        setLoading(false);
      }
    };

    fetchRoles();
  }, []);

  return { roles, loading, error };
};
