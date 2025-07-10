'use client';

import React, { useState } from 'react';

import { useRouter } from 'next/navigation';

import { Select } from '@/components/atoms/inputs/Select';
import { TextArea } from '@/components/atoms/inputs/TextArea';
import { PageLoader } from '@/components/atoms/loaders/PageLoader';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Progress } from '@/components/ui/progress';
import { WorkExperience, useWorkExperiences } from '@/hooks/useEmployeeProfile';
import { useProfileLoadingState } from '@/hooks/useProfileLoadingState';
import { buttonStyles } from '@/lib/buttonStyles';

import { Briefcase, Calendar, Download, Edit, FileText, Plus, Trash2, Upload, X } from 'lucide-react';
import { toast } from 'sonner';

const versionControlOptions = [
  { value: 'Git', label: 'Git' },
  { value: 'SVN', label: 'Subversion (SVN)' },
  { value: 'Mercurial', label: 'Mercurial' },
  { value: 'Perforce', label: 'Perforce' },
  { value: 'Bazaar', label: 'Bazaar' },
  { value: 'CVS', label: 'CVS' },
  { value: 'Other', label: 'Otro' },
];

const projectManagementOptions = [
  { value: 'Scrum', label: 'Scrum' },
  { value: 'Kanban', label: 'Kanban' },
  { value: 'Waterfall', label: 'Waterfall' },
  { value: 'Agile', label: 'Agile' },
  { value: 'Lean', label: 'Lean' },
  { value: 'DevOps', label: 'DevOps' },
  { value: 'Traditional', label: 'Tradicional' },
  { value: 'Other', label: 'Otro' },
];

const commonTools = [
  'Visual Studio Code',
  'IntelliJ IDEA',
  'Eclipse',
  'Sublime Text',
  'Atom',
  'Postman',
  'Insomnia',
  'Docker',
  'Kubernetes',
  'Jenkins',
  'GitLab CI',
  'GitHub Actions',
  'Travis CI',
  'CircleCI',
  'AWS CLI',
  'Azure CLI',
  'Terraform',
  'Ansible',
  'Chef',
  'Puppet',
  'Vagrant',
  'Jira',
  'Confluence',
  'Slack',
  'Microsoft Teams',
  'Zoom',
  'Figma',
  'Adobe XD',
  'Sketch',
];

const commonFrameworks = [
  'React',
  'Angular',
  'Vue.js',
  'Next.js',
  'Nuxt.js',
  'Express.js',
  'NestJS',
  'Django',
  'Flask',
  'FastAPI',
  'Spring Boot',
  'Laravel',
  'Ruby on Rails',
  'ASP.NET',
  '.NET Core',
  'Symfony',
  'CodeIgniter',
  'Bootstrap',
  'Tailwind CSS',
  'Material-UI',
  'Ant Design',
  'Chakra UI',
];

const commonThirdParties = [
  'Google Analytics',
  'Google Maps',
  'Stripe',
  'PayPal',
  'Twilio',
  'SendGrid',
  'Mailgun',
  'Auth0',
  'Firebase',
  'AWS Services',
  'Azure Services',
  'Google Cloud',
  'Cloudflare',
  'MongoDB Atlas',
  'Redis Cloud',
  'Elasticsearch',
  'Algolia',
  'Sentry',
  'LogRocket',
  'Mixpanel',
  'Amplitude',
  'Intercom',
  'Zendesk',
  'Slack API',
];

interface JsonImportData {
  projectName: string;
  description: string;
  tools: string[];
  thirdParties: string[];
  frameworks: string[];
  versionControl: string;
  projectManagement: string;
  responsibilities: string[];
  startDate: string;
  endDate?: string;
  selected: boolean;
}

