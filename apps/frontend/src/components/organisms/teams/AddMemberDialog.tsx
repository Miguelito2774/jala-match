'use client';

import { useState } from 'react';

import { Select } from '@/components/atoms/inputs/Select';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { useRoles } from '@/hooks/useRoles';
import { useTechnologies } from '@/hooks/useTechnologies';

import { Loader2, UserPlus } from 'lucide-react';
import { toast } from 'sonner';

interface SelectOption {
  value: string | number;
  label: string;
}

interface TeamMemberRecommendation {
  employee_id: string;
  name: string;
  role: string;
  area: string;
  technologies: string[];
  sfia_level: number;
  compatibility_score: number;
  analysis: string;
}

interface MemberToAdd {
  employeeProfileId: string;
  name: string;
  role: string;
  sfiaLevel: number;
  isLeader: boolean;
}

interface AddTeamMemberProps {
  teamId: string;
  onMembersAdded: () => void;
}

export const AddTeamMemberComponent = ({ teamId, onMembersAdded }: AddTeamMemberProps) => {
  const { roles, loading: loadingRoles } = useRoles();
  const { technologies, loading: loadingTech } = useTechnologies();

  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [searchingMembers, setSearchingMembers] = useState(false);
  const [addingMembers, setAddingMembers] = useState(false);

  const [selectedRole, setSelectedRole] = useState<string>('');
  const [selectedArea, setSelectedArea] = useState<string>('');
  const [selectedLevel, setSelectedLevel] = useState<string>('');
  const [selectedTechnologies, setSelectedTechnologies] = useState<string[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<string>('');

  const [recommendations, setRecommendations] = useState<TeamMemberRecommendation[]>([]);
  const [selectedMembers, setSelectedMembers] = useState<string[]>([]);

  const roleOptions = roles.map((r) => ({ value: r.role, label: r.role }));
  const selectedRoleData = roles.find((r) => r.role === selectedRole);
  const areaOptions = selectedRoleData?.areas?.map((a) => ({ value: a, label: a })) || [];
  const levelOptions = selectedRoleData?.levels?.map((l) => ({ value: l, label: l })) || [];

  const categoryOptions = Array.from(new Set(technologies.map((tech) => tech.categoryName))).map((category) => ({
    value: category,
    label: category,
  }));

  const techOptions = !selectedCategory
    ? technologies.map((tech) => ({ value: tech.name, label: tech.name }))
    : technologies
        .filter((tech) => tech.categoryName === selectedCategory)
        .map((tech) => ({ value: tech.name, label: tech.name }));

  const handleRoleChange = (selected: SelectOption | SelectOption[] | null) => {
    if (selected && !Array.isArray(selected)) {
      setSelectedRole(selected.value.toString());
      setSelectedArea('');
      setSelectedLevel('');
    }
  };

  const handleAreaChange = (selected: SelectOption | SelectOption[] | null) => {
    if (selected && !Array.isArray(selected)) {
      setSelectedArea(selected.value.toString());
    }
  };

  const handleLevelChange = (selected: SelectOption | SelectOption[] | null) => {
    if (selected && !Array.isArray(selected)) {
      setSelectedLevel(selected.value.toString());
    }
  };

  const handleCategoryChange = (selected: SelectOption | SelectOption[] | null) => {
    if (selected && !Array.isArray(selected)) {
      setSelectedCategory(selected.value.toString());
      setSelectedTechnologies([]);
    } else {
      setSelectedCategory('');
      setSelectedTechnologies([]);
    }
  };

  const handleTechChange = (selected: SelectOption | SelectOption[] | null) => {
    const selectedOptions = Array.isArray(selected) ? selected : selected ? [selected] : [];
    setSelectedTechnologies(selectedOptions.map((option) => option.value.toString()));
  };

  const handleMemberSelection = (memberId: string) => {
    setSelectedMembers((prev) =>
      prev.includes(memberId) ? prev.filter((id) => id !== memberId) : [...prev, memberId],
    );
  };

  const handleSelectAll = () => {
    if (selectedMembers.length === recommendations.length) {
      setSelectedMembers([]);
    } else {
      setSelectedMembers(recommendations.map((member) => member.employee_id));
    }
  };

  const resetForm = () => {
    setSelectedRole('');
    setSelectedArea('');
    setSelectedLevel('');
    setSelectedTechnologies([]);
    setSelectedCategory('');
    setRecommendations([]);
    setSelectedMembers([]);
  };

  const handleDialogClose = () => {
    resetForm();
    setIsDialogOpen(false);
  };

  const handleFindMembers = async () => {
    if (!selectedRole || !selectedArea || !selectedLevel) {
      toast.error('Por favor, complete todos los campos requeridos');
      return;
    }

    setSearchingMembers(true);

    try {
      const response = await fetch('http://localhost:5001/api/teams/find', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          TeamId: teamId,
          Role: selectedRole,
          Area: selectedArea,
          Level: selectedLevel,
          Technologies: selectedTechnologies,
        }),
      });

      if (!response.ok) {
        throw new Error(`Error: ${response.status}`);
      }

      const data = await response.json();
      setRecommendations(data);
      setSelectedMembers([]);
    } catch (_error) {
      toast.error('Error al buscar miembros para el equipo');
    } finally {
      setSearchingMembers(false);
    }
  };

  const handleAddMembers = async () => {
    if (selectedMembers.length === 0) {
      toast.error('Seleccione al menos un miembro para añadir al equipo');
      return;
    }

    setAddingMembers(true);

    try {
      const membersToAdd: MemberToAdd[] = selectedMembers.map((memberId) => {
        const member = recommendations.find((rec) => rec.employee_id === memberId);
        return {
          employeeProfileId: member!.employee_id,
          name: member!.name,
          role: member!.role,
          sfiaLevel: member!.sfia_level,
          isLeader: false,
        };
      });

      const response = await fetch('http://localhost:5001/api/teams/add', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          teamId: teamId,
          members: membersToAdd,
        }),
      });

      if (!response.ok) {
        throw new Error(`Error: ${response.status}`);
      }

      toast.success('Miembros agregados correctamente');
      handleDialogClose();
      onMembersAdded();
    } catch (_error) {
      toast.error('Error al añadir miembros al equipo');
    } finally {
      setAddingMembers(false);
    }
  };

  return (
    <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
      <DialogTrigger asChild>
        <Button variant="outline" className="flex items-center gap-2">
          <UserPlus className="h-4 w-4" />
          <span>Nuevos miembros</span>
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-2xl">
        <DialogHeader>
          <DialogTitle>Añadir nuevos miembros al equipo</DialogTitle>
        </DialogHeader>

        <div className="mt-4 space-y-6">
          {/* Search Form */}
          <div className="space-y-4">
            <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
              <div>
                <label className="mb-1 block text-sm font-medium text-gray-700">Rol</label>
                <Select
                  options={roleOptions}
                  value={roleOptions.find((opt) => opt.value === selectedRole)}
                  onChange={handleRoleChange}
                  placeholder="Seleccionar rol..."
                  isDisabled={loadingRoles}
                />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium text-gray-700">Área</label>
                <Select
                  options={areaOptions}
                  value={areaOptions.find((opt) => opt.value === selectedArea)}
                  onChange={handleAreaChange}
                  placeholder="Seleccionar área..."
                  isDisabled={!selectedRole || loadingRoles}
                />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium text-gray-700">Nivel</label>
                <Select
                  options={levelOptions}
                  value={levelOptions.find((opt) => opt.value === selectedLevel)}
                  onChange={handleLevelChange}
                  placeholder="Seleccionar nivel..."
                  isDisabled={!selectedRole || loadingRoles}
                />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium text-gray-700">Categoría</label>
                <Select
                  options={categoryOptions}
                  value={categoryOptions.find((opt) => opt.value === selectedCategory)}
                  onChange={handleCategoryChange}
                  placeholder="Filtrar por categoría..."
                  isDisabled={loadingTech}
                />
              </div>

              <div className="md:col-span-2">
                <label className="mb-1 block text-sm font-medium text-gray-700">Tecnologías</label>
                <Select
                  options={techOptions}
                  isMulti
                  value={techOptions.filter((opt) => selectedTechnologies.includes(opt.value.toString()))}
                  onChange={handleTechChange}
                  placeholder="Seleccionar tecnologías..."
                  isDisabled={loadingTech}
                />
              </div>
            </div>

            <div className="flex justify-end">
              <Button
                className="bg-blue-600 text-white hover:bg-blue-700"
                onClick={handleFindMembers}
                disabled={!selectedRole || !selectedArea || !selectedLevel || searchingMembers}
              >
                {searchingMembers ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Buscando...
                  </>
                ) : (
                  'Buscar'
                )}
              </Button>
            </div>
          </div>

          {/* Results */}
          {recommendations.length > 0 && (
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <h3 className="text-sm font-medium text-gray-700">Resultados ({recommendations.length})</h3>
                <Button variant="outline" size="sm" onClick={handleSelectAll}>
                  {selectedMembers.length === recommendations.length ? 'Deseleccionar todos' : 'Seleccionar todos'}
                </Button>
              </div>

              <div className="max-h-80 space-y-3 overflow-y-auto rounded-md border border-gray-200 p-3">
                {recommendations.map((member) => (
                  <div
                    key={member.employee_id}
                    className={`relative cursor-pointer rounded-lg p-4 transition-colors ${
                      selectedMembers.includes(member.employee_id)
                        ? 'border-2 border-blue-500 bg-blue-50'
                        : 'border border-gray-200 bg-white hover:bg-gray-50'
                    }`}
                    onClick={() => handleMemberSelection(member.employee_id)}
                  >
                    <div className="absolute top-4 right-4">
                      <Checkbox
                        checked={selectedMembers.includes(member.employee_id)}
                        onCheckedChange={() => handleMemberSelection(member.employee_id)}
                      />
                    </div>

                    <div className="pr-8">
                      <div className="mb-2 flex flex-col justify-between gap-2 sm:flex-row sm:items-center">
                        <h4 className="font-medium">{member.name}</h4>
                        <div className="flex items-center gap-2">
                          <Badge variant="outline">{member.role}</Badge>
                          <Badge variant="secondary">SFIA {member.sfia_level}</Badge>
                          <Badge
                            variant={
                              member.compatibility_score >= 90
                                ? 'success'
                                : member.compatibility_score >= 80
                                  ? 'default'
                                  : 'outline'
                            }
                          >
                            {member.compatibility_score}%
                          </Badge>
                        </div>
                      </div>

                      <div className="mb-2">
                        <p className="text-sm text-gray-600">{member.area}</p>
                      </div>

                      <div className="mb-3 flex flex-wrap gap-1">
                        {member.technologies.map((tech, index) => (
                          <Badge key={index} variant="outline" className="bg-blue-50">
                            {tech}
                          </Badge>
                        ))}
                      </div>

                      <p className="text-sm text-gray-700">{member.analysis}</p>
                    </div>
                  </div>
                ))}
              </div>

              <div className="flex justify-end gap-3">
                <Button variant="outline" onClick={handleDialogClose}>
                  Cancelar
                </Button>
                <Button onClick={handleAddMembers} disabled={selectedMembers.length === 0 || addingMembers}>
                  {addingMembers ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Agregando...
                    </>
                  ) : (
                    `Agregar ${selectedMembers.length} miembro${selectedMembers.length !== 1 ? 's' : ''}`
                  )}
                </Button>
              </div>
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
};
