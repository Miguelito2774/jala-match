import { CreateTeamRequest, TeamAnalysis, TeamMember, TeamWeights } from '@/hooks/useTeams';

export interface AITeamMember {
  id: string;
  name: string;
  role: string;
  sfia_level: number;
}

export interface AITeam {
  team_id: string;
  members: AITeamMember[];
}

export interface AIRecommendedLeader {
  id: string;
  name: string;
  rationale: string;
}

export interface AITeamAnalysis {
  strengths: string[];
  weaknesses: string[];
  compatibility: string;
}

export interface AIRecommendedMember {
  id: string;
  name: string;
  compatibility_score: number;
  analysis: string;
  potential_conflicts: string[];
  team_impact: string;
}

export interface AITeamResponse {
  teams: AITeam[];
  recommended_leader: AIRecommendedLeader;
  team_analysis: AITeamAnalysis;
  compatibility_score: number;
  recommended_Members: AIRecommendedMember[];
}

export interface TeamBuilderFormData {
  teamName: string;
  creatorId: string;
  weights: TeamWeights;
  requiredTechnologies: string[];
  isBlended?: boolean;
}

export const buildCreateTeamRequest = (
  aiResponse: AITeamResponse,
  teamBuilderData: TeamBuilderFormData,
  userSelectedLeaderId?: string,
): CreateTeamRequest => {
  const finalLeaderId = userSelectedLeaderId || aiResponse.recommended_leader.id;

  const members: TeamMember[] = aiResponse.teams[0].members.map((member) => ({
    employeeProfileId: member.id,
    name: member.name,
    role: member.role,
    sfiaLevel: member.sfia_level,
    isLeader: member.id === finalLeaderId,
  }));

  const analysis: TeamAnalysis = {
    strengths: aiResponse.team_analysis.strengths,
    weaknesses: aiResponse.team_analysis.weaknesses,
    compatibility: aiResponse.team_analysis.compatibility,
  };

  return {
    name: teamBuilderData.teamName,
    creatorId: teamBuilderData.creatorId,
    members,
    leaderId: finalLeaderId,
    analysis,
    compatibilityScore: aiResponse.compatibility_score,
    weights: teamBuilderData.weights,
    requiredTechnologies: teamBuilderData.requiredTechnologies,
    isBlended: teamBuilderData.isBlended || false,
  };
};
