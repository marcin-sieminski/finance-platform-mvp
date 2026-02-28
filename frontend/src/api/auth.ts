import client from './client';
import type { AuthResponse, UserProfile } from '../types';

export const register = (data: { email: string; password: string; firstName: string; lastName: string }) =>
  client.post<AuthResponse>('/auth/register', data).then(r => r.data);

export const login = (data: { email: string; password: string }) =>
  client.post<AuthResponse>('/auth/login', data).then(r => r.data);

export const getMe = () =>
  client.get<UserProfile>('/auth/me').then(r => r.data);
