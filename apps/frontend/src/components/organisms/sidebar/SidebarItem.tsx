import { FC } from 'react';

import Link from 'next/link';
import { usePathname } from 'next/navigation';

import { cn } from '@/lib/utils';

import { IconType } from 'react-icons';

interface SidebarItemProps {
  href: string;
  icon: IconType;
  label: string;
  onNavigate?: () => void;
}

export const SidebarItem: FC<SidebarItemProps> = ({ href, icon: Icon, label, onNavigate }) => {
  const pathname = usePathname();
  const isActive = pathname === href;

  return (
    <Link
      href={href}
      onClick={onNavigate}
      className={cn(
        'flex items-center rounded-md px-3 py-2 text-sm font-medium transition-colors',
        isActive ? 'bg-blue-600 font-bold text-white' : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900',
      )}
    >
      <Icon className="mr-3 h-5 w-5 flex-shrink-0" />
      <span>{label}</span>
    </Link>
  );
};
