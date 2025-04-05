import React from 'react';

import { cn } from '@/lib/utils';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
}

export const Input = React.forwardRef<HTMLInputElement, InputProps>(({ className, label, error, ...props }, ref) => {
  return (
    <div className="space-y-1">
      {label && <label className="text-sm font-medium text-gray-700 block">{label}</label>}
      <input
        className={cn(
          'h-10 rounded-md border-gray-300 px-3 py-2 text-sm flex w-full border',
          'focus:ring-primary focus:border-transparent focus:ring-2 focus:outline-none',
          error && 'border-danger focus:ring-danger',
          className,
        )}
        ref={ref}
        {...props}
      />
      {error && <p className="text-danger text-sm">{error}</p>}
    </div>
  );
});

Input.displayName = 'Input';
