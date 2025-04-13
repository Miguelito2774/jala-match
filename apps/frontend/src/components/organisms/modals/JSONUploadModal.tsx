'use client';

import { useState } from 'react';

import { Button } from '@/components/atoms/buttons/Button';
import { TextArea } from '@/components/atoms/inputs/TextArea';

import { Download, Upload, X } from 'lucide-react';

interface JSONUploadModalProps {
  onClose: () => void;
  onImport: (data: any, selectedFields: string[]) => void;
  templateData: any;
  section: string;
}

export const JSONUploadModal = ({ onClose, onImport, templateData, section }: JSONUploadModalProps) => {
  const [jsonInput, setJsonInput] = useState('');
  const [parsedData, setParsedData] = useState<any>(null);
  const [error, setError] = useState<string | null>(null);
  const [selectedFields, setSelectedFields] = useState<string[]>([]);

  const handleParseJSON = () => {
    try {
      const parsed = JSON.parse(jsonInput);
      setParsedData(parsed);
      setError(null);
      // Pre-select all fields
      if (parsed) {
        const fields = Object.keys(parsed);
        setSelectedFields(fields);
      }
    } catch (_e) {
      setError('JSON inválido. Por favor verifica el formato.');
    }
  };

  const handleFieldToggle = (field: string) => {
    if (selectedFields.includes(field)) {
      setSelectedFields(selectedFields.filter((f) => f !== field));
    } else {
      setSelectedFields([...selectedFields, field]);
    }
  };

  const downloadTemplate = () => {
    const dataStr = `data:text/json;charset=utf-8,${encodeURIComponent(JSON.stringify(templateData, null, 2))}`;
    const downloadAnchorNode = document.createElement('a');
    downloadAnchorNode.setAttribute('href', dataStr);
    downloadAnchorNode.setAttribute('download', `plantilla_${section}.json`);
    document.body.appendChild(downloadAnchorNode);
    downloadAnchorNode.click();
    downloadAnchorNode.remove();
  };

  return (
    <>
      <div className="bg-opacity-50 fixed inset-0 z-50 flex items-center justify-center bg-black p-4"></div>
      <div className="max-h-[90vh] w-full max-w-2xl overflow-auto rounded-lg bg-white p-6 shadow-xl">
        <div className="flex items-center justify-between border-b border-gray-200 pb-4">
          <h2 className="text-lg font-medium text-gray-900">Importar datos desde JSON</h2>
          <Button variant="ghost" size="sm" onClick={onClose}>
            <X className="h-5 w-5" />
          </Button>
        </div>

        <div className="my-4 space-y-4">
          <div className="flex justify-between">
            <p className="text-sm text-gray-600">
              Carga un archivo JSON con los datos para la sección {section} o usa nuestra plantilla.
            </p>
            <Button variant="outline" size="sm" onClick={downloadTemplate}>
              <Download className="mr-2 h-4 w-4" />
              Descargar plantilla
            </Button>
          </div>

          <div>
            <TextArea
              placeholder="Pega tu JSON aquí..."
              value={jsonInput}
              onChange={(e) => setJsonInput(e.target.value)}
              className="h-40 font-mono"
            />
            {error && <p className="mt-2 text-sm text-red-500">{error}</p>}
          </div>

          <div className="flex justify-end">
            <Button variant="primary" onClick={handleParseJSON} disabled={!jsonInput}>
              <Upload className="mr-2 h-4 w-4" />
              Analizar JSON
            </Button>
          </div>

          {parsedData && (
            <div className="mt-4 border-t border-gray-200 pt-4">
              <h3 className="mb-2 font-medium">Selecciona los campos a importar:</h3>
              <div className="grid grid-cols-2 gap-2">
                {Object.keys(parsedData).map((field) => (
                  <div key={field} className="flex items-center">
                    <input
                      type="checkbox"
                      id={field}
                      checked={selectedFields.includes(field)}
                      onChange={() => handleFieldToggle(field)}
                      className="mr-2 rounded border-gray-300"
                    />
                    <label htmlFor={field} className="text-sm">
                      {field}
                    </label>
                  </div>
                ))}
              </div>

              <div className="mt-4 flex justify-end space-x-2">
                <Button variant="outline" onClick={onClose}>
                  Cancelar
                </Button>
                <Button
                  variant="primary"
                  onClick={() => onImport(parsedData, selectedFields)}
                  disabled={selectedFields.length === 0}
                >
                  Importar datos
                </Button>
              </div>
            </div>
          )}
        </div>
      </div>
    </>
  );
};

export default JSONUploadModal;
