'use client';

import Link from 'next/link';

import { Button } from '@/components/atoms/buttons/Button';
import { Input } from '@/components/atoms/inputs/Input';

export const RegisterForm = () => {
  return (
    <div className="w-full max-w-md space-y-6">
      <div className="text-center">
        <h1 className="text-3xl font-bold text-gray-900">Registro</h1>
        <p className="mt-2 text-gray-600">Crea tu cuenta para comenzar</p>
      </div>

      <form className="space-y-4">
        <div className="grid grid-cols-2 gap-4">
          <Input label="Nombre" type="text" placeholder="Tu nombre" required />
          <Input label="Apellido" type="text" placeholder="Tu apellido" required />
        </div>

        <Input label="Correo Electrónico" type="email" placeholder="tu@email.com" required />

        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">Rol</label>
          {/* <Select
            options={[
              { value: 'developer', label: 'Desarrollador' },
              { value: 'qa', label: 'QA' },
              { value: 'devops', label: 'DevOps' },
              { value: 'automation', label: 'Automation' },
              { value: 'ui-ux', label: 'UI/UX' },
            ]}
            //onChange={(selected) => setRole(selected.value)}
          /> */}
        </div>

        <Input label="Contraseña" type="password" placeholder="••••••••" required />
        <Input label="Confirmar Contraseña" type="password" placeholder="••••••••" required />

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

        <Button type="submit" className="w-full">
          Registrarse
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
