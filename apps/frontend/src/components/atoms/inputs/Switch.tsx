import React from 'react';

import { cn } from '@/lib/utils';

interface SwitchProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'onChange' | 'onCheckedChange'> {
  label?: string;
  onCheckedChange?: (checked: boolean) => void;
}

export const Switch = ({ className, label, checked, onCheckedChange, ...props }: SwitchProps) => {
  return (
    <label className="flex cursor-pointer items-center">
      <div className="relative">
        <input
          type="checkbox"
          className="sr-only"
          checked={checked}
          onChange={(e) => onCheckedChange && onCheckedChange(e.target.checked)}
          {...props}
        />
        <div className={cn('block h-6 w-10 rounded-full', checked ? 'bg-blue-600' : 'bg-gray-300', className)} />
        <div
          className={cn(
            'absolute top-1 left-1 h-4 w-4 rounded-full bg-white transition-transform',
            checked && 'translate-x-4',
          )}
        />
      </div>
      {label && <span className="ml-3 text-sm font-medium text-gray-700">{label}</span>}
    </label>
  );
};
