import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export interface SelectOption {
  value: any;
  label: string;
}

export function getSelectValue(options: SelectOption[], value: any) {
  return options.find((option) => option.value === value);
}
