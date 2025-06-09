import { useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';

export const useCreateInvitation = () => {
  const [isLoading, setIsLoading] = useState(false);
  const { token } = useAuth();

  const createInvitation = async (email: string, targetRole: 'Manager'): Promise<string> => {
    setIsLoading(true);

    try {
      const response = await fetch(
        `${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5001'}/api/auth/invitations`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            email,
            targetRole: targetRole === 'Manager' ? 2 : 1,
          }),
        },
      );

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.detail || 'Error al crear la invitación');
      }

      const data = await response.text(); // API returns string directly
      return data.replace(/"/g, ''); // Remove quotes if present
    } finally {
      setIsLoading(false);
    }
  };

  return {
    createInvitation,
    isLoading,
  };
};
