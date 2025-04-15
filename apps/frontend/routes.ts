export const ROUTES = {
  PUBLIC: {
    HOME: '/',
    PRIVACY: '/privacy',
    TERMS: '/terms',
    HELP: '/help',
  },
  AUTH: {
    LOGIN: '/login',
    REGISTER: '/register',
    FORGOT_PASSWORD: '/forgot-password',
  },
  EMPLOYEE: {
    DASHBOARD: '/employee/dashboard',
    PROFILE: '/employee/profile',
    TEAMS: '/employee/teams',
  },
  MANAGER: {
    TEAM_BUILDER: '/team-builder',
    TEAM_GENERATED: '/manager/team-generated',
  },
  ADMIN: {
    INVITATIONS: '/admin/invitations',
  },
  SETTINGS: '/settings',
};

export const NAV_ITEMS = [
  { href: ROUTES.EMPLOYEE.DASHBOARD, label: 'Inicio', role: 'employee' },
  { href: ROUTES.EMPLOYEE.PROFILE, label: 'Perfil', role: 'employee' },
  { href: ROUTES.EMPLOYEE.TEAMS, label: 'Equipos', role: 'employee' },
  { href: ROUTES.PUBLIC.HELP, label: 'Ayuda', role: 'all' },
  { href: ROUTES.MANAGER.TEAM_BUILDER, label: 'Crear Equipos', role: 'manager' },
  { href: ROUTES.ADMIN.INVITATIONS, label: 'Invitar Managers', role: 'admin' },
  { href: ROUTES.SETTINGS, label: 'Configuración', role: 'all' },
];
