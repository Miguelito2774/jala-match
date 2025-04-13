'use client';

import { useState } from 'react';

import { Button } from '@/components/atoms/buttons/Button';
import { Progress } from '@/components/ui/progress';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';

import { ArrowLeft, CheckCircle, Crown, Info } from 'lucide-react';

interface TeamMember {
  id: string;
  name: string;
  role: string;
  sfia_level: number;
}

interface TeamData {
  teams: {
    team_Id: string;
    members: TeamMember[];
  }[];
  recommended_Leader: {
    id: string;
    name: string;
    rationale: string;
  };
  team_Analysis: {
    strengths: string[];
    weaknesses: string[];
    compatibility: string;
  };
  compatibility_Score: number;
}

interface TeamResultsPageProps {
  teamData: TeamData;
  onBack: () => void;
}

export const TeamResultsPage = ({ teamData, onBack }: TeamResultsPageProps) => {
  const [teamName, setTeamName] = useState('Equipo Generado');
  const [isEditingName, setIsEditingName] = useState(false);

  const team = teamData.teams[0];
  const leader = teamData.recommended_Leader;
  const analysis = teamData.team_Analysis;
  const score = teamData.compatibility_Score;

  const isLeader = (memberId: string) => memberId === leader.id;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <Button onClick={onBack} variant="outline" size="sm" className="flex items-center gap-1">
          <ArrowLeft className="h-4 w-4" />
          Volver
        </Button>

        <div className="flex items-center gap-2">
          {isEditingName ? (
            <input
              type="text"
              value={teamName}
              onChange={(e) => setTeamName(e.target.value)}
              onBlur={() => setIsEditingName(false)}
              onKeyDown={(e) => e.key === 'Enter' && setIsEditingName(false)}
              className="rounded border border-gray-300 px-2 py-1 text-lg font-semibold"
              autoFocus
            />
          ) : (
            <h2 className="text-lg font-semibold">{teamName}</h2>
          )}
          <button onClick={() => setIsEditingName(!isEditingName)} className="text-gray-500 hover:text-gray-700">
            ✏️
          </button>
        </div>
      </div>

      {/* Score Bar */}
      <div className="rounded-lg bg-white p-4 shadow">
        <div className="mb-2 flex items-center justify-between">
          <h3 className="font-medium">Compatibilidad</h3>
          <span className="font-bold">{score}%</span>
        </div>
        <Progress value={score} className="h-3 bg-gray-200" />
      </div>

      {/* Team Members */}
      <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
        {team.members.map((member) => (
          <div
            key={member.id}
            className={`relative rounded-lg bg-white p-4 shadow transition-shadow hover:shadow-md ${isLeader(member.id) ? 'border-2 border-yellow-400' : ''}`}
          >
            {isLeader(member.id) && (
              <div className="absolute -top-3 -right-3 rounded-full bg-yellow-400 p-1 shadow">
                <TooltipProvider>
                  <Tooltip>
                    <TooltipTrigger>
                      <Crown className="h-6 w-6 text-white" />
                    </TooltipTrigger>
                    <TooltipContent>
                      <p className="max-w-xs text-sm">{leader.rationale}</p>
                    </TooltipContent>
                  </Tooltip>
                </TooltipProvider>
              </div>
            )}

            <div className="flex items-center gap-3">
              <div
                className={`relative flex h-14 w-14 items-center justify-center rounded-full bg-gray-100 text-xl font-bold ${isLeader(member.id) ? 'ring-4 ring-yellow-200' : ''}`}
              >
                {member.name
                  .split(' ')
                  .map((word) => word[0])
                  .join('')}
              </div>

              <div>
                <h3 className="text-lg font-semibold">{member.name}</h3>
                <p className="text-sm text-gray-600">
                  {member.role} · Nivel SFIA {member.sfia_level || 'N/A'}
                </p>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Team Analysis */}
      <div className="overflow-hidden rounded-lg bg-white shadow">
        <div className="border-b border-gray-200 p-4">
          <h3 className="text-lg font-medium">Análisis del Equipo</h3>
        </div>

        <div className="p-4">
          <div className="mb-6">
            <h4 className="mb-2 flex items-center gap-2 font-medium text-green-600">
              <CheckCircle className="h-5 w-5" /> Fortalezas
            </h4>
            <ul className="list-disc space-y-1 pl-6">
              {analysis.strengths.map((strength, index) => (
                <li key={index} className="text-gray-700">
                  {strength}
                </li>
              ))}
            </ul>
          </div>

          <div className="mb-6">
            <h4 className="mb-2 flex items-center gap-2 font-medium text-amber-600">
              <Info className="h-5 w-5" /> Debilidades
            </h4>
            <ul className="list-disc space-y-1 pl-6">
              {analysis.weaknesses.map((weakness, index) => (
                <li key={index} className="text-gray-700">
                  {weakness}
                </li>
              ))}
            </ul>
          </div>

          <div>
            <h4 className="mb-2 font-medium text-blue-600">Justificación</h4>
            <p className="text-gray-700">{analysis.compatibility}</p>
          </div>
        </div>
      </div>

      <div className="flex justify-end">
        <Button className="bg-green-500 hover:bg-green-600">Confirmar Equipo</Button>
      </div>
    </div>
  );
};
