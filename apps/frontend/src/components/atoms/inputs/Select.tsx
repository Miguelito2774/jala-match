'use client';

import React, { useState } from 'react';

import { cn } from '@/lib/utils';

import { ChevronDown, X } from 'lucide-react';

interface SelectOption<T = any> {
  value: T;
  label: string;
}

type SelectProps<T> = {
  options: SelectOption<T>[];
  onChange: (selected: SelectOption<T> | SelectOption<T>[] | null) => void;
  isMulti?: boolean;
  defaultValue?: SelectOption<T> | SelectOption<T>[];
  className?: string;
  placeholder?: string;
  value?: SelectOption<T> | SelectOption<T>[];
  isDisabled?: boolean;
};

export const Select = <T,>({
  options,
  onChange,
  isMulti = false,
  defaultValue,
  className,
  placeholder,
  value,
  isDisabled = false,
}: SelectProps<T>) => {
  const [isOpen, setIsOpen] = useState(false);
  const [selected, setSelected] = useState<SelectOption<T> | null>(
    defaultValue && !Array.isArray(defaultValue) ? defaultValue : null,
  );
  const [multiSelected, setMultiSelected] = useState<SelectOption<T>[]>(
    (Array.isArray(defaultValue) ? defaultValue : []) as SelectOption<T>[],
  );

  const handleSelect = (option: SelectOption<T>) => {
    if (isMulti) {
      const alreadySelected = multiSelected.some((item) => item.value === option.value);
      let newSelected: SelectOption<T>[];

      if (alreadySelected) {
        newSelected = multiSelected.filter((item) => item.value !== option.value);
      } else {
        newSelected = [...multiSelected, option];
      }

      setMultiSelected(newSelected);
      onChange(newSelected);
    } else {
      setSelected(option);
      onChange(option);
      setIsOpen(false);
    }
  };

  const currentValue = isMulti ? (value as SelectOption<T>[]) || multiSelected : (value as SelectOption<T>) || selected;

  return (
    <div className={cn('relative', className)}>
      <div
        className={cn(
          'flex w-full items-center justify-between rounded-md border border-gray-300 p-2',
          'focus:ring-primary focus:border-transparent focus:ring-2 focus:outline-none',
          isOpen && !isDisabled && 'ring-primary border-transparent ring-2',
          isDisabled && 'cursor-not-allowed opacity-50',
        )}
        onClick={() => !isDisabled && setIsOpen(!isOpen)}
      >
        <div className="flex flex-wrap gap-1">
          {isMulti ? (
            (currentValue as SelectOption<T>[]).length > 0 ? (
              (currentValue as SelectOption<T>[]).map((item) => (
                <span
                  key={`selected-${item.value}`}
                  className="bg-primary/10 text-primary inline-flex items-center rounded px-2 py-1 text-xs"
                >
                  {item.label}
                  <button
                    type="button"
                    onClick={(e) => {
                      e.stopPropagation();
                      const newSelected = (currentValue as SelectOption<T>[]).filter((i) => i.value !== item.value);
                      setMultiSelected(newSelected);
                      onChange(newSelected);
                    }}
                    className="ml-1"
                  >
                    <X className="h-3 w-3" />
                  </button>
                </span>
              ))
            ) : (
              <span className="text-gray-400">{placeholder || 'Seleccionar...'}</span>
            )
          ) : (
            <span>{(currentValue as SelectOption<T>)?.label || placeholder || 'Seleccionar...'}</span>
          )}
        </div>
        <ChevronDown className={cn('h-4 w-4 text-gray-400 transition-transform', isOpen && 'rotate-180 transform')} />
      </div>

      {isOpen && (
        <div className="absolute z-10 mt-1 max-h-60 w-full overflow-auto rounded-md border border-gray-200 bg-white py-1 shadow-lg">
          {options.map((option, index) => {
            const isSelected = isMulti
              ? (currentValue as SelectOption<T>[]).some((item) => item.value === option.value)
              : (currentValue as SelectOption<T>)?.value === option.value;

            return (
              <div
                key={`option-${option.value ?? index}`}
                className={cn('cursor-pointer px-3 py-2 hover:bg-gray-100', isSelected && 'bg-primary/10 text-primary')}
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
