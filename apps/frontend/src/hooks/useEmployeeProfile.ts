import { useCallback, useEffect, useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';

export interface EmployeeProfileGeneral {
  id: string;
  firstName: string;
  lastName: string;
  country: string;
  timezone: string;
  sfiaLevelGeneral: number;
  mbti: string;
  profilePictureUrl?: string;
}

export interface EmployeeProfileTechnical {
  sfiaLevelGeneral: number;
  mbti: string;
  specializedRoles: EmployeeSpecializedRole[];
}

// Interface for specialized roles in profile
export interface EmployeeSpecializedRole {
  id: string;
  specializedRoleId: string;
  roleName: string;
  technicalAreaName: string;
  level: number;
  yearsExperience: number;
}

export interface EmployeeProfileComplete {
  generalInfo: EmployeeProfileGeneral;
  technicalProfile: EmployeeProfileTechnical;
  languages: EmployeeLanguage[];
  technologies: EmployeeTechnology[];
  workExperiences: WorkExperience[];
  personalInterests: PersonalInterest[];
  hasProfile: boolean;
  // 1=Pending, 2=Approved, 3=Rejected
  verificationStatus: number;
  verificationNotes?: string | null;
  hasVerificationRequests: boolean;
  completionStatus: {
    general: boolean;
    technical: boolean;
    experience: boolean;
    interests: boolean;
  };
}

export interface EmployeeLanguage {
  id: string;
  language: string;
  proficiency: string;
}

export interface EmployeeTechnology {
  id: string;
  technologyId: string;
  technologyName: string;
  sfiaLevel: number;
  yearsExperience: number;
  version: string;
}

export interface UpdateWorkExperienceRequest {
  projectName: string;
  description: string;
  tools: string[];
  thirdParties: string[];
  frameworks: string[];
  versionControl: string | null;
  projectManagement: string | null;
  responsibilities: string[];
  startDate: string;
  endDate?: string;
}

export interface WorkExperience {
  id: string;
  projectName: string;
  description: string;
  tools: string[];
  thirdParties: string[];
  frameworks: string[];
  versionControl: string;
  projectManagement: string;
  responsibilities: string[];
  startDate: string;
  endDate?: string;
}

export interface PersonalInterest {
  id: string;
  name: string;
  sessionDurationMinutes: number;
  frequency: string;
  interestLevel: string;
}

// Tipos para Roles y ÁreasEspecializadas
export interface RoleAreaLevel {
  role: string;
  areas: string[];
  levels: string[];
}

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5001';

export const useEmployeeProfile = (userId?: string) => {
  const { user, token } = useAuth();
  const [profile, setProfile] = useState<EmployeeProfileComplete | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const targetUserId = userId || user?.id;

  const fetchProfile = useCallback(async () => {
    if (!targetUserId || !token) return;

    try {
      setLoading(true);
      const response = await fetch(`${API_BASE_URL}/employee-profiles/user/${targetUserId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (response.ok) {
        const data = await response.json();
        setProfile(data);
      } else if (response.status === 404) {
        // No tiene perfil creado
        setProfile(null);
      } else {
        throw new Error('Error al cargar el perfil');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido');
    } finally {
      setLoading(false);
    }
  }, [targetUserId, token]);

  const createProfile = async (data: {
    firstName: string;
    lastName: string;
    country: string;
    timezone: string;
    sfiaLevelGeneral: number;
    mbti: string;
  }) => {
    if (!token) throw new Error('No hay token de autenticación');

    const response = await fetch(`${API_BASE_URL}/employee-profiles`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al crear el perfil');
    }

    await fetchProfile();
    return response.json();
  };

  const updateGeneralInfo = async (data: {
    firstName: string;
    lastName: string;
    country: string;
    timezone: string;
  }) => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/employee-profiles/user/${targetUserId}/general-info`, {
      method: 'PUT',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al actualizar información general');
    }

    await fetchProfile();
  };

  const updateTechnicalProfile = async (data: { sfiaLevelGeneral: number; mbti: string }) => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/employee-profiles/user/${targetUserId}/technical`, {
      method: 'PUT',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al actualizar perfil técnico');
    }

    await fetchProfile();
  };

  const requestVerification = async () => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/employee-profiles/user/${targetUserId}/verification-request`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al solicitar verificación');
    }

    await fetchProfile();
  };

  const addSpecializedRole = async (data: { specializedRoleId: string; level: number; yearsExperience: number }) => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/employee-profiles/user/${targetUserId}/specialized-roles`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al agregar rol especializado');
    }

    await fetchProfile();
    return response.json();
  };

  // avoid exhaustive-deps warning: fetchProfile stable

  useEffect(() => {
    fetchProfile();
  }, [fetchProfile]);

  return {
    profile,
    loading,
    error,
    refetch: fetchProfile,
    createProfile,
    updateGeneralInfo,
    updateTechnicalProfile,
    requestVerification,
    addSpecializedRole,
  };
};

// Hook específico para idiomas
export const useEmployeeLanguages = (userId?: string) => {
  const { user, token } = useAuth();
  const [languages, setLanguages] = useState<EmployeeLanguage[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const targetUserId = userId || user?.id;

  const fetchLanguages = useCallback(async () => {
    if (!targetUserId || !token) return;

    try {
      setLoading(true);
      const response = await fetch(`${API_BASE_URL}/api/employee-languages/user/${targetUserId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (response.ok) {
        const data = await response.json();
        setLanguages(data);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido');
    } finally {
      setLoading(false);
    }
  }, [targetUserId, token]);

  const addLanguage = async (data: { language: string; proficiency: string }) => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/api/employee-languages/user/${targetUserId}`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al agregar idioma');
    }

    await fetchLanguages();
    return response.json();
  };

  const updateLanguage = async (languageId: string, data: { language: string; proficiency: string }) => {
    if (!token) throw new Error('No hay token de autenticación');

    const response = await fetch(`${API_BASE_URL}/api/employee-languages/${languageId}`, {
      method: 'PUT',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al actualizar idioma');
    }

    await fetchLanguages();
  };

  const deleteLanguage = async (languageId: string) => {
    if (!token) throw new Error('No hay token de autenticación');

    const response = await fetch(`${API_BASE_URL}/api/employee-languages/${languageId}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al eliminar idioma');
    }

    await fetchLanguages();
  };

  useEffect(() => {
    fetchLanguages();
  }, [fetchLanguages]);

  return {
    languages,
    loading,
    error,
    refetch: fetchLanguages,
    addLanguage,
    updateLanguage,
    deleteLanguage,
  };
};

