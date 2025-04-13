import { ReactNode } from 'react';

import Link from 'next/link';

import { Button } from '@/components/atoms/buttons/Button';

interface StaticPageLayoutProps {
  children: ReactNode;
  title: string;
}

export const StaticPageLayout = ({ children, title }: StaticPageLayoutProps) => {
  return (
    <div className="min-h-screen bg-gray-50 py-12">
      <div className="mx-auto max-w-3xl space-y-6 rounded-lg bg-white p-8 shadow-md">
        <div className="flex items-center justify-between border-b border-gray-200 pb-4">
          <h1 className="text-2xl font-bold text-gray-900">{title}</h1>
          <Link href="/login">
            <Button variant="outline" size="sm">
              Volver
            </Button>
          </Link>
        </div>
        <div className="prose max-w-none">{children}</div>
      </div>
    </div>
  );
};
