'use client';

import { Loader2 } from 'lucide-react';

interface PageLoaderProps {
  title?: string;
  subtitle?: string;
}

export const PageLoader = ({ title = 'Cargando...', subtitle = 'Por favor espere' }: PageLoaderProps) => {
  return (
    <div className="flex min-h-[60vh] items-center justify-center">
      <div className="space-y-4 text-center">
        <Loader2 className="mx-auto h-8 w-8 animate-spin text-blue-600" />
        <div className="space-y-2">
          <h3 className="text-lg font-medium text-gray-900">{title}</h3>
          <p className="text-sm text-gray-500">{subtitle}</p>
        </div>
      </div>
    </div>
  );
};

interface SectionLoaderProps {
  height?: string;
}

export const SectionLoader = ({ height = 'h-32' }: SectionLoaderProps) => {
  return (
    <div className={`flex items-center justify-center ${height} animate-pulse rounded-lg bg-gray-50`}>
      <Loader2 className="h-6 w-6 animate-spin text-gray-400" />
    </div>
  );
};