// Hook para tecnologías
export const useEmployeeTechnologies = (userId?: string) => {
  const { user, token } = useAuth();
  const [technologies, setTechnologies] = useState<EmployeeTechnology[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const targetUserId = userId || user?.id;

  const fetchTechnologies = useCallback(async () => {
    if (!targetUserId || !token) return;

    try {
      setLoading(true);
      // Este endpoint necesitaría estar en el controller principal
      const response = await fetch(`${API_BASE_URL}/employee-profiles/user/${targetUserId}/technical`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (response.ok) {
        const data = await response.json();
        setTechnologies(data.technologies || []);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido');
    } finally {
      setLoading(false);
    }
  }, [targetUserId, token]);

  const addTechnology = async (data: {
    technologyId: string;
    sfiaLevel: number;
    yearsExperience: number;
    version: string;
  }) => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/employee-technologies/user/${targetUserId}`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al agregar tecnología');
    }

    await fetchTechnologies();
    return response.json();
  };

  const importTechnologies = async (technologies: any[]) => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/employee-technologies/user/${targetUserId}/import`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ technologies }),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al importar tecnologías');
    }

    await fetchTechnologies();
    return response.json();
  };

  useEffect(() => {
    fetchTechnologies();
  }, [fetchTechnologies]);

  return {
    technologies,
    loading,
    error,
    refetch: fetchTechnologies,
    addTechnology,
    importTechnologies,
  };
};

