import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

const PROTECTED_ROUTES = ['/profile', '/teams', '/team-builder', '/admin', '/settings'];
const PUBLIC_ROUTES = ['/', '/login', '/register', '/forgot-password', '/privacy', '/terms'];

const ROLE_ROUTES = {
  Employee: ['/profile', '/teams'],
  Manager: ['/team-builder', '/teams'],
  Admin: ['/admin'],
};

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  if (PUBLIC_ROUTES.includes(pathname) || pathname.startsWith('/_next') || pathname.startsWith('/api')) {
    return NextResponse.next();
  }

  const token = request.cookies.get('auth_token')?.value;
  const userCookie = request.cookies.get('auth_user')?.value;

  if (!token) {
    const loginUrl = new URL('/login', request.url);
    loginUrl.searchParams.set('redirect', pathname);
    return NextResponse.redirect(loginUrl);
  }

  const expiresAtCookie = request.cookies.get('auth_expires_at')?.value;
  if (expiresAtCookie) {
    const expirationDate = new Date(expiresAtCookie);
    if (expirationDate <= new Date()) {
      const loginUrl = new URL('/login', request.url);
      const response = NextResponse.redirect(loginUrl);

      response.cookies.delete('auth_token');
      response.cookies.delete('auth_user');
      response.cookies.delete('auth_expires_at');

      return response;
    }
  }

  try {
    if (userCookie) {
      const decodedUserCookie = decodeURIComponent(userCookie);
      const user = JSON.parse(decodedUserCookie);
      const userRole = user.role as keyof typeof ROLE_ROUTES;

      const isProtectedRoute = PROTECTED_ROUTES.some((route) => pathname.startsWith(route));

      if (isProtectedRoute) {
        const allowedRoutes = ROLE_ROUTES[userRole] || [];
        const hasPermission = allowedRoutes.some((route) => pathname.startsWith(route));

        if (!hasPermission) {
          const dashboardUrl = getDashboardForRole(user.role);
          return NextResponse.redirect(new URL(dashboardUrl, request.url));
        }
      }

      if (userRole === 'Manager') {
      }
    }
  } catch (_error) {
    const loginUrl = new URL('/login', request.url);
    const response = NextResponse.redirect(loginUrl);

    response.cookies.delete('auth_token');
    response.cookies.delete('auth_user');
    response.cookies.delete('auth_expires_at');

    return response;
  }

  return NextResponse.next();
}

function getDashboardForRole(role: string): string {
  switch (role) {
    case 'Employee':
      return '/employee/profile';
    case 'Manager':
      return '/teams';
    case 'Admin':
      return '/admin/invitations';
    default:
      return '/login';
  }
}
export const config = {
  matcher: [
    /*
     * Match all request paths except for the ones starting with:
     * - api (API routes)
     * - _next/static (static files)
     * - _next/image (image optimization files)
     * - favicon.ico (favicon file)
     */
    '/((?!api|_next/static|_next/image|favicon.ico).*)',
  ],
};
