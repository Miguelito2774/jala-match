'use client';

import React, { useState } from 'react';

import { cn } from '@/lib/utils';

import { ChevronDown, X } from 'lucide-react';

interface SelectProps {
  options: { value: any; label: string }[];
  onChange: (selected: { value: any; label: string }) => void;
  isMulti?: boolean;
  defaultValue?: { value: any; label: string };
  className?: string;
}

export const Select = ({ options, onChange, isMulti = false, defaultValue, className }: SelectProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const [selected, setSelected] = useState<{ value: any; label: string } | null>(defaultValue || null);
  const [multiSelected, setMultiSelected] = useState<{ value: any; label: string }[]>([]);

  const handleSelect = (option: { value: any; label: string }) => {
    if (isMulti) {
      const alreadySelected = multiSelected.some((item) => item.value === option.value);
      if (alreadySelected) {
        setMultiSelected(multiSelected.filter((item) => item.value !== option.value));
      } else {
        setMultiSelected([...multiSelected, option]);
      }
    } else {
      setSelected(option);
      onChange(option);
      setIsOpen(false);
    }
  };

  return (
    <div className={cn('relative', className)}>
      <div
        className={cn(
          'rounded-md border-gray-300 p-2 flex w-full cursor-pointer items-center justify-between border',
          'focus:ring-primary focus:border-transparent focus:ring-2 focus:outline-none',
          isOpen && 'ring-primary border-transparent ring-2',
        )}
        onClick={() => setIsOpen(!isOpen)}
      >
        <div className="gap-1 flex flex-wrap">
          {isMulti ? (
            multiSelected.length > 0 ? (
              multiSelected.map((item) => (
                <span
                  key={item.value}
                  className="bg-primary/10 text-primary rounded px-2 py-1 text-xs inline-flex items-center"
                >
                  {item.label}
                  <button
                    type="button"
                    onClick={(e) => {
                      e.stopPropagation();
                      setMultiSelected(multiSelected.filter((i) => i.value !== item.value));
                    }}
                    className="ml-1"
                  >
                    <X className="h-3 w-3" />
                  </button>
                </span>
              ))
            ) : (
              <span className="text-gray-400">Seleccionar...</span>
            )
          ) : (
            <span>{selected?.label || 'Seleccionar...'}</span>
          )}
        </div>
        <ChevronDown className={cn('h-4 w-4 text-gray-400 transition-transform', isOpen && 'rotate-180 transform')} />
      </div>

      {isOpen && (
        <div className="mt-1 max-h-60 rounded-md border-gray-200 bg-white py-1 shadow-lg absolute z-10 w-full overflow-auto border">
          {options.map((option) => {
            const isSelected = isMulti
              ? multiSelected.some((item) => item.value === option.value)
              : selected?.value === option.value;

            return (
              <div
                key={option.value}
                className={cn('px-3 py-2 hover:bg-gray-100 cursor-pointer', isSelected && 'bg-primary/10 text-primary')}
                onClick={() => handleSelect(option)}
              >
                {option.label}
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
};
