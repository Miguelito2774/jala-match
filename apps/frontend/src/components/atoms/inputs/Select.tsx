'use client';

import React, { useEffect, useRef, useState } from 'react';

import { cn } from '@/lib/utils';

import { ChevronDown, Search, X } from 'lucide-react';

interface SelectOption<T = any> {
  value: T;
  label: string;
  [key: string]: any;
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
  formatOptionLabel?: (option: SelectOption<T>) => React.ReactNode;
  isSearchable?: boolean;
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
  formatOptionLabel,
  isSearchable = false,
}: SelectProps<T>) => {
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [selected, setSelected] = useState<SelectOption<T> | null>(
    defaultValue && !Array.isArray(defaultValue) ? defaultValue : null,
  );
  const [multiSelected, setMultiSelected] = useState<SelectOption<T>[]>(
    (Array.isArray(defaultValue) ? defaultValue : []) as SelectOption<T>[],
  );

  const containerRef = useRef<HTMLDivElement>(null);
  const searchInputRef = useRef<HTMLInputElement>(null);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        setIsOpen(false);
        setSearchTerm('');
      }
    };

    if (isOpen) {
      document.addEventListener('mousedown', handleClickOutside);
    }

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [isOpen]);

  // Focus search input when dropdown opens
  useEffect(() => {
    if (isOpen && isSearchable && searchInputRef.current) {
      searchInputRef.current.focus();
    }
  }, [isOpen, isSearchable]);

  // Filter options based on search term
  const filteredOptions =
    isSearchable && searchTerm
      ? options.filter((option) => option.label.toLowerCase().includes(searchTerm.toLowerCase()))
      : options;

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
      setSearchTerm('');
    }
  };

  const handleToggleDropdown = () => {
    if (!isDisabled) {
      setIsOpen(!isOpen);
      if (!isOpen) {
        setSearchTerm('');
      }
    }
  };

  const currentValue = isMulti ? (value as SelectOption<T>[]) || multiSelected : (value as SelectOption<T>) || selected;

  return (
    <div className={cn('relative', className)} ref={containerRef}>
      <div
        className={cn(
          'flex w-full items-center justify-between rounded-md border border-gray-300 p-2',
          'focus:ring-primary focus:border-transparent focus:ring-2 focus:outline-none',
          isOpen && !isDisabled && 'ring-primary border-transparent ring-2',
          isDisabled && 'cursor-not-allowed opacity-50',
        )}
        onClick={handleToggleDropdown}
      >
        <div className="flex flex-wrap gap-1">
          {isMulti ? (
            (currentValue as SelectOption<T>[]).length > 0 ? (
              (currentValue as SelectOption<T>[]).map((item) => (
                <span
                  key={`selected-${item.value}`}
                  className="bg-primary/10 text-primary inline-flex items-center rounded px-2 py-1 text-xs"
                >
                  {formatOptionLabel ? formatOptionLabel(item) : item.label}
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
          ) : // Single select: if no current value, show placeholder, else show formatted label
          (currentValue as SelectOption<T>) ? (
            <>
              {formatOptionLabel
                ? formatOptionLabel(currentValue as SelectOption<T>)
                : (currentValue as SelectOption<T>).label}
            </>
          ) : (
            <span className="text-gray-400">{placeholder || 'Seleccionar...'}</span>
          )}
        </div>
        <ChevronDown className={cn('h-4 w-4 text-gray-400 transition-transform', isOpen && 'rotate-180 transform')} />
      </div>

      {isOpen && (
        <div className="absolute z-[100] mt-1 max-h-60 w-full overflow-auto rounded-md border border-gray-200 bg-white py-1 shadow-lg">
          {isSearchable && (
            <div className="sticky top-0 border-b border-gray-200 bg-white p-2">
              <div className="relative">
                <Search className="absolute top-1/2 left-2 h-4 w-4 -translate-y-1/2 text-gray-400" />
                <input
                  ref={searchInputRef}
                  type="text"
                  placeholder="Buscar..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-full rounded border border-gray-300 py-1 pr-2 pl-8 text-sm focus:ring-1 focus:ring-blue-500 focus:outline-none"
                  onClick={(e) => e.stopPropagation()}
                />
              </div>
            </div>
          )}
          {filteredOptions.length > 0 ? (
            filteredOptions.map((option, index) => {
              const isSelected = isMulti
                ? (currentValue as SelectOption<T>[]).some((item) => item.value === option.value)
                : (currentValue as SelectOption<T>)?.value === option.value;

              return (
                <div
                  key={`option-${option.value ?? index}`}
                  className={cn(
                    'cursor-pointer px-3 py-2 hover:bg-gray-100',
                    isSelected && 'bg-primary/10 text-primary',
                  )}
                  onClick={() => handleSelect(option)}
                >
                  {formatOptionLabel ? formatOptionLabel(option) : option.label}
                </div>
              );
            })
          ) : (
            <div className="px-3 py-2 text-sm text-gray-500">No se encontraron resultados</div>
          )}
        </div>
      )}
    </div>
  );
};
