import Link from 'next/link';
import { usePathname } from 'next/navigation';

import { cn } from '@/lib/utils';

import { IconType } from 'react-icons';

interface SidebarItemProps {
  href: string;
  icon: IconType;
  label: string;
}

export const SidebarItem = ({ href, icon: Icon, label }: SidebarItemProps) => {
  const pathname = usePathname();
  const isActive = pathname === href;

  return (
    <Link
      href={href}
      className={cn(
        'group rounded-md px-3 py-2 text-sm font-medium flex items-center',
        isActive ? 'bg-primary text-white' : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900',
      )}
    >
      <Icon
        className={cn(
          'mr-3 h-5 w-5 flex-shrink-0',
          isActive ? 'text-white' : 'text-gray-400 group-hover:text-gray-500',
        )}
      />
      <span>{label}</span>
    </Link>
  );
};
