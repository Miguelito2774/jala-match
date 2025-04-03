import type { Metadata } from 'next';
import { Inter, Roboto_Mono } from 'next/font/google';

import { cn } from '@/lib/utils';

import './globals.css';

const inter = Inter({
  variable: '--font-inter',
  subsets: ['latin'],
});

const roboto = Roboto_Mono({
  variable: '--font-roboto-mono',
  subsets: ['latin'],
});

export const metadata: Metadata = {
  title: 'Jala Match',
  description: 'Jala Match - A web application for matching developers based on their interests and preferences.',
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="es">
      <body
        className={cn(
          inter.variable,
          roboto.variable,
          'font-sans antialiased',
          process.env.NODE_ENV === 'development' ? 'debug-screens' : '',
        )}
      >
        {children}
      </body>
    </html>
  );
}
