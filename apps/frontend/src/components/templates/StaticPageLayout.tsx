import { ReactNode } from 'react';

import Link from 'next/link';

import { Button } from '@/components/atoms/buttons/Button';

interface StaticPageLayoutProps {
  children: ReactNode;
  title: string;
}

export const StaticPageLayout = ({ children, title }: StaticPageLayoutProps) => {
  return (
    <div className="bg-gray-50 py-12 min-h-screen">
      <div className="max-w-3xl space-y-6 rounded-lg bg-white p-8 shadow-md mx-auto">
        <div className="border-gray-200 pb-4 flex items-center justify-between border-b">
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
