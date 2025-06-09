'use client';

import { useState } from 'react';

import Link from 'next/link';

import { Button } from '@/components/atoms/buttons/Button';
import { Input } from '@/components/atoms/inputs/Input';
import { usePasswordReset } from '@/hooks/usePasswordReset';

export const ForgotPasswordForm = () => {
  const { requestPasswordReset, loading, error } = usePasswordReset();
  const [submitted, setSubmitted] = useState(false);
  const [email, setEmail] = useState('');
  const [formError, setFormError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError('');

    if (!email.trim()) {
      setFormError('Por favor ingresa tu correo electrónico');
      return;
    }

    try {
      await requestPasswordReset(email);
      setSubmitted(true);
    } catch (_err) {
      setFormError(error || 'Error al enviar el correo de recuperación');
    }
  };

  return (
    <div className="w-full max-w-md space-y-6">
      <div className="text-center">
        <h1 className="text-3xl font-bold text-gray-900">Recuperar Contraseña</h1>
        <p className="mt-2 text-gray-600">
          {submitted
            ? 'Revisa tu correo para el enlace de recuperación'
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
            disabled={loading}
          />

          {(formError || error) && (
            <div className="rounded-md bg-red-50 p-3 text-sm text-red-600">{formError || error}</div>
          )}

          <Button type="submit" className="w-full" disabled={loading}>
            {loading ? 'Enviando...' : 'Enviar Enlace de Recuperación'}
          </Button>
        </form>
      ) : (
        <div className="rounded-md bg-green-50 p-4">
          <div className="text-center">
            <h3 className="mb-2 text-lg font-medium text-green-800">¡Correo enviado!</h3>
            <p className="mb-4 text-sm text-green-700">
              Hemos enviado un enlace de recuperación a <strong>{email}</strong>. Revisa tu bandeja de entrada y sigue
              las instrucciones para restablecer tu contraseña.
            </p>
            <p className="text-xs text-green-600">El enlace expirará en 1 hora por seguridad.</p>

            <div className="mt-4">
              <Button variant="ghost" size="sm" onClick={() => setSubmitted(false)} disabled={loading}>
                ¿No recibiste el correo? Enviar de nuevo
              </Button>
            </div>
          </div>
        </div>
      )}

      <div className="text-center text-sm text-gray-600">
        <Link href="/login" className="font-medium text-blue-600 hover:text-blue-500">
          Volver a Iniciar Sesión
        </Link>
      </div>
    </div>
  );
};
