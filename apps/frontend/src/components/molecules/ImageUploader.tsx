import React, { useRef, useState } from 'react';

import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import { useImageUpload } from '@/hooks/useImageUpload';

import { Camera, Loader2, Trash2 } from 'lucide-react';

interface ImageUploaderProps {
  currentImageUrl?: string;
  onImageChange: (imageUrl: string, publicId: string) => void;
  onImageRemove?: () => void;
  placeholder?: string;
  className?: string;
  size?: 'sm' | 'md' | 'lg';
}

export const ImageUploader: React.FC<ImageUploaderProps> = ({
  currentImageUrl,
  onImageChange,
  onImageRemove,
  placeholder = '??',
  className = '',
  size = 'lg',
}) => {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const { uploadProfilePicture, deleteProfilePicture, isUploading } = useImageUpload();

  const sizeClasses = {
    sm: 'h-12 w-12',
    md: 'h-16 w-16',
    lg: 'h-20 w-20',
  };

  const handleFileSelect = () => {
    fileInputRef.current?.click();
  };

  const handleFileChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    const objectUrl = URL.createObjectURL(file);
    setPreviewUrl(objectUrl);

    try {
      const result = await uploadProfilePicture(file);
      if (result) {
        onImageChange(result.url, result.publicId);
        setPreviewUrl(null);
      } else {
        setPreviewUrl(null);
      }
    } catch {
      setPreviewUrl(null);
    }

    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  const handleRemoveImage = async () => {
    if (onImageRemove) {
      const success = await deleteProfilePicture();
      if (success) {
        onImageRemove();
      }
    }
  };

  const displayUrl = previewUrl || currentImageUrl;

  return (
    <div className={`relative ${className}`}>
      <Avatar className={`${sizeClasses[size]} border-4 border-white shadow-lg`}>
        <AvatarImage src={displayUrl || undefined} />
        <AvatarFallback className="bg-gradient-to-br from-blue-500 to-purple-600 text-lg font-semibold text-white">
          {placeholder}
        </AvatarFallback>
      </Avatar>

      <Button
        type="button"
        size="sm"
        variant="outline"
        onClick={handleFileSelect}
        disabled={isUploading}
        className="absolute -right-2 -bottom-2 h-8 w-8 rounded-full bg-white p-0 shadow-sm"
      >
        {isUploading ? <Loader2 className="h-4 w-4 animate-spin" /> : <Camera className="h-4 w-4" />}
      </Button>

      {currentImageUrl && onImageRemove && (
        <Button
          type="button"
          size="sm"
          variant="outline"
          onClick={handleRemoveImage}
          disabled={isUploading}
          className="absolute -bottom-2 -left-2 h-8 w-8 rounded-full border-red-200 bg-white p-0 text-red-600 shadow-sm hover:bg-red-50"
        >
          <Trash2 className="h-4 w-4" />
        </Button>
      )}

      <input ref={fileInputRef} type="file" accept="image/*" onChange={handleFileChange} className="hidden" />

      {isUploading && (
        <div className="absolute inset-0 flex items-center justify-center rounded-full bg-black/50">
          <Loader2 className="h-6 w-6 animate-spin text-white" />
        </div>
      )}
    </div>
  );
};
