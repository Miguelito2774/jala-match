import { ReactNode } from 'react';

import ProfileGuard from './ProfileGuard';

interface ProfileLayoutProps {
  children: ReactNode;
}

export default function ProfileLayout({ children }: ProfileLayoutProps) {
  return (
    <ProfileGuard>
      <div className="flex min-h-screen">
        <main className="flex-1 bg-gray-50 p-6">{children}</main>
      </div>
    </ProfileGuard>
  );
}
