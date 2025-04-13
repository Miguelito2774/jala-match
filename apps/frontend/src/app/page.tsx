import Link from 'next/link';

import { Button } from '@/components/atoms/buttons/Button';

export default function HomePage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-4">
      <h1 className="text-4xl font-bold">Bienvenido a Jala Match</h1>

      <div className="mt-8 flex gap-4">
        <Link href="/login">
          <Button variant="primary">Iniciar Sesión</Button>
        </Link>

        <Link href="/register">
          <Button variant="secondary">Registrarse</Button>
        </Link>
      </div>

      <div className="mt-12">
        <h2 className="mb-4 text-xl">Enlaces legales:</h2>
        <div className="flex gap-4">
          <Link href="/privacy" className="text-blue-500 hover:underline">
            Política de Privacidad
          </Link>
          <Link href="/terms" className="text-blue-500 hover:underline">
            Términos y Condiciones
          </Link>
        </div>
      </div>
    </div>
  );
}
