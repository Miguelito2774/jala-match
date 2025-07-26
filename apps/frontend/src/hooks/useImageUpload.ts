import { useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';
import { API_BASE_URL } from '@/lib/api';

import { toast } from 'sonner';

interface UploadImageResponse {
  url: string;
  publicId: string;
}

export const useImageUpload = () => {
  const [isUploading, setIsUploading] = useState(false);
  const { token } = useAuth();

  const uploadProfilePicture = async (file: File): Promise<UploadImageResponse | null> => {
    if (!file) {
      toast.error('Por favor selecciona una imagen');
      return null;
    }

    // Validar tipo de archivo
    if (!file.type.startsWith('image/')) {
      toast.error('Por favor selecciona un archivo de imagen válido');
      return null;
    }

    // Validar tamaño (ejemplo: máximo 5MB)
    const maxSize = 5 * 1024 * 1024; // 5MB
    if (file.size > maxSize) {
      toast.error('La imagen es muy grande. Máximo 5MB permitido');
      return null;
    }

    try {
      setIsUploading(true);

      // Convertir imagen a base64
      const base64 = await fileToBase64(file);

      const response = await fetch(`${API_BASE_URL}/api/images/profile-picture`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          imageBase64: base64,
        }),
      });

      if (!response.ok) {
        try {
          const errorData = await response.json();
          throw new Error(errorData.message || errorData.detail || `Error del servidor (${response.status})`);
        } catch (_parseError) {
          const errorText = await response.text();
          throw new Error(`Error del servidor (${response.status}): ${errorText || 'Error al subir la imagen'}`);
        }
      }

      const data: UploadImageResponse = await response.json();
      toast.success('Foto de perfil actualizada exitosamente');
      return data;
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Error al subir la imagen');
      return null;
    } finally {
      setIsUploading(false);
    }
  };

  const deleteProfilePicture = async (): Promise<boolean> => {
    try {
      const response = await fetch(`${API_BASE_URL}/api/images/profile-picture`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error('Error al eliminar la imagen');
      }

      toast.success('Foto de perfil eliminada exitosamente');
      return true;
    } catch (_error) {
      toast.error('Error al eliminar la imagen');
      return false;
    }
  };

  return {
    uploadProfilePicture,
    deleteProfilePicture,
    isUploading,
  };
};

// Función auxiliar para convertir File a base64
const fileToBase64 = (file: File): Promise<string> => {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => {
      // Remover el prefijo "data:image/...;base64,"
      const base64 = (reader.result as string).split(',')[1];
      resolve(base64);
    };
    reader.onerror = (error) => reject(error);
  });
};
