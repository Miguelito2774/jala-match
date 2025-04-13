/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: ['class'],
  content: [
    './app//**/*.{js,ts,jsx,tsx,mdx}',
    './components/**/*.{js,ts,jsx,tsx,mdx}',
    './src/**/*.{js,ts,jsx,tsx,mdx}',
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          blue: '#2563EB',
          white: '#FFFFFF',
        },
        button: {
          green: '#10B981',
        },
        brand: {
          dark: '#282827',
          red: '#ED2A3D',
        },
      },
    },
  },
  plugins: [],
};
