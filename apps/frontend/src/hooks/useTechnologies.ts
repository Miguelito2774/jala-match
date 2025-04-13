import { useEffect, useState } from 'react';

interface Technology {
  id: string;
  name: string;
  categoryId: string;
  categoryName: string;
}

export const useTechnologies = () => {
  const [technologies, setTechnologies] = useState<Technology[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchTechnologies = async () => {
      try {
        const response = await fetch('http://localhost:5001/api/technologies');
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
  }, []);

  return { technologies, loading, error };
};
