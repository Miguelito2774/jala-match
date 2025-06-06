'use client';

import { useState } from 'react';

import { useRouter } from 'next/navigation';

import { Button } from '@/components/atoms/buttons/Button';
import { Input } from '@/components/atoms/inputs/Input';
import { useAuth } from '@/contexts/AuthContext';

import { DEFAULT_DASHBOARD_ROUTES } from '../../../../routes';

export default function LoginForm() {
  const router = useRouter();
  const { login } = useAuth();
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    try {
      // Autenticar usuario y redirigir según rol
      const { user } = await login(formData.email, formData.password);
      const redirectPath = DEFAULT_DASHBOARD_ROUTES[user.role as keyof typeof DEFAULT_DASHBOARD_ROUTES] || '/login';
      router.replace(redirectPath);
    } catch (err: any) {
      setError(err.message || 'Error al iniciar sesión');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="w-full max-w-md space-y-6">
      <div className="text-center">
        <h1 className="text-3xl font-bold text-gray-900">Iniciar Sesión</h1>
        <p className="mt-2 text-gray-600">Accede a tu cuenta</p>
      </div>

      <form onSubmit={handleSubmit} className="space-y-4">
        <Input
          label="Email"
          type="email"
          value={formData.email}
          onChange={(e) => setFormData({ ...formData, email: e.target.value })}
          required
        />

        <Input
          label="Contraseña"
          type="password"
          value={formData.password}
          onChange={(e) => setFormData({ ...formData, password: e.target.value })}
          required
        />

        {error && <div className="rounded-md bg-red-50 p-3 text-sm text-red-600">{error}</div>}

        <Button type="submit" variant="primary" className="w-full" disabled={isLoading}>
          {isLoading ? 'Iniciando sesión...' : 'Iniciar Sesión'}
        </Button>

        <div className="text-center">
          <a href="/forgot-password" className="text-sm text-blue-500 hover:underline">
            ¿Olvidaste tu contraseña?
          </a>
        </div>

        <div className="text-center text-sm text-gray-600">
          ¿Eres empleado y no tienes cuenta?{' '}
          <a href="/register" className="font-medium text-blue-600 hover:text-blue-500">
            Registrarse
          </a>
        </div>
      </form>
    </div>
  );
}
