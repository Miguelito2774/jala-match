'use client';

import { useEffect, useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';

interface ProfileImage {
  employeeId: string;
  profilePictureUrl: string | null;
}

export const useProfileImages = (employeeIds: string[]) => {
  const [profileImages, setProfileImages] = useState<Record<string, string | null>>({});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { token } = useAuth();

  useEffect(() => {
    if (!employeeIds.length || !token) return;

    const fetchProfileImages = async () => {
      setLoading(true);
      setError(null);

      try {
        const idsParam = employeeIds.join(',');
        const response = await fetch(`http://localhost:5001/api/images/profiles?employeeIds=${idsParam}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data: ProfileImage[] = await response.json();

        const imageMap: Record<string, string | null> = {};
        data.forEach((item) => {
          imageMap[item.employeeId] = item.profilePictureUrl;
        });

        setProfileImages(imageMap);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to fetch profile images');
      } finally {
        setLoading(false);
      }
    };

    fetchProfileImages();
  }, [employeeIds, token]);

  return {
    profileImages,
    loading,
    error,
    getProfileImage: (employeeId: string) => profileImages[employeeId] || null,
  };
};
