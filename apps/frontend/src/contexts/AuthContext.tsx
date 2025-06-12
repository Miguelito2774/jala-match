'use client';

import { ReactNode, createContext, useContext, useEffect, useState } from 'react';

export interface User {
  id: string;
  email: string;
  role: 'Employee' | 'Manager' | 'Admin';
  hasProfile: boolean;
  isProfileVerified: boolean;
}

interface AuthResponse {
  token: string;
  user: User;
  expiresAt: string;
}

interface AuthContextType {
  user: User | null;
  token: string | null;
  login: (email: string, password: string) => Promise<AuthResponse>;
  registerEmployee: (email: string, password: string) => Promise<AuthResponse>;
  registerManager: (email: string, password: string, invitationToken: string) => Promise<AuthResponse>;
  logout: () => void;
  isLoading: boolean;
  validateInvitation: (token: string) => Promise<{ isValid: boolean; targetRole?: string }>;
  isAuthenticated: boolean;
  isAdmin: boolean;
  isManager: boolean;
  isEmployee: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5001';

// Transform numeric role from backend to string role
const transformRoleFromNumber = (roleNumber: number): 'Employee' | 'Manager' | 'Admin' => {
  switch (roleNumber) {
    case 1:
      return 'Employee';
    case 2:
      return 'Manager';
    case 3:
      return 'Admin';
    default:
      return 'Employee';
  }
};

export const AuthProvider = ({ children }: AuthProviderProps) => {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const savedToken = localStorage.getItem('auth_token');
    const savedUser = localStorage.getItem('auth_user');
    const expiresAt = localStorage.getItem('auth_expires_at');

    if (savedToken && savedUser && expiresAt) {
      try {
        // Verificar si el token no ha expirado
        const expirationDate = new Date(expiresAt);
        if (expirationDate > new Date()) {
          const userData = JSON.parse(savedUser) as User;
          setToken(savedToken);
          setUser(userData);
        } else {
          // Token expirado, limpiar
          localStorage.removeItem('auth_token');
          localStorage.removeItem('auth_user');
          localStorage.removeItem('auth_expires_at');
        }
      } catch {
        localStorage.removeItem('auth_token');
        localStorage.removeItem('auth_user');
        localStorage.removeItem('auth_expires_at');
      }
    }
    setIsLoading(false);
  }, []);

  const login = async (email: string, password: string) => {
    const res = await fetch(`${API_BASE_URL}/api/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password }),
    });

    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.detail || 'Error en el login');
    }

    const rawData = await res.json();

    // Transform numeric role to string role
    const transformedUser = {
      ...rawData.user,
      role: transformRoleFromNumber(rawData.user.role),
    };

    const data: AuthResponse = {
      ...rawData,
      user: transformedUser,
    };

    setToken(data.token);
    setUser(data.user);
    localStorage.setItem('auth_token', data.token);
    document.cookie = `auth_token=${data.token}; path=/;`;
    localStorage.setItem('auth_user', JSON.stringify(data.user));
    document.cookie = `auth_user=${encodeURIComponent(JSON.stringify(data.user))}; path=/;`;
    localStorage.setItem('auth_expires_at', data.expiresAt);
    document.cookie = `auth_expires_at=${data.expiresAt}; path=/;`;

    return data;
  };

  const registerEmployee = async (email: string, password: string) => {
    const res = await fetch(`${API_BASE_URL}/api/auth/register/employee`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password }),
    });

    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.detail || 'Error en el registro de empleado');
    }

    const rawData = await res.json();

    // Transform numeric role to string role
    const transformedUser = {
      ...rawData.user,
      role: transformRoleFromNumber(rawData.user.role),
      hasProfile: false, // Estado inicial después del registro
      isProfileVerified: false, // Estado inicial después del registro
    };

    const data: AuthResponse = {
      ...rawData,
      user: transformedUser,
    };

    setToken(data.token);
    setUser(data.user);
    localStorage.setItem('auth_token', data.token);
    document.cookie = `auth_token=${data.token}; path=/;`;
    localStorage.setItem('auth_user', JSON.stringify(data.user));
    document.cookie = `auth_user=${encodeURIComponent(JSON.stringify(data.user))}; path=/;`;
    localStorage.setItem('auth_expires_at', data.expiresAt);
    document.cookie = `auth_expires_at=${data.expiresAt}; path=/;`;

    return data;
  };

  const registerManager = async (email: string, password: string, invitationToken: string) => {
    const res = await fetch(`${API_BASE_URL}/api/auth/register/manager?invitationToken=${invitationToken}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password }),
    });

    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.detail || 'Error en el registro de manager');
    }

    const rawData = await res.json();

    // Transform numeric role to string role
    const transformedUser = {
      ...rawData.user,
      role: transformRoleFromNumber(rawData.user.role),
    };

    const data: AuthResponse = {
      ...rawData,
      user: transformedUser,
    };

    setToken(data.token);
    setUser(data.user);
    localStorage.setItem('auth_token', data.token);
    document.cookie = `auth_token=${data.token}; path=/;`;
    localStorage.setItem('auth_user', JSON.stringify(data.user));
    document.cookie = `auth_user=${encodeURIComponent(JSON.stringify(data.user))}; path=/;`;
    localStorage.setItem('auth_expires_at', data.expiresAt);
    document.cookie = `auth_expires_at=${data.expiresAt}; path=/;`;

    return data;
  };

  const validateInvitation = async (token: string): Promise<{ isValid: boolean; targetRole?: string }> => {
    try {
      const res = await fetch(`${API_BASE_URL}/api/auth/validate-invitation/${token}`, {
        method: 'GET',
      });

      if (res.ok) {
        const data = await res.json();
        return {
          isValid: data.isValid,
          targetRole: transformRoleFromNumber(data.targetRole),
        };
      }

      return { isValid: false };
    } catch {
      return { isValid: false };
    }
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem('auth_token');
    localStorage.removeItem('auth_user');
    localStorage.removeItem('auth_expires_at');
    // Clear cookies as well
    document.cookie = 'auth_token=; path=/; max-age=0; expires=Thu, 01 Jan 1970 00:00:00 GMT';
    document.cookie = 'auth_user=; path=/; max-age=0; expires=Thu, 01 Jan 1970 00:00:00 GMT';
    document.cookie = 'auth_expires_at=; path=/; max-age=0; expires=Thu, 01 Jan 1970 00:00:00 GMT';
  };

  const isAuthenticated = Boolean(token && user);
  const isAdmin = user?.role === 'Admin';
  const isManager = user?.role === 'Manager';
  const isEmployee = user?.role === 'Employee';

  const value: AuthContextType = {
    user,
    token,
    login,
    registerEmployee,
    registerManager,
    logout,
    isLoading,
    validateInvitation,
    isAuthenticated,
    isAdmin,
    isManager,
    isEmployee,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