export default function WorkExperiencePage() {
  const router = useRouter();
  const { experiences, loading, addExperience, updateExperience, deleteExperience } = useWorkExperiences();

  // Track changes for dynamic button (no form state to track since it's just a list)
  const [hasChanges, setHasChanges] = useState(false);

  // Track when experiences are added/updated/deleted
  const trackChange = () => {
    setHasChanges(true);
    // Reset to false after 30 seconds
    setTimeout(() => setHasChanges(false), 30000);
  };

  // Check for incomplete fields and generate suggestions
  const getIncompleteSuggestions = () => {
    const incomplete = [];
    if (experiences.length === 0) incomplete.push('al menos una experiencia laboral');
    return incomplete;
  };

  // Handle navigation with suggestions
  const handleContinue = () => {
    const incompleteFields = getIncompleteSuggestions();
    if (incompleteFields.length > 0) {
      setIncompleteFieldsList(incompleteFields);
      setIsConfirmDialogOpen(true);
      return;
    }
    router.push('/profile/interests');
  };

  // Confirm navigation with incomplete fields
  const confirmContinue = () => {
    setIsConfirmDialogOpen(false);
    router.push('/profile/interests');
  };

  // Form state
  const [projectName, setProjectName] = useState('');
  const [description, setDescription] = useState('');
  const [selectedTools, setSelectedTools] = useState<string[]>([]);
  const [selectedThirdParties, setSelectedThirdParties] = useState<string[]>([]);
  const [selectedFrameworks, setSelectedFrameworks] = useState<string[]>([]);
  const [versionControl, setVersionControl] = useState('');
  const [projectManagement, setProjectManagement] = useState('');
  const [responsibilities, setResponsibilities] = useState<string[]>(['', '', '']);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [isCurrentProject, setIsCurrentProject] = useState(false);

  // Modal states
  const [isAddingProject, setIsAddingProject] = useState(false);
  const [editingProject, setEditingProject] = useState<WorkExperience | null>(null);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [experienceToDelete, setExperienceToDelete] = useState<{ id: string; name: string } | null>(null);

  // Confirmation dialog for incomplete fields
  const [isConfirmDialogOpen, setIsConfirmDialogOpen] = useState(false);
  const [incompleteFieldsList, setIncompleteFieldsList] = useState<string[]>([]);

  // JSON Import states
  const [jsonImportOpen, setJsonImportOpen] = useState(false);
  const [jsonInput, setJsonInput] = useState('');
  const [importPreview, setImportPreview] = useState<JsonImportData[]>([]);
  const [isImporting, setIsImporting] = useState(false);

  // Loading state management
  const loadingState = useProfileLoadingState(
    false, // profileLoading - not used in this page
    false, // languagesLoading - not used in this page
    false, // technologiesLoading - not used in this page
    loading, // experiencesLoading
    false, // interestsLoading - not used in this page
    experiences.length > 0, // hasData
  );

  // Early return for loading state
  if (!loadingState.showContent) {
    return (
      <PageLoader
        title="Cargando experiencias laborales..."
        subtitle="Por favor espere mientras cargamos su información"
      />
    );
  }

  const calculateProgress = () => {
    return experiences.length > 0 ? 100 : 0;
  };

  const populateFormWithExperience = (experience: WorkExperience) => {
    setProjectName(experience.projectName);
    setDescription(experience.description || '');
    setSelectedTools(experience.tools || []);
    setSelectedThirdParties(experience.thirdParties || []);
    setSelectedFrameworks(experience.frameworks || []);
    setVersionControl(experience.versionControl || '');
    setProjectManagement(experience.projectManagement || '');
    setResponsibilities([
      experience.responsibilities?.[0] || '',
      experience.responsibilities?.[1] || '',
      experience.responsibilities?.[2] || '',
    ]);
    setStartDate(experience.startDate);
    setEndDate(experience.endDate || '');
    setIsCurrentProject(!experience.endDate);
    setEditingProject(experience);
  };

  const handleDelete = (experienceId: string, projectName: string) => {
    setExperienceToDelete({ id: experienceId, name: projectName });
    setIsDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (!experienceToDelete) return;

    try {
      await deleteExperience(experienceToDelete.id);
      trackChange();
      toast.success('Proyecto eliminado exitosamente');
    } catch (_err) {
      toast.error('Error al eliminar el proyecto');
    } finally {
      setIsDeleteDialogOpen(false);
      setExperienceToDelete(null);
    }
  };

  const resetForm = () => {
    setProjectName('');
    setDescription('');
    setSelectedTools([]);
    setSelectedThirdParties([]);
    setSelectedFrameworks([]);
    setVersionControl('');
    setProjectManagement('');
    setResponsibilities(['', '', '']);
    setStartDate('');
    setEndDate('');
    setIsCurrentProject(false);
    setEditingProject(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!projectName || !description || !startDate) {
      toast.error('Por favor complete los campos obligatorios');
      return;
    }

    const validResponsibilities = responsibilities.filter((r) => r.trim()).slice(0, 3);

    const projectData = {
      projectName,
      description,
      tools: selectedTools,
      thirdParties: selectedThirdParties,
      frameworks: selectedFrameworks,
      versionControl,
      projectManagement,
      responsibilities: validResponsibilities,
      startDate,
      endDate: isCurrentProject ? undefined : endDate || undefined,
    };

    try {
      if (editingProject) {
        await updateExperience(editingProject.id, projectData);
        toast.success('Proyecto actualizado exitosamente');
      } else {
        await addExperience(projectData);
        toast.success('Proyecto agregado exitosamente');
      }

      trackChange();
      resetForm();
      setIsAddingProject(false);
    } catch (_err) {
      toast.error(editingProject ? 'Error al actualizar el proyecto' : 'Error al agregar el proyecto');
    }
  };

  // JSON Import Functions
  const handleJsonUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (e) => {
      const content = e.target?.result as string;
      setJsonInput(content);
      parseJsonPreview(content);
    };
    reader.readAsText(file);
  };

  const parseJsonPreview = (jsonString: string) => {
    try {
      const data = JSON.parse(jsonString);
      const items = Array.isArray(data) ? data : [data];

      const preview: JsonImportData[] = items.map((item, index) => ({
        projectName: item.projectName || `Proyecto ${index + 1}`,
        description: item.description || '',
        tools: Array.isArray(item.tools) ? item.tools : [],
        thirdParties: Array.isArray(item.thirdParties) ? item.thirdParties : [],
        frameworks: Array.isArray(item.frameworks) ? item.frameworks : [],
        versionControl: item.versionControl || '',
        projectManagement: item.projectManagement || '',
        responsibilities: Array.isArray(item.responsibilities) ? item.responsibilities.slice(0, 3) : [],
        startDate: item.startDate || '',
        endDate: item.endDate || undefined,
        selected: true,
      }));

      setImportPreview(preview);
      toast.success(`${preview.length} proyectos encontrados en el JSON`);
    } catch (_error) {
      toast.error('Error al procesar el JSON. Verifique el formato.');
      setImportPreview([]);
    }
  };

  const handleJsonTextChange = (value: string) => {
    setJsonInput(value);
    if (value.trim()) {
      parseJsonPreview(value);
    } else {
      setImportPreview([]);
    }
  };

  const toggleImportSelection = (index: number) => {
    setImportPreview((prev) => prev.map((item, i) => (i === index ? { ...item, selected: !item.selected } : item)));
  };

  const handleImportConfirm = async () => {
    const selectedItems = importPreview.filter((item) => item.selected);
    if (selectedItems.length === 0) {
      toast.error('Seleccione al menos un proyecto para importar');
      return;
    }

    setIsImporting(true);
    try {
      for (const item of selectedItems) {
        const projectData = {
          projectName: item.projectName,
          description: item.description,
          tools: item.tools,
          thirdParties: item.thirdParties,
          frameworks: item.frameworks,
          versionControl: item.versionControl,
          projectManagement: item.projectManagement,
          responsibilities: item.responsibilities,
          startDate: item.startDate,
          endDate: item.endDate,
        };
        await addExperience(projectData);
      }
      trackChange();
      toast.success(`${selectedItems.length} proyectos importados correctamente`);
      setJsonImportOpen(false);
      setJsonInput('');
      setImportPreview([]);
    } catch (_error) {
      toast.error('Error al importar los proyectos');
    } finally {
      setIsImporting(false);
    }
  };

  const generateSampleJson = () => {
    const sample = [
      {
        projectName: 'Sistema de Gestión Empresarial',
        description: 'Desarrollo de sistema integral para gestión de recursos humanos y contabilidad',
        tools: ['Visual Studio Code', 'Postman', 'Docker'],
        thirdParties: ['Stripe', 'SendGrid', 'Google Analytics'],
        frameworks: ['React', 'Express.js', 'Bootstrap'],
        versionControl: 'Git',
        projectManagement: 'Scrum',
        responsibilities: [
          'Desarrollo de componentes frontend',
          'Integración con APIs externas',
          'Optimización de rendimiento',
        ],
        startDate: '2023-01-15',
        endDate: '2023-06-30',
      },
    ];
    return JSON.stringify(sample, null, 2);
  };

  const downloadSampleJson = () => {
    const sample = generateSampleJson();
    const blob = new Blob([sample], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'sample-projects.json';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  };

  const handleToolToggle = (tool: string) => {
    setSelectedTools((prev) => (prev.includes(tool) ? prev.filter((t) => t !== tool) : [...prev, tool]));
  };

  const handleThirdPartyToggle = (party: string) => {
    setSelectedThirdParties((prev) => (prev.includes(party) ? prev.filter((t) => t !== party) : [...prev, party]));
  };

  const handleFrameworkToggle = (framework: string) => {
    setSelectedFrameworks((prev) =>
      prev.includes(framework) ? prev.filter((f) => f !== framework) : [...prev, framework],
    );
  };

  const handleResponsibilityChange = (index: number, value: string) => {
    const newResponsibilities = [...responsibilities];
    newResponsibilities[index] = value;
    setResponsibilities(newResponsibilities);
  };

  const formatDate = (dateString: string) => {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'long',
    });
  };

  const progress = calculateProgress();

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100 p-4 md:p-6">
      <div className="mx-auto max-w-4xl space-y-6">
        <div className="space-y-2 text-center">
          <h1 className="text-3xl font-bold text-slate-900">Experiencia Laboral</h1>
          <p className="text-slate-600">Registre sus proyectos y experiencia profesional</p>
          <div className="mx-auto max-w-md">
            <div className="mb-2 flex justify-between text-sm text-slate-600">
              <span>Progreso del formulario</span>
              <span>{Math.round(progress)}%</span>
            </div>
            <Progress value={progress} className="h-2" />
          </div>
        </div>

        {/* Action Buttons */}
        <div className="flex flex-wrap justify-center gap-3">
          <Button
            onClick={() => setIsAddingProject(true)}
            className={`${buttonStyles.secondary} flex items-center gap-2`}
          >
            <Plus className="h-4 w-4" />
            Agregar Proyecto
          </Button>

          <Dialog open={jsonImportOpen} onOpenChange={setJsonImportOpen}>
            <DialogTrigger asChild>
              <Button variant="outline" className={`${buttonStyles.utility} flex items-center gap-2`}>
                <Upload className="h-4 w-4" />
                Importar desde JSON
              </Button>
            </DialogTrigger>
            <DialogContent className="mx-4 max-h-[80vh] max-w-2xl overflow-y-auto sm:mx-auto">
              <DialogHeader>
                <DialogTitle>Importar Proyectos desde JSON</DialogTitle>
                <DialogDescription>
                  Importe múltiples proyectos desde un archivo JSON o pegue el contenido directamente
                </DialogDescription>
              </DialogHeader>

              <div className="space-y-4">
                <div className="flex items-center gap-2">
                  <Button variant="outline" size="sm" onClick={downloadSampleJson}>
                    <Download className="mr-2 h-4 w-4" />
                    Descargar Ejemplo
                  </Button>
                  <Label htmlFor="json-upload" className="cursor-pointer">
                    <Button variant="outline" size="sm" asChild>
                      <span>
                        <FileText className="mr-2 h-4 w-4" />
                        Cargar Archivo
                      </span>
                    </Button>
                  </Label>
                  <Input id="json-upload" type="file" accept=".json" onChange={handleJsonUpload} className="hidden" />
                </div>

                <div>
                  <Label htmlFor="json-input">JSON de Proyectos</Label>
                  <TextArea
                    id="json-input"
                    value={jsonInput}
                    onChange={(e) => handleJsonTextChange(e.target.value)}
                    placeholder="Pegue aquí el JSON con sus proyectos..."
                    className="min-h-[200px] font-mono text-sm"
                  />
                </div>

                {importPreview.length > 0 && (
                  <div className="space-y-3">
                    <div className="flex items-center justify-between">
                      <h4 className="font-medium">Proyectos encontrados ({importPreview.length})</h4>
                      <div className="flex gap-2">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => setImportPreview((prev) => prev.map((item) => ({ ...item, selected: true })))}
                          className={buttonStyles.utility}
                        >
                          Seleccionar Todos
                        </Button>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => setImportPreview((prev) => prev.map((item) => ({ ...item, selected: false })))}
                          className={buttonStyles.utility}
                        >
                          Deseleccionar Todos
                        </Button>
                      </div>
                    </div>

                    <div className="max-h-60 space-y-2 overflow-y-auto">
                      {importPreview.map((project, index) => (
                        <div key={index} className="flex items-start gap-3 rounded-lg border p-3">
                          <Checkbox checked={project.selected} onCheckedChange={() => toggleImportSelection(index)} />
                          <div className="flex-1">
                            <h5 className="font-medium">{project.projectName}</h5>
                            <p className="line-clamp-2 text-sm text-slate-600">{project.description}</p>
                            <div className="mt-2 flex flex-wrap gap-1">
                              {project.tools.slice(0, 3).map((tool, i) => (
                                <Badge key={i} variant="secondary" className="text-xs">
                                  {tool}
                                </Badge>
                              ))}
                              {project.tools.length > 3 && (
                                <Badge variant="secondary" className="text-xs">
                                  +{project.tools.length - 3} más
                                </Badge>
                              )}
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>

                    <div className="flex justify-end gap-2">
                      <Button variant="outline" onClick={() => setJsonImportOpen(false)}>
                        Cancelar
                      </Button>
                      <Button
                        onClick={handleImportConfirm}
                        disabled={isImporting || importPreview.filter((item) => item.selected).length === 0}
                      >
                        {isImporting
                          ? 'Importando...'
                          : `Importar ${importPreview.filter((item) => item.selected).length} Proyecto(s)`}
                      </Button>
                    </div>
                  </div>
                )}
              </div>
            </DialogContent>
          </Dialog>
        </div>

        {/* Projects List */}
        <div className="space-y-4">
          {loading ? (
            <Card className="border-0 bg-white/70 shadow-sm backdrop-blur-sm">
              <CardContent className="py-8 text-center text-slate-500">Cargando experiencias laborales...</CardContent>
            </Card>
          ) : experiences.length > 0 ? (
            experiences.map((experience) => (
              <Card key={experience.id} className="border-0 bg-white/70 shadow-sm backdrop-blur-sm">
                <CardHeader className="pb-4">
                  <div className="flex items-start justify-between">
                    <div className="flex items-center gap-2">
                      <Briefcase className="h-5 w-5 text-slate-600" />
                      <CardTitle className="text-lg">{experience.projectName}</CardTitle>
                    </div>
                    <div className="flex gap-2">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => {
                          populateFormWithExperience(experience);
                          setIsAddingProject(true);
                        }}
                        className={buttonStyles.utility}
                      >
                        <Edit className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleDelete(experience.id, experience.projectName)}
                        className={buttonStyles.danger}
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </div>
                  <div className="flex items-center gap-2 text-sm text-slate-600">
                    <Calendar className="h-4 w-4" />
                    <span>
                      {formatDate(experience.startDate)} -{' '}
                      {experience.endDate ? formatDate(experience.endDate) : 'Presente'}
                    </span>
                  </div>
                </CardHeader>
                <CardContent className="space-y-4">
                  <p className="text-slate-700">{experience.description}</p>

                  <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                    <div>
                      <h4 className="mb-2 font-medium">Herramientas</h4>
                      <div className="flex flex-wrap gap-1">
                        {experience.tools.map((tool, index) => (
                          <Badge key={index} variant="outline">
                            {tool}
                          </Badge>
                        ))}
                      </div>
                    </div>

                    <div>
                      <h4 className="mb-2 font-medium">Frameworks</h4>
                      <div className="flex flex-wrap gap-1">
                        {experience.frameworks.map((framework, index) => (
                          <Badge key={index} variant="outline">
                            {framework}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  </div>

                  {experience.thirdParties.length > 0 && (
                    <div>
                      <h4 className="mb-2 font-medium">Terceros</h4>
                      <div className="flex flex-wrap gap-1">
                        {experience.thirdParties.map((party, index) => (
                          <Badge key={index} variant="outline">
                            {party}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  )}

                  <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                    <div>
                      <span className="font-medium">Control de Versiones: </span>
                      <span className="text-slate-600">{experience.versionControl}</span>
                    </div>
                    <div>
                      <span className="font-medium">Gestión de Proyecto: </span>
                      <span className="text-slate-600">{experience.projectManagement}</span>
                    </div>
                  </div>

                  {experience.responsibilities.length > 0 && (
                    <div>
                      <h4 className="mb-2 font-medium">Responsabilidades Principales</h4>
                      <ul className="list-inside list-disc space-y-1 text-slate-700">
                        {experience.responsibilities.map((resp, index) => (
                          <li key={index}>{resp}</li>
                        ))}
                      </ul>
                    </div>
                  )}
                </CardContent>
              </Card>
            ))
          ) : (
            <Card className="border-0 bg-white/70 shadow-sm backdrop-blur-sm">
              <CardContent className="py-8 text-center">
                <Briefcase className="mx-auto mb-4 h-12 w-12 text-slate-400" />
                <h3 className="mb-2 text-lg font-medium text-slate-900">No hay experiencias registradas</h3>
                <p className="mb-4 text-slate-600">
                  Comience agregando su primer proyecto o importe desde un archivo JSON
                </p>
              </CardContent>
            </Card>
          )}
        </div>

        {/* Add Project Modal */}
        <Dialog open={isAddingProject} onOpenChange={setIsAddingProject}>
          <DialogContent className="mx-4 max-h-[90vh] max-w-4xl overflow-y-auto sm:mx-auto">
            <DialogHeader>
              <DialogTitle>
                {editingProject ? 'Editar Experiencia Laboral' : 'Agregar Nueva Experiencia Laboral'}
              </DialogTitle>
              <DialogDescription>
                {editingProject
                  ? 'Modifique la información del proyecto'
                  : 'Complete la información del proyecto en el que trabajó'}
              </DialogDescription>
            </DialogHeader>

            <form onSubmit={handleSubmit} className="space-y-6">
              {/* Basic Information */}
              <Card>
                <CardHeader>
                  <CardTitle className="text-base">Información Básica</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="space-y-2">
                    <Label htmlFor="projectName">Nombre del Proyecto *</Label>
                    <Input
                      id="projectName"
                      value={projectName}
                      onChange={(e) => setProjectName(e.target.value)}
                      placeholder="Ej: Sistema de Gestión Empresarial"
                      required
                    />
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="description">Descripción *</Label>
                    <TextArea
                      id="description"
                      value={description}
                      onChange={(e) => setDescription(e.target.value)}
                      placeholder="Descripción detallada del proyecto, objetivos y alcance..."
                      rows={3}
                      required
                    />
                  </div>
                </CardContent>
              </Card>

              {/* Technical Details */}
              <Card>
                <CardHeader>
                  <CardTitle className="text-base">Detalles Técnicos</CardTitle>
                </CardHeader>
                <CardContent className="space-y-6">
                  {/* Tools */}
                  <div className="space-y-2">
                    <Label>Herramientas Utilizadas</Label>
                    <div className="mt-2 grid grid-cols-1 gap-3 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
                      {commonTools.map((tool) => (
                        <div key={tool} className="flex items-center space-x-2">
                          <Checkbox
                            id={`tool-${tool}`}
                            checked={selectedTools.includes(tool)}
                            onCheckedChange={() => handleToolToggle(tool)}
                          />
                          <Label htmlFor={`tool-${tool}`} className="text-sm leading-none">
                            {tool}
                          </Label>
                        </div>
                      ))}
                    </div>
                    {selectedTools.length > 0 && (
                      <div className="mt-2 flex flex-wrap gap-1">
                        {selectedTools.map((tool) => (
                          <Badge key={tool} variant="outline" className="flex items-center gap-1">
                            {tool}
                            <X className="h-3 w-3 cursor-pointer" onClick={() => handleToolToggle(tool)} />
                          </Badge>
                        ))}
                      </div>
                    )}
                  </div>

                  {/* Frameworks */}
                  <div className="space-y-2">
                    <Label>Frameworks</Label>
                    <div className="mt-2 grid grid-cols-1 gap-3 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
                      {commonFrameworks.map((framework) => (
                        <div key={framework} className="flex items-center space-x-2">
                          <Checkbox
                            id={`framework-${framework}`}
                            checked={selectedFrameworks.includes(framework)}
                            onCheckedChange={() => handleFrameworkToggle(framework)}
                          />
                          <Label htmlFor={`framework-${framework}`} className="text-sm">
                            {framework}
                          </Label>
                        </div>
                      ))}
                    </div>
                    {selectedFrameworks.length > 0 && (
                      <div className="mt-2 flex flex-wrap gap-1">
                        {selectedFrameworks.map((framework) => (
                          <Badge key={framework} variant="outline" className="flex items-center gap-1">
                            {framework}
                            <X className="h-3 w-3 cursor-pointer" onClick={() => handleFrameworkToggle(framework)} />
                          </Badge>
                        ))}
                      </div>
                    )}
                  </div>

                  {/* Third Parties */}
                  <div className="space-y-2">
                    <Label>Servicios de Terceros</Label>
                    <div className="mt-2 grid grid-cols-1 gap-3 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
                      {commonThirdParties.map((party) => (
                        <div key={party} className="flex items-center space-x-2">
                          <Checkbox
                            id={`thirdParty-${party}`}
                            checked={selectedThirdParties.includes(party)}
                            onCheckedChange={() => handleThirdPartyToggle(party)}
                          />
                          <Label htmlFor={`thirdParty-${party}`} className="text-sm">
                            {party}
                          </Label>
                        </div>
                      ))}
                    </div>
                    {selectedThirdParties.length > 0 && (
                      <div className="mt-2 flex flex-wrap gap-1">
                        {selectedThirdParties.map((party) => (
                          <Badge key={party} variant="outline" className="flex items-center gap-1">
                            {party}
                            <X className="h-3 w-3 cursor-pointer" onClick={() => handleThirdPartyToggle(party)} />
                          </Badge>
                        ))}
                      </div>
                    )}
                  </div>

                  {/* Version Control & Project Management */}
                  <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
                    <div className="space-y-2">
                      <Label>Control de Versiones</Label>
                      <Select
                        options={versionControlOptions}
                        value={versionControlOptions.find((opt) => opt.value === versionControl)}
                        onChange={(selected) => setVersionControl((selected as any)?.value || '')}
                        placeholder="Seleccionar..."
                        isSearchable={true}
                      />
                    </div>
                    <div className="space-y-2">
                      <Label>Gestión de Proyecto</Label>
                      <Select
                        options={projectManagementOptions}
                        value={projectManagementOptions.find((opt) => opt.value === projectManagement)}
                        onChange={(selected) => setProjectManagement((selected as any)?.value || '')}
                        placeholder="Seleccionar..."
                        isSearchable={true}
                      />
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Responsibilities */}
              <Card>
                <CardHeader>
                  <CardTitle className="text-base">Responsabilidades Principales</CardTitle>
                  <CardDescription>Máximo 3 responsabilidades clave en el proyecto</CardDescription>
                </CardHeader>
                <CardContent className="space-y-3">
                  {responsibilities.map((resp, index) => (
                    <div key={index} className="space-y-2">
                      <Label>Responsabilidad #{index + 1}</Label>
                      <Input
                        value={resp}
                        onChange={(e) => handleResponsibilityChange(index, e.target.value)}
                        placeholder={`Describe tu responsabilidad #${index + 1}`}
                      />
                    </div>
                  ))}
                </CardContent>
              </Card>

              {/* Project Duration */}
              <Card>
                <CardHeader>
                  <CardTitle className="text-base">Duración del Proyecto</CardTitle>
                </CardHeader>
                <CardContent className="grid grid-cols-1 gap-6 md:grid-cols-2">
                  <div className="space-y-2">
                    <Label htmlFor="startDate">Fecha de Inicio *</Label>
                    <Input
                      id="startDate"
                      type="date"
                      value={startDate}
                      onChange={(e) => setStartDate(e.target.value)}
                      required
                    />
                  </div>
                  <div className="space-y-2">
                    <div className="flex items-center gap-2">
                      <Checkbox
                        id="currentProject"
                        checked={isCurrentProject}
                        onCheckedChange={() => setIsCurrentProject(!isCurrentProject)}
                      />
                      <Label htmlFor="currentProject">Actualmente trabajo en este proyecto</Label>
                    </div>

                    {!isCurrentProject && (
                      <div className="space-y-2">
                        <Label htmlFor="endDate">Fecha de Finalización</Label>
                        <Input id="endDate" type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} />
                      </div>
                    )}
                  </div>
                </CardContent>
              </Card>

              <div className="flex justify-end gap-3">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => {
                    resetForm();
                    setIsAddingProject(false);
                  }}
                >
                  Cancelar
                </Button>
                <Button type="submit">{editingProject ? 'Actualizar Proyecto' : 'Guardar Proyecto'}</Button>
              </div>
            </form>
          </DialogContent>
        </Dialog>

        {/* Navegación final */}
        <div className="mt-6 flex justify-between">
          <Button variant="outline" onClick={() => router.push('/profile')} className={buttonStyles.outline}>
            Volver
          </Button>
          <div className="flex gap-2">
            <Button
              variant="outline"
              onClick={() => router.push('/profile/technical')}
              className={buttonStyles.outline}
            >
              Anterior
            </Button>
            <Button type="button" onClick={handleContinue} className={buttonStyles.navigation}>
              {hasChanges ? 'Guardar y Continuar' : 'Siguiente'}
            </Button>
          </div>
        </div>
      </div>

      {/* Confirmation Dialog for Incomplete Fields */}
      <AlertDialog open={isConfirmDialogOpen} onOpenChange={setIsConfirmDialogOpen}>
        <AlertDialogContent className="fixed top-1/2 left-1/2 z-[100] w-full max-w-md -translate-x-1/2 -translate-y-1/2 rounded-lg border bg-white p-6 shadow-lg">
          <AlertDialogHeader>
            <AlertDialogTitle className="text-lg font-semibold text-gray-900">
              ¿Continuar sin completar?
            </AlertDialogTitle>
            <AlertDialogDescription className="mt-2 text-sm text-gray-600">
              Te falta completar: {incompleteFieldsList.join(', ')}. ¿Igual quieres continuar?
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter className="mt-6 flex justify-end gap-3">
            <AlertDialogCancel className={buttonStyles.outline}>Volver a completar</AlertDialogCancel>
            <AlertDialogAction onClick={confirmContinue} className={buttonStyles.primary}>
              Continuar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Delete Project Dialog */}
      <AlertDialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <AlertDialogContent className="fixed top-1/2 left-1/2 z-[100] w-full max-w-md -translate-x-1/2 -translate-y-1/2 rounded-lg border bg-white p-6 shadow-lg">
          <AlertDialogHeader>
            <AlertDialogTitle className="text-lg font-semibold text-gray-900">¿Eliminar proyecto?</AlertDialogTitle>
            <AlertDialogDescription className="mt-2 text-sm text-gray-600">
              ¿Estás seguro de que quieres eliminar el proyecto &quot;{experienceToDelete?.name}&quot;? Esta acción no
              se puede deshacer.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter className="mt-6 flex justify-end gap-3">
            <AlertDialogCancel className={buttonStyles.outline}>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} className={buttonStyles.danger}>
              Eliminar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Overlay for AlertDialogs */}
      {(isDeleteDialogOpen || isConfirmDialogOpen) && <div className="fixed inset-0 z-[99] bg-black/50" />}
    </div>
  );
}
