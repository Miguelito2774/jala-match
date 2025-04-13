import React from 'react';

import { cn } from '@/lib/utils';

interface TextAreaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
}

export const TextArea = React.forwardRef<HTMLTextAreaElement, TextAreaProps>(
  ({ className, label, error, ...props }, ref) => {
    return (
      <div className="space-y-1">
        {label && <label className="block text-sm font-medium text-gray-700">{label}</label>}
        <textarea
          className={cn(
            'flex min-h-[100px] w-full rounded-md border border-gray-300 px-3 py-2 text-sm',
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
  },
);

TextArea.displayName = 'TextArea';
