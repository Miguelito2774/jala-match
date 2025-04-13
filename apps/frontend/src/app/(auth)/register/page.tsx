import Link from 'next/link';

import { RegisterForm } from '@/components/molecules/forms/RegisterForm';

export default function RegisterPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-gray-50 p-4">
      <RegisterForm />

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
