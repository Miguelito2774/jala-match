import { useEffect, useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';

interface Technology {
  id: string;
  name: string;
  categoryId: string;
  categoryName: string;
}

export const useTechnologies = () => {
  const { token } = useAuth();
  const [technologies, setTechnologies] = useState<Technology[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!token) return;
    const fetchTechnologies = async () => {
      try {
        const response = await fetch('http://localhost:5001/api/technologies', {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
        });
        if (!response.ok) throw new Error('Error fetching technologies');
        const data = await response.json();
        setTechnologies(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error desconocido');
      } finally {
        setLoading(false);
      }
    };

    fetchTechnologies();
  }, [token]);

  return { technologies, loading, error };
};
