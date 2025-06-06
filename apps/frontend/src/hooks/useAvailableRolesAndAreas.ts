import { useEffect, useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';
import { API_BASE_URL } from '@/lib/api';

export interface RoleAreaLevel {
  role: string;
  areas: string[];
  levels: string[];
}

export interface SpecializedRoleMapping {
  id: string;
  roleName: string;
  technicalArea: string;
}

export function useAvailableRolesAndAreas() {
  const { token } = useAuth();
  const [rolesData, setRolesData] = useState<RoleAreaLevel[]>([]);
  const [rolesMapping, setRolesMapping] = useState<SpecializedRoleMapping[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchData() {
      if (!token) {
        setLoading(false);
        return;
      }
      try {
        // Fetch available roles and areas
        const rolesRes = await fetch(`${API_BASE_URL}/api/roles-and-areas/available`, {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        });
        if (!rolesRes.ok) {
          const err = await rolesRes.json();
          throw new Error(err.detail || 'Error fetching available roles and areas');
        }
        const rolesData = await rolesRes.json();
        setRolesData(rolesData.roles || []);

        // Fetch specialized roles mapping
        const mappingRes = await fetch(`${API_BASE_URL}/api/roles-and-areas/mapping`, {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        });
        if (!mappingRes.ok) {
          const err = await mappingRes.json();
          throw new Error(err.detail || 'Error fetching roles mapping');
        }
        const mappingData = await mappingRes.json();
        setRolesMapping(mappingData.specializedRoles || []);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error desconocido');
      } finally {
        setLoading(false);
      }
    }
    fetchData();
  }, [token]);

  return { rolesData, rolesMapping, loading, error };
}

// Helper function to find specialized role ID
export function findSpecializedRoleId(
  rolesMapping: SpecializedRoleMapping[],
  roleName: string,
  technicalArea: string,
): string | null {
  const mapping = rolesMapping.find((role) => role.roleName === roleName && role.technicalArea === technicalArea);
  return mapping?.id || null;
}
