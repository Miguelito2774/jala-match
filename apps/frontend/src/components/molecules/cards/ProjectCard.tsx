import { useState } from 'react';

import { Button } from '@/components/atoms/buttons/Button';
import { cn } from '@/lib/utils';

import { ChevronDown, ChevronUp, Pencil, X } from 'lucide-react';

interface ProjectCardProps {
  project: {
    name: string;
    description: string;
    tools: string[];
    thirdParties: string[];
    framework: string[];
    libraries: string[];
    versionControl: string;
    projectManagement: string;
    responsibilities: string[];
  };
  onEdit: () => void;
  onRemove: () => void;
  className?: string;
}

export const ProjectCard = ({ project, onEdit, onRemove, className }: ProjectCardProps) => {
  const [expanded, setExpanded] = useState(false);

  return (
    <div className={cn('rounded-lg border-gray-200 bg-white shadow-sm border', className)}>
      <div className="p-4 flex items-center justify-between">
        <div className="flex items-center">
          <h4 className="font-medium text-gray-900">{project.name}</h4>
        </div>
        <div className="gap-2 flex">
          <Button variant="ghost" size="sm" onClick={() => setExpanded(!expanded)} className="text-gray-500">
            {expanded ? <ChevronUp className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />}
          </Button>
          <Button variant="ghost" size="sm" onClick={onEdit} className="text-gray-500">
            <Pencil className="h-4 w-4" />
          </Button>
          <Button variant="ghost" size="sm" onClick={onRemove} className="text-gray-500 hover:text-red-500">
            <X className="h-4 w-4" />
          </Button>
        </div>
      </div>

      {expanded && (
        <div className="border-gray-200 p-4 border-t">
          <div className="space-y-3 text-sm">
            <p className="text-gray-700">{project.description}</p>

            <div>
              <h5 className="font-medium text-gray-700">Herramientas:</h5>
              <div className="mt-1 gap-2 flex flex-wrap">
                {project.tools.map((tool, index) => (
                  <span key={index} className="bg-blue-100 px-2 py-0.5 text-xs text-blue-800 rounded-full">
                    {tool}
                  </span>
                ))}
              </div>
            </div>

            <div>
              <h5 className="font-medium text-gray-700">Frameworks:</h5>
              <div className="mt-1 gap-2 flex flex-wrap">
                {project.framework.map((fw, index) => (
                  <span key={index} className="bg-purple-100 px-2 py-0.5 text-xs text-purple-800 rounded-full">
                    {fw}
                  </span>
                ))}
              </div>
            </div>

            <div>
              <h5 className="font-medium text-gray-700">Responsabilidades:</h5>
              <ul className="ml-4 mt-1 list-disc">
                {project.responsibilities.map((resp, index) => (
                  <li key={index} className="text-gray-700">
                    {resp}
                  </li>
                ))}
              </ul>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
