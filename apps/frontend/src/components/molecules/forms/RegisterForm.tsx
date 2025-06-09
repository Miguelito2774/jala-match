'use client';

import { useCallback, useEffect, useState } from 'react';

import Link from 'next/link';
import { useRouter, useSearchParams } from 'next/navigation';

import { Button } from '@/components/atoms/buttons/Button';
import { Input } from '@/components/atoms/inputs/Input';
import { useAuth } from '@/contexts/AuthContext';

import { DEFAULT_DASHBOARD_ROUTES } from '../../../../routes';

export const RegisterForm = () => {
  const router = useRouter();
  const searchParams = useSearchParams();
  const { registerEmployee, registerManager, validateInvitation } = useAuth();

  const [formData, setFormData] = useState({
    email: '',
    password: '',
    confirmPassword: '',
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [isManagerRegistration, setIsManagerRegistration] = useState(false);
  const [invitationToken, setInvitationToken] = useState('');
  const [isValidatingInvitation, setIsValidatingInvitation] = useState(false);
  const [registrationSuccessful, setRegistrationSuccessful] = useState(false);

  const validateInvitationToken = useCallback(
    async (token: string) => {
      if (registrationSuccessful) return;

      setIsValidatingInvitation(true);
      try {
        const isValid = await validateInvitation(token);
        if (isValid) {
          setIsManagerRegistration(true);
        } else {
          setError('Token de invitación inválido o expirado');
        }
      } catch (_err) {
        setError('Error al validar la invitación');
      } finally {
        setIsValidatingInvitation(false);
      }
    },
    [validateInvitation, registrationSuccessful],
  );

  useEffect(() => {
    const token = searchParams?.get('invitation');
    if (token) {
      setInvitationToken(token);
      validateInvitationToken(token);
    }
  }, [searchParams, validateInvitationToken]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (formData.password !== formData.confirmPassword) {
      setError('Las contraseñas no coinciden');
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      let authResponse;
      if (isManagerRegistration && invitationToken) {
        authResponse = await registerManager(formData.email, formData.password, invitationToken);
      } else {
        authResponse = await registerEmployee(formData.email, formData.password);
      }

      // Marcar el registro como exitoso antes de redirigir
      setRegistrationSuccessful(true);

      // Use the role from the auth response instead of the context state
      const userRole = authResponse.user.role;
      const redirectPath = DEFAULT_DASHBOARD_ROUTES[userRole as keyof typeof DEFAULT_DASHBOARD_ROUTES] || '/login';
      router.replace(redirectPath);
    } catch (err: any) {
      setError(err.message || 'Error al registrarse');
    } finally {
      setIsLoading(false);
    }
  };

  if (isValidatingInvitation) {
    return (
      <div className="w-full max-w-md space-y-6">
        <div className="text-center">
          <div className="mx-auto h-8 w-8 animate-spin rounded-full border-b-2 border-blue-600"></div>
          <p className="mt-2 text-gray-600">Validando invitación...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="w-full max-w-md space-y-6">
      <div className="text-center">
        <h1 className="text-3xl font-bold text-gray-900">
          {isManagerRegistration ? 'Registro de Manager' : 'Registro de Empleado'}
        </h1>
        <p className="mt-2 text-gray-600">
          {isManagerRegistration ? 'Completa tu registro como manager' : 'Crea tu cuenta como empleado'}
        </p>
      </div>

      <form onSubmit={handleSubmit} className="space-y-4">
        <Input
          label="Correo Electrónico"
          type="email"
          placeholder="tu@email.com"
          value={formData.email}
          onChange={(e) => setFormData({ ...formData, email: e.target.value })}
          required
        />

        <Input
          label="Contraseña"
          type="password"
          placeholder="••••••••"
          value={formData.password}
          onChange={(e) => setFormData({ ...formData, password: e.target.value })}
          required
        />

        <Input
          label="Confirmar Contraseña"
          type="password"
          placeholder="••••••••"
          value={formData.confirmPassword}
          onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
          required
        />

        {error && <div className="rounded-md bg-red-50 p-3 text-sm text-red-600">{error}</div>}

        <div className="flex items-center">
          <input
            id="terms"
            name="terms"
            type="checkbox"
            className="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
            required
          />
          <label htmlFor="terms" className="ml-2 block text-sm text-gray-900">
            Acepto los{' '}
            <Link href="/terms" className="font-medium text-blue-600 hover:text-blue-500">
              Términos de Servicio
            </Link>{' '}
            y{' '}
            <Link href="/privacy" className="font-medium text-blue-600 hover:text-blue-500">
              Política de Privacidad
            </Link>
          </label>
        </div>

        <Button type="submit" className="w-full" disabled={isLoading}>
          {isLoading ? 'Registrando...' : 'Registrarse'}
        </Button>
      </form>

      <div className="text-center text-sm text-gray-600">
        ¿Ya tienes una cuenta?{' '}
        <Link href="/login" className="font-medium text-blue-600 hover:text-blue-500">
          Iniciar Sesión
        </Link>
      </div>
    </div>
  );
};
