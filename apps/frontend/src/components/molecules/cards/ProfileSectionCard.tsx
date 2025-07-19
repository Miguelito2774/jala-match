import { cn } from '@/lib/utils';

import { CheckCircle2, ChevronRight } from 'lucide-react';

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
        'group cursor-pointer rounded-lg border-2 p-6 transition-all duration-200 hover:shadow-lg hover:shadow-blue-500/10',
        completed
          ? 'border-green-500 bg-gradient-to-r from-green-50 to-green-100 hover:from-green-100 hover:to-green-200'
          : 'border-slate-300 bg-white hover:border-blue-500 hover:bg-blue-50',
        'transform hover:-translate-y-1 hover:scale-105',
      )}
      onClick={onClick}
    >
      <div className="flex items-start justify-between">
        <div className="flex-1">
          <div className="flex items-center gap-3">
            <h3
              className={cn(
                'text-lg font-semibold transition-colors',
                completed ? 'text-green-800' : 'text-slate-900 group-hover:text-blue-700',
              )}
            >
              {title}
            </h3>
            {completed && <CheckCircle2 className="h-6 w-6 text-green-600" />}
          </div>
          <p
            className={cn(
              'mt-2 text-sm leading-relaxed',
              completed ? 'text-green-700' : 'text-slate-600 group-hover:text-blue-600',
            )}
          >
            {description}
          </p>
        </div>
        <ChevronRight
          className={cn(
            'h-5 w-5 transition-transform duration-200 group-hover:translate-x-1',
            completed ? 'text-green-600' : 'text-slate-400 group-hover:text-blue-600',
          )}
        />
      </div>

      <div className="mt-4 flex items-center gap-3">
        <div className="h-2 flex-1 rounded-full bg-slate-200">
          <div
            className={cn('h-2 rounded-full transition-all duration-300', completed ? 'bg-green-500' : 'bg-slate-400')}
            style={{ width: completed ? '100%' : '0%' }}
          />
        </div>
        <span className={cn('text-xs font-medium', completed ? 'text-green-700' : 'text-slate-500')}>
          {completed ? 'Completado' : 'Pendiente'}
        </span>
      </div>
    </div>
  );
};
