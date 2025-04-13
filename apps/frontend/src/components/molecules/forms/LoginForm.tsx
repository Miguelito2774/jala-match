'use client';

import { useState } from 'react';

import { useRouter } from 'next/navigation';

import { Button } from '@/components/atoms/buttons/Button';
import { Input } from '@/components/atoms/inputs/Input';

export default function LoginForm() {
  const router = useRouter();
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    // Lógica de autenticación...

    const userRole = 'manager'; // o 'manager' según respuesta del API

    if (userRole === 'manager') {
      router.push('/team-builder');
    }

    // Si es empleado, redirigir a la página de perfil
    //const userRole = 'employee'; // o 'employee' según respuesta del API
    //if (userRole === 'employee') {
    //router.push('/profile');
    //}
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Input
        label="Email"
        type="email"
        value={formData.email}
        onChange={(e) => setFormData({ ...formData, email: e.target.value })}
      />

      <Input
        label="Contraseña"
        type="password"
        value={formData.password}
        onChange={(e) => setFormData({ ...formData, password: e.target.value })}
      />

      <Button type="submit" variant="primary" className="w-full">
        Iniciar Sesión
      </Button>

      <div className="text-center">
        <a href="/forgot-password" className="text-sm text-blue-500 hover:underline">
          ¿Olvidaste tu contraseña?
        </a>
      </div>
    </form>
  );
}
