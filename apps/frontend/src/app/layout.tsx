import type { Metadata } from 'next';
import { Inter } from 'next/font/google';

import { AuthProvider } from '@/contexts/AuthContext';

import { Toaster } from 'sonner';

import './globals.css';

const inter = Inter({
  variable: '--font-inter',
  subsets: ['latin'],
});

export const metadata: Metadata = {
  title: 'Jala Match',
  description: 'Jala Match - A web application for matching developers based on their interests and preferences.',
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="es" className="h-full">
      <body className={`${inter.className} bg-brand-dark text-primary-white h-full`}>
        <AuthProvider>
          <div className="flex min-h-full flex-col">
            {children}
            <Toaster position="top-right" richColors />
          </div>
        </AuthProvider>
      </body>
    </html>
  );
}
