import { useCallback, useEffect, useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';
import { API_BASE_URL } from '@/lib/api';

import { toast } from 'sonner';

interface ConsentSettings {
  teamMatchingAnalysis: boolean;
  lastUpdated: string;
  version: string;
}

interface DeletionResponse {
  requestId: string;
  scheduledDeletionDate: string;
  message: string;
}

export const usePrivacySettings = () => {
  const { token } = useAuth();
  const [consentSettings, setConsentSettings] = useState<ConsentSettings>({
    teamMatchingAnalysis: true,
    lastUpdated: new Date().toISOString(),
    version: '1.0',
  });

  const [loading, setLoading] = useState(true);
  const [updating, setUpdating] = useState(false);

  const loadConsentSettings = useCallback(async () => {
    try {
      setLoading(true);
      const authToken = token || localStorage.getItem('auth_token');

      if (!authToken) {
        // Don't show error toast when user is not authenticated
        setLoading(false);
        return;
      }

      const response = await fetch(`${API_BASE_URL}/api/privacy/consents`, {
        headers: {
          Authorization: `Bearer ${authToken}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to load consent settings: ${response.status} ${errorText}`);
      }

      const data = await response.json();
      setConsentSettings({
        teamMatchingAnalysis: data.teamMatchingAnalysis,
        lastUpdated: data.lastUpdated,
        version: data.version,
      });
    } catch (_error) {
      // Only show error toast if we have a token (user is authenticated)
      const authToken = token || localStorage.getItem('auth_token');
      if (authToken) {
        toast.error('Error al cargar la configuración de privacidad');
      }

      // Set default values if loading fails
      setConsentSettings({
        teamMatchingAnalysis: true,
        lastUpdated: new Date().toISOString(),
        version: '1.0',
      });
    } finally {
      setLoading(false);
    }
  }, [token]);

  // Load initial consent settings only when authenticated
  useEffect(() => {
    const authToken = token || localStorage.getItem('auth_token');
    if (authToken) {
      loadConsentSettings();
    } else {
      setLoading(false);
    }
  }, [loadConsentSettings, token]);

  const updateConsentSettings = async (key: keyof ConsentSettings, value: boolean) => {
    try {
      setUpdating(true);

      const updatedSettings = {
        ...consentSettings,
        [key]: value,
      };

      const response = await fetch(`${API_BASE_URL}/api/privacy/consents`, {
        method: 'PUT',
        headers: {
          Authorization: `Bearer ${token || localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          teamMatchingAnalysis: updatedSettings.teamMatchingAnalysis,
        }),
      });

      if (!response.ok) {
        throw new Error('Failed to update consent settings');
      }

      setConsentSettings({
        ...updatedSettings,
        lastUpdated: new Date().toISOString(),
      });

      toast.success('Configuración de consentimiento actualizada');
    } catch (_error) {
      toast.error('Error al actualizar la configuración');
    } finally {
      setUpdating(false);
    }
  };

  const exportUserData = async (): Promise<void> => {
    try {
      toast.loading('Preparando exportación de datos...');

      const response = await fetch(`${API_BASE_URL}/api/privacy/export`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${token || localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error('Failed to export data');
      }

      // Create download link
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `jala-match-data-export-${new Date().toISOString().split('T')[0]}.json`;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);

      toast.dismiss();
      toast.success('Datos exportados exitosamente');
    } catch (_error) {
      toast.dismiss();
      toast.error('Error al exportar los datos');
    }
  };

  const requestDataDeletion = async (dataTypes: string[] = []): Promise<DeletionResponse | null> => {
    try {
      const response = await fetch(`${API_BASE_URL}/api/privacy/reset-profile`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${token || localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          dataTypes,
          reason: 'User requested profile reset',
        }),
      });

      if (!response.ok) {
        throw new Error('Failed to reset profile');
      }

      const data = await response.json();
      toast.success('Perfil reiniciado exitosamente. Puedes completar tu información nuevamente.');

      return {
        requestId: 'immediate',
        scheduledDeletionDate: new Date().toISOString(),
        message: data.message || 'Perfil reiniciado exitosamente',
      };
    } catch (_error) {
      toast.error('Error al reiniciar el perfil');
      return null;
    }
  };

  const cancelDataDeletion = async (requestId: string): Promise<boolean> => {
    try {
      const response = await fetch(`${API_BASE_URL}/api/privacy/delete-request/${requestId}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${token || localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error('Failed to cancel data deletion');
      }

      toast.success('Solicitud de eliminación cancelada exitosamente');
      return true;
    } catch (_error) {
      toast.error('Error al cancelar la solicitud de eliminación');
      return false;
    }
  };

  return {
    consentSettings,
    loading,
    updating,
    updateConsentSettings,
    exportUserData,
    requestDataDeletion,
    cancelDataDeletion,
    refreshSettings: loadConsentSettings,
  };
};
