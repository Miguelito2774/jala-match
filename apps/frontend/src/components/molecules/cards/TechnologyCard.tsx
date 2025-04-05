import { Button } from '@/components/atoms/buttons/Button';
import { cn } from '@/lib/utils';

import { X } from 'lucide-react';

interface TechnologyCardProps {
  name: string;
  level: string;
  category: string;
  onRemove: () => void;
  className?: string;
}

export const TechnologyCard = ({ name, level, category, onRemove, className }: TechnologyCardProps) => {
  const getLevelColor = (level: string) => {
    switch (level) {
      case 'SFIA 1':
        return 'bg-gray-100 text-gray-800';
      case 'SFIA 2':
        return 'bg-blue-100 text-blue-800';
      case 'SFIA 3':
        return 'bg-green-100 text-green-800';
      case 'SFIA 4':
        return 'bg-yellow-100 text-yellow-800';
      case 'SFIA 5':
        return 'bg-purple-100 text-purple-800';
      case 'SFIA 6':
        return 'bg-pink-100 text-pink-800';
      case 'SFIA 7':
        return 'bg-red-100 text-red-800';
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
          <h4 className="font-medium text-gray-900">{name}</h4>
          <div className="mt-1 gap-2 flex flex-wrap">
            <span className={cn('px-2 py-0.5 text-xs font-medium rounded-full', getLevelColor(level))}>{level}</span>
            <span className="bg-gray-100 px-2 py-0.5 text-xs font-medium text-gray-800 rounded-full">{category}</span>
          </div>
        </div>
      </div>
      <Button variant="ghost" size="sm" onClick={onRemove} className="text-gray-500 hover:text-red-500">
        <X className="h-4 w-4" />
      </Button>
    </div>
  );
};
