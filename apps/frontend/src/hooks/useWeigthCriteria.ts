import { useEffect, useState } from 'react';

interface WeightCriterion {
  id: string;
  name: string;
  defaultValue: number;
}

export const useWeightCriteria = () => {
  const [criteria, setCriteria] = useState<WeightCriterion[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchCriteria = async () => {
      try {
        const response = await fetch('http://localhost:5001/api/teams/weight-criteria');
        if (!response.ok) throw new Error('Error fetching weight criteria');
        const data = await response.json();
        setCriteria(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error desconocido');
      } finally {
        setLoading(false);
      }
    };

    fetchCriteria();
  }, []);

  return { criteria, loading, error };
};
