import { Suspense } from 'react';

import Link from 'next/link';

import { RegisterForm } from '@/components/molecules/forms/RegisterForm';

function RegisterFormWrapper() {
  return <RegisterForm />;
}

export default function RegisterPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-gray-50 p-4">
      <Suspense
        fallback={
          <div className="w-full max-w-md space-y-6">
            <div className="text-center">
              <div className="mx-auto h-8 w-8 animate-spin rounded-full border-b-2 border-blue-600"></div>
              <p className="mt-2 text-gray-600">Cargando...</p>
            </div>
          </div>
        }
      >
        <RegisterFormWrapper />
      </Suspense>

      <div className="mt-8 text-center text-sm text-gray-500">
        Al registrarte, aceptas nuestros{' '}
        <Link href="/terms" className="font-medium text-blue-600 hover:text-blue-500">
          Términos de Servicio
        </Link>{' '}
        y{' '}
        <Link href="/privacy" className="font-medium text-blue-600 hover:text-blue-500">
          Política de Privacidad
        </Link>
        .
      </div>
    </div>
  );
}
