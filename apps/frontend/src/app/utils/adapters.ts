import { AITeamResponse } from '@/app/utils';
import { GeneratedTeamResponse } from '@/hooks/useTeamGenerator';

export const adaptGeneratedTeamToAITeamResponse = (response: GeneratedTeamResponse): AITeamResponse => {
  return {
    teams: response.teams.map((team) => ({
      team_id: team.team_Id,
      members: team.members.map((member) => ({
        id: member.id,
        name: member.name,
        role: member.role,
        sfia_level: member.sfia_Level,
      })),
    })),
    recommended_leader: {
      id: response.recommended_Leader.id,
      name: response.recommended_Leader.name,
      rationale: response.recommended_Leader.rationale,
    },
    team_analysis: {
      strengths: response.team_Analysis.strengths,
      weaknesses: response.team_Analysis.weaknesses,
      compatibility: response.team_Analysis.compatibility,
    },
    compatibility_score: response.compatibility_Score,
    recommended_Members: response.recommended_members.map((member) => ({
      id: member.id,
      name: member.name,
      compatibility_score: member.compatibility_Score,
      analysis: member.analysis,
      potential_conflicts: member.potential_Conflicts,
      team_impact: member.team_Impact,
    })),
  };
};
