import { useEffect, useState } from 'react';

interface UseProfileLoadingState {
  isInitialLoad: boolean;
  isReady: boolean;
  showContent: boolean;
}

export const useProfileLoadingState = (
  profileLoading: boolean,
  languagesLoading: boolean,
  technologiesLoading: boolean,
  experiencesLoading: boolean,
  interestsLoading: boolean,
  _hasProfile: boolean = false,
): UseProfileLoadingState => {
  const [isInitialLoad, setIsInitialLoad] = useState(true);
  const [isReady, setIsReady] = useState(false);

  // Verificar si todos los datos críticos han terminado de cargar
  const allLoaded =
    !profileLoading && !languagesLoading && !technologiesLoading && !experiencesLoading && !interestsLoading;

  useEffect(() => {
    if (allLoaded && isInitialLoad) {
      // Pequeño delay para evitar flash, especialmente en conexiones rápidas
      const timer = setTimeout(() => {
        setIsReady(true);
        setIsInitialLoad(false);
      }, 100);

      return () => clearTimeout(timer);
    }
  }, [allLoaded, isInitialLoad]);

  // Reset when profile changes (navigation between users)
  useEffect(() => {
    if (profileLoading) {
      setIsInitialLoad(true);
      setIsReady(false);
    }
  }, [profileLoading]);

  return {
    isInitialLoad,
    isReady,
    showContent: isReady || (!isInitialLoad && allLoaded),
  };
};
