'use client';

import { useEffect } from 'react';

import { useRouter } from 'next/navigation';

import { useAuth } from '@/contexts/AuthContext';

export default function AdminPage() {
  const { user, isLoading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!isLoading && user?.role === 'Admin') {
      router.replace('/dashboard');
    }
  }, [user, isLoading, router]);

  return null;
}
