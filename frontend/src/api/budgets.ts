import client from './client';
import type { Budget } from '../types';

export const getBudgets = (month?: number, year?: number) =>
  client.get<Budget[]>('/budgets', { params: { month, year } }).then(r => r.data);

export const createBudget = (data: { categoryId: string; limitAmount: number; month: number; year: number }) =>
  client.post<Budget>('/budgets', data).then(r => r.data);

export const updateBudget = (id: string, data: { limitAmount: number }) =>
  client.put<Budget>(`/budgets/${id}`, data).then(r => r.data);

export const deleteBudget = (id: string) =>
  client.delete(`/budgets/${id}`);
