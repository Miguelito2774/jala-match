export const buttonStyles = {
  primary:
    'transform bg-gradient-to-r from-blue-600 to-blue-700 px-8 py-2.5 text-white shadow-lg transition-all duration-200 hover:scale-105 hover:from-blue-700 hover:to-blue-800 hover:shadow-xl',

  secondary:
    'bg-gradient-to-r from-green-600 to-green-700 px-6 py-2.5 text-white shadow-md transition-all duration-200 hover:scale-105 hover:from-green-700 hover:to-green-800 hover:shadow-lg disabled:cursor-not-allowed disabled:opacity-50 disabled:hover:scale-100',

  outline: 'border-2 px-6 py-2.5 transition-all duration-200 hover:border-slate-300 hover:bg-slate-100',

  danger:
    'bg-gradient-to-r from-red-600 to-red-700 px-4 py-2 text-white shadow-lg transition-all duration-200 hover:scale-105 hover:from-red-700 hover:to-red-800 hover:shadow-xl focus:ring-2 focus:ring-red-600 focus:ring-offset-2',

  dangerOutline: 'border-2 px-4 py-2 text-gray-700 transition-all duration-200 hover:border-gray-400 hover:bg-gray-50',

  iconAction: 'h-8 w-8 p-0 transition-all duration-200',

  iconDelete: 'h-8 w-8 p-0 text-red-600 transition-all duration-200 hover:bg-red-50 hover:text-red-700',

  iconEdit: 'h-8 w-8 p-0 text-blue-600 transition-all duration-200 hover:bg-blue-50 hover:text-blue-700',

  utility:
    'bg-gradient-to-r from-purple-600 to-purple-700 px-4 py-2 text-white shadow-md transition-all duration-200 hover:from-purple-700 hover:to-purple-800 hover:shadow-lg',

  navigation:
    'bg-gradient-to-r from-indigo-600 to-indigo-700 px-6 py-2.5 text-white shadow-md transition-all duration-200 hover:from-indigo-700 hover:to-indigo-800 hover:shadow-lg',

  finish:
    'transform bg-gradient-to-r from-emerald-600 to-emerald-700 px-8 py-2.5 text-white shadow-lg transition-all duration-200 hover:scale-105 hover:from-emerald-700 hover:to-emerald-800 hover:shadow-xl',
};

export const combineButtonStyles = (baseStyle: string, additionalClasses?: string) => {
  return additionalClasses ? `${baseStyle} ${additionalClasses}` : baseStyle;
};

export const buttonStates = {
  disabled: 'disabled:cursor-not-allowed disabled:opacity-50 disabled:hover:scale-100',
  loading: 'cursor-wait opacity-75',
  active: 'ring-2 ring-offset-2',
};
