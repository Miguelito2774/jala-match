'use client';

import Link from 'next/link';

import { Button } from '@/components/atoms/buttons/Button';
import { Input } from '@/components/atoms/inputs/Input';

export const LoginForm = () => {
  return (
    <div className="max-w-md space-y-6 w-full">
      <div className="text-center">
        <h1 className="text-3xl font-bold text-gray-900">Iniciar Sesión</h1>
        <p className="mt-2 text-gray-600">Ingresa tus credenciales para acceder</p>
      </div>

      <form className="space-y-4">
        <Input label="Correo Electrónico" type="email" placeholder="tu@email.com" required />

        <Input label="Contraseña" type="password" placeholder="••••••••" required />

        <div className="flex items-center justify-between">
          <div className="flex items-center">
            <input
              id="remember-me"
              name="remember-me"
              type="checkbox"
              className="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
            />
            <label htmlFor="remember-me" className="ml-2 text-sm text-gray-900 block">
              Recordarme
            </label>
          </div>

          <Link href="/forgot-password" className="text-sm text-blue-600 hover:text-blue-500">
            ¿Olvidaste tu contraseña?
          </Link>
        </div>

        <Button type="submit" className="w-full">
          Iniciar Sesión
        </Button>
      </form>

      <div className="text-sm text-gray-600 text-center">
        ¿No tienes una cuenta?{' '}
        <Link href="/register" className="font-medium text-blue-600 hover:text-blue-500">
          Regístrate
        </Link>
      </div>
    </div>
  );
};
