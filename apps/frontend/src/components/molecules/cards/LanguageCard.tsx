import { Button } from '@/components/atoms/buttons/Button';
import { cn } from '@/lib/utils';

import { X } from 'lucide-react';

interface LanguageCardProps {
  language: string;
  level: string;
  onRemove: () => void;
  className?: string;
}

export const LanguageCard = ({ language, level, onRemove, className }: LanguageCardProps) => {
  const getLevelColor = (level: string) => {
    switch (level.toLowerCase()) {
      case 'nativo':
        return 'bg-green-100 text-green-800';
      case 'c2':
      case 'c1':
        return 'bg-blue-100 text-blue-800';
      case 'b2':
      case 'b1':
        return 'bg-purple-100 text-purple-800';
      case 'a2':
      case 'a1':
        return 'bg-orange-100 text-orange-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  return (
    <div
      className={cn(
        'rounded-lg border-gray-200 bg-white p-3 shadow-sm flex items-center justify-between border',
        className,
      )}
    >
      <div className="flex items-center">
        <div>
          <h4 className="font-medium text-gray-900">{language}</h4>
          <div className={cn('mt-1 px-2 py-0.5 text-xs font-medium rounded-full', getLevelColor(level))}>{level}</div>
        </div>
      </div>
      <Button variant="ghost" size="sm" onClick={onRemove} className="text-gray-500 hover:text-red-500">
        <X className="h-4 w-4" />
      </Button>
    </div>
  );
};
