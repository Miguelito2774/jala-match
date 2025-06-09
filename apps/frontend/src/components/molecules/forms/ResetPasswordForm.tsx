'use client';

import { useEffect, useState } from 'react';

import Link from 'next/link';
import { useRouter, useSearchParams } from 'next/navigation';

import { Button } from '@/components/atoms/buttons/Button';
import { Input } from '@/components/atoms/inputs/Input';
import { usePasswordReset } from '@/hooks/usePasswordReset';

export const ResetPasswordForm = () => {
  const { resetPassword, loading, error } = usePasswordReset();
  const [submitted, setSubmitted] = useState(false);
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [formError, setFormError] = useState('');
  const [token, setToken] = useState<string | null>(null);

  const searchParams = useSearchParams();
  const router = useRouter();

  useEffect(() => {
    const tokenParam = searchParams.get('token');
    if (!tokenParam) {
      setFormError('Token de recuperación inválido o faltante');
    } else {
      setToken(tokenParam);
    }
  }, [searchParams]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError('');

    if (!token) {
      setFormError('Token de recuperación inválido');
      return;
    }

    if (!newPassword.trim()) {
      setFormError('Por favor ingresa tu nueva contraseña');
      return;
    }

    if (newPassword.length < 6) {
      setFormError('La contraseña debe tener al menos 6 caracteres');
      return;
    }

    if (newPassword !== confirmPassword) {
      setFormError('Las contraseñas no coinciden');
      return;
    }

    try {
      await resetPassword(token, newPassword);
      setSubmitted(true);
      // Redirect to login after 3 seconds
      setTimeout(() => {
        router.push('/login');
      }, 3000);
    } catch (_err) {
      setFormError(error || 'Error al restablecer la contraseña');
    }
  };

  if (!token && !formError) {
    return (
      <div className="w-full max-w-md space-y-6">
        <div className="text-center">
          <h1 className="text-3xl font-bold text-gray-900">Cargando...</h1>
        </div>
      </div>
    );
  }

  return (
    <div className="w-full max-w-md space-y-6">
      <div className="text-center">
        <h1 className="text-3xl font-bold text-gray-900">Restablecer Contraseña</h1>
        <p className="mt-2 text-gray-600">
          {submitted ? 'Tu contraseña ha sido restablecida exitosamente' : 'Ingresa tu nueva contraseña'}
        </p>
      </div>

      {!submitted ? (
        <form className="space-y-4" onSubmit={handleSubmit}>
          <Input
            label="Nueva Contraseña"
            type="password"
            placeholder="Ingresa tu nueva contraseña"
            required
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            disabled={loading}
          />

          <Input
            label="Confirmar Contraseña"
            type="password"
            placeholder="Confirma tu nueva contraseña"
            required
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            disabled={loading}
          />

          {(formError || error) && (
            <div className="rounded-md bg-red-50 p-3 text-sm text-red-600">{formError || error}</div>
          )}

          <Button type="submit" className="w-full" disabled={loading || !token}>
            {loading ? 'Restableciendo...' : 'Restablecer Contraseña'}
          </Button>
        </form>
      ) : (
        <div className="rounded-md bg-green-50 p-4">
          <div className="text-center">
            <h3 className="mb-2 text-lg font-medium text-green-800">¡Contraseña restablecida!</h3>
            <p className="mb-4 text-sm text-green-700">
              Tu contraseña ha sido cambiada exitosamente. Serás redirigido al inicio de sesión en unos momentos.
            </p>
            <p className="text-xs text-green-600">
              Si no eres redirigido automáticamente, haz clic en el enlace de abajo.
            </p>
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
