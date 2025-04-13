import Link from 'next/link';

import LoginForm from '@/components/molecules/forms/LoginForm';

export default function LoginPage() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-50 p-4">
      <div className="flex flex-col items-center justify-center">
        <LoginForm />

        <div className="mt-8 text-center text-sm text-gray-500">
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
    </div>
  );
}
