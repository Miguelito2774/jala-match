import { cn } from '@/lib/utils';

import { CheckCircle2 } from 'lucide-react';

interface ProfileSectionCardProps {
  title: string;
  description: string;
  completed: boolean;
  onClick?: () => void;
}

export const ProfileSectionCard = ({ title, description, completed, onClick }: ProfileSectionCardProps) => {
  return (
    <div
      className={cn(
        'rounded-lg p-4 hover:shadow-md cursor-pointer border transition-all',
        completed ? 'border-secondary' : 'border-gray-200',
        completed ? 'bg-green-50' : 'bg-white',
      )}
      onClick={onClick}
    >
      <div className="flex items-start justify-between">
        <div>
          <h3 className="font-medium text-gray-900">{title}</h3>
          <p className="mt-1 text-sm text-gray-500">{description}</p>
        </div>
        {completed && <CheckCircle2 className="text-secondary h-5 w-5" />}
      </div>
      <div className="mt-3 flex items-center">
        <div className="h-1.5 bg-gray-200 w-full rounded-full">
          <div className="bg-secondary h-1.5 rounded-full" style={{ width: completed ? '100%' : '0%' }} />
        </div>
        <span className="ml-2 text-xs text-gray-500">{completed ? 'Completado' : '0%'}</span>
      </div>
    </div>
  );
};
