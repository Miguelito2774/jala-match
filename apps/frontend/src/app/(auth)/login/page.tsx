import Link from 'next/link';

import { LoginForm } from '@/components/molecules/forms/LoginForm';

export default function LoginPage() {
  return (
    <div className="bg-gray-50 p-4 flex min-h-screen flex-col items-center justify-center">
      <LoginForm />

      <div className="mt-8 text-sm text-gray-500 text-center">
        Al iniciar sesión, aceptas nuestros{' '}
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
