import { Button } from '@/components/atoms/buttons/Button';
import { cn } from '@/lib/utils';

import { X } from 'lucide-react';

interface InterestCardProps {
  interest: {
    name: string;
    duration: string;
    frequency: string;
    likertScale: number;
  };
  onRemove: () => void;
  className?: string;
}

export const InterestCard = ({ interest, onRemove, className }: InterestCardProps) => {
  return (
    <div
      className={cn(
        'flex items-center justify-between rounded-lg border border-gray-200 bg-white p-3 shadow-sm',
        className,
      )}
    >
      <div className="flex items-center">
        <div>
          <h4 className="font-medium text-gray-900">{interest.name}</h4>
          <div className="mt-1 flex flex-wrap gap-2 text-xs text-gray-500">
            <span>{interest.duration} por sesión</span>
            <span>•</span>
            <span>{interest.frequency}</span>
            <span>•</span>
            <span>Nivel de interés: {interest.likertScale}/5</span>
          </div>
        </div>
      </div>
      <Button variant="ghost" size="sm" onClick={onRemove} className="text-gray-500 hover:text-red-500">
        <X className="h-4 w-4" />
      </Button>
    </div>
  );
};
