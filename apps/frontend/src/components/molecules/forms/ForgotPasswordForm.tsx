'use client';

import { useState } from 'react';

import Link from 'next/link';

import { Button } from '@/components/atoms/buttons/Button';
import { Input } from '@/components/atoms/inputs/Input';

export const ForgotPasswordForm = () => {
  const [submitted, setSubmitted] = useState(false);
  const [email, setEmail] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitted(true);
  };

  return (
    <div className="w-full max-w-md space-y-6">
      <div className="text-center">
        <h1 className="text-3xl font-bold text-gray-900">Recuperar Contraseña</h1>
        <p className="mt-2 text-gray-600">
          {submitted
            ? 'Revisa tu correo para el código de verificación'
            : 'Ingresa tu correo electrónico para recuperar tu contraseña'}
        </p>
      </div>

      {!submitted ? (
        <form className="space-y-4" onSubmit={handleSubmit}>
          <Input
            label="Correo Electrónico"
            type="email"
            placeholder="tu@email.com"
            required
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />

          <Button type="submit" className="w-full">
            Enviar Código de Verificación
          </Button>
        </form>
      ) : (
        <form className="space-y-4">
          <Input label="Código de Verificación" type="text" placeholder="123456" required />
          <Input label="Nueva Contraseña" type="password" placeholder="••••••••" required />
          <Input label="Confirmar Contraseña" type="password" placeholder="••••••••" required />

          <Button type="submit" className="w-full">
            Cambiar Contraseña
          </Button>

          <div className="text-center">
            <Button variant="ghost" size="sm" onClick={() => setSubmitted(false)}>
              ¿No recibiste el código? Enviar de nuevo
            </Button>
          </div>
        </form>
      )}

      <div className="text-center text-sm text-gray-600">
        <Link href="/login" className="font-medium text-blue-600 hover:text-blue-500">
          Volver a Iniciar Sesión
        </Link>
      </div>
    </div>
  );
};
