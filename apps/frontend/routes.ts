// routes.ts
export const ROUTES = {
  // Rutas de autenticación
  AUTH: {
    LOGIN: '/login',
    REGISTER: '/register',
    FORGOT_PASSWORD: '/forgot-password',
  },

  // Rutas de empleado (App Router groups usan `/profile` y `/teams` directamente)
  EMPLOYEE: {
    DASHBOARD: '/profile',
    PROFILE: '/profile',
    TEAMS: '/teams',
  },

  // Rutas de manager (sin segment prefix en URL)
  MANAGER: {
    DASHBOARD: '/teams',
    TEAM_BUILDER: '/team-builder',
    TEAM_LIST: '/teams',
    TEAM_DETAIL: '/teams/[teamId]',
  },

  // Rutas de admin
  ADMIN: {
    DASHBOARD: '/dashboard',
    INVITATIONS: '/invitations',
    USERS: '/users',
  },

  // Rutas compartidas
  SETTINGS: '/settings',
  HOME: '/',

  // Rutas públicas
  PRIVACY: '/privacy',
  TERMS: '/terms',
} as const;

// Mapeo de roles a rutas de dashboard por defecto
export const DEFAULT_DASHBOARD_ROUTES = {
  Employee: ROUTES.EMPLOYEE.DASHBOARD,
  Manager: ROUTES.MANAGER.DASHBOARD, // Cambiado a teams como dashboard principal
  Admin: ROUTES.ADMIN.DASHBOARD,
} as const;

// Rutas protegidas que requieren autenticación
export const PROTECTED_ROUTES = ['/employee', '/manager', '/admin', '/settings'] as const;

// Rutas públicas que no requieren autenticación
export const PUBLIC_ROUTES = [
  ROUTES.HOME,
  ROUTES.AUTH.LOGIN,
  ROUTES.AUTH.REGISTER,
  ROUTES.AUTH.FORGOT_PASSWORD,
  ROUTES.PRIVACY,
  ROUTES.TERMS,
] as const;