// Hook para experiencias laborales
export const useWorkExperiences = (userId?: string) => {
  const { user, token } = useAuth();
  const [experiences, setExperiences] = useState<WorkExperience[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const targetUserId = userId || user?.id;

  const fetchExperiences = useCallback(async () => {
    if (!targetUserId || !token) return;

    try {
      setLoading(true);
      const response = await fetch(`${API_BASE_URL}/work-experiences/user/${targetUserId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (response.ok) {
        const data = await response.json();
        setExperiences(data);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido');
    } finally {
      setLoading(false);
    }
  }, [targetUserId, token]);

  const addExperience = async (data: Omit<WorkExperience, 'id'>) => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/work-experiences/user/${targetUserId}`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al agregar experiencia');
    }

    await fetchExperiences();
    return response.json();
  };

  const updateExperience = async (id: string, data: Omit<WorkExperience, 'id'>) => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/work-experiences/${id}`, {
      method: 'PUT',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json().catch(() => null);
      throw new Error(error?.detail || 'Error al actualizar experiencia');
    }

    await fetchExperiences();

    try {
      return await response.json();
    } catch {
      return null;
    }
  };

  const deleteExperience = async (id: string) => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/work-experiences/${id}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al eliminar experiencia');
    }

    await fetchExperiences();
    return response.ok;
  };

  const refetch = () => {
    fetchExperiences();
  };

  useEffect(() => {
    fetchExperiences();
  }, [fetchExperiences]);

  return {
    experiences,
    loading,
    error,
    addExperience,
    updateExperience,
    deleteExperience,
    refetch,
  };
};

// Hook para intereses personales
export const usePersonalInterests = (userId?: string) => {
  const { user, token } = useAuth();
  const [interests, setInterests] = useState<PersonalInterest[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const targetUserId = userId || user?.id;

  const fetchInterests = useCallback(async () => {
    if (!targetUserId || !token) return;

    try {
      setLoading(true);
      const response = await fetch(`${API_BASE_URL}/personal-interests/user/${targetUserId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (response.ok) {
        const data = await response.json();
        setInterests(data);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido');
    } finally {
      setLoading(false);
    }
  }, [targetUserId, token]);

  const addInterest = async (data: Omit<PersonalInterest, 'id'>) => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/personal-interests`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al agregar interés');
    }

    await fetchInterests();
    return response.json();
  };

  const updateInterest = async (id: string, data: Omit<PersonalInterest, 'id'>) => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/personal-interests/${id}`, {
      method: 'PUT',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json().catch(() => null);
      throw new Error(error?.detail || 'Error al actualizar interés');
    }

    await fetchInterests();

    try {
      return await response.json();
    } catch {
      return null;
    }
  };

  const deleteInterest = async (id: string) => {
    if (!targetUserId || !token) throw new Error('Datos faltantes');

    const response = await fetch(`${API_BASE_URL}/personal-interests/${id}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Error al eliminar interés');
    }

    await fetchInterests();
  };

  useEffect(() => {
    fetchInterests();
  }, [fetchInterests]);

  return {
    interests,
    loading,
    error,
    refetch: fetchInterests,
    addInterest,
    updateInterest,
    deleteInterest,
  };
};

// Hook para obtener roles y áreas disponibles
export const useAvailableRolesAndAreas = () => {
  const { token } = useAuth();
  const [rolesData, setRolesData] = useState<RoleAreaLevel[]>([]);
  const [loadingRoles, setLoadingRoles] = useState(false);
  const [errorRoles, setErrorRoles] = useState<string | null>(null);

  const fetchRolesAndAreas = useCallback(async () => {
    if (!token) return;
    setLoadingRoles(true);
    try {
      const res = await fetch(`${API_BASE_URL}/employee-profiles/available-roles-areas`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });
      if (!res.ok) throw new Error('Error fetching available roles and areas');
      const data = await res.json();
      setRolesData(data.roles || []);
    } catch (err) {
      setErrorRoles(err instanceof Error ? err.message : 'Error desconocido');
    } finally {
      setLoadingRoles(false);
    }
  }, [token]);

  useEffect(() => {
    fetchRolesAndAreas();
  }, [fetchRolesAndAreas]);

  return { rolesData, loadingRoles, errorRoles, refetchRoles: fetchRolesAndAreas };
};
