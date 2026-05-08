import { useCallback, useState } from 'react';

import { useAuth } from '@/contexts/AuthContext';

import { Team } from './useTeams';

export interface PsychologistReview {
  reviewId: string;
  teamId: string;
  psychologistId: string;
  psychologistEmail: string;
  comment: string;
  status: 'Pending' | 'Approved' | 'FlaggedConcerns';
  reviewedAt: string | null;
  createdAt: string;
}

export interface TeamWithReviewStatus {
  team: Team;
  psychologistReviewRequested: boolean;
  latestReview: PsychologistReview | null;
}

export interface SubmitReviewRequest {
  comment: string;
  status: 'Approved' | 'FlaggedConcerns';
}

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5001/api';

export function usePsychologistReview() {
  const { token } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const getHeaders = useCallback(() => {
    return {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    };
  }, [token]);

  const requestReview = async (teamId: string): Promise<boolean> => {
    setLoading(true);
    setError(null);
    try {
      console.log(
        `[requestReview] Sending POST to ${API_URL}/teams/${teamId}/request-review with headers:`,
        getHeaders(),
      );
      const response = await fetch(`${API_URL}/teams/${teamId}/request-review`, {
        method: 'POST',
        headers: getHeaders(),
      });

      console.log(`[requestReview] Response status: ${response.status}`);

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        console.error('[requestReview] Error response:', errorData);
        throw new Error(errorData.detail || errorData.title || `HTTP ${response.status}: Failed to request review`);
      }

      console.log('[requestReview] Success');
      return true;
    } catch (err: any) {
      console.error('[requestReview] Catch error:', err);
      setError(err.message);
      return false;
    } finally {
      setLoading(false);
    }
  };

  const getPendingTeams = async (): Promise<TeamWithReviewStatus[]> => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch(`${API_URL}/teams/pending-review`, {
        headers: getHeaders(),
      });

      if (!response.ok) {
        throw new Error('Failed to fetch pending teams');
      }

      return await response.json();
    } catch (err: any) {
      setError(err.message);
      return [];
    } finally {
      setLoading(false);
    }
  };

  const getTeamReviews = async (teamId: string): Promise<PsychologistReview[]> => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch(`${API_URL}/teams/${teamId}/reviews`, {
        headers: getHeaders(),
      });

      if (!response.ok) {
        throw new Error('Failed to fetch reviews');
      }

      return await response.json();
    } catch (err: any) {
      setError(err.message);
      return [];
    } finally {
      setLoading(false);
    }
  };

  const submitReview = async (teamId: string, request: SubmitReviewRequest): Promise<boolean> => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch(`${API_URL}/teams/${teamId}/reviews`, {
        method: 'POST',
        headers: getHeaders(),
        body: JSON.stringify(request),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.title || 'Failed to submit review');
      }

      return true;
    } catch (err: any) {
      setError(err.message);
      return false;
    } finally {
      setLoading(false);
    }
  };

  return {
    loading,
    error,
    requestReview,
    getPendingTeams,
    getTeamReviews,
    submitReview,
  };
}
