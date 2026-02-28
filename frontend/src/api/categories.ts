import client from './client';
import type { Category } from '../types';

export const getCategories = (type?: number) =>
  client.get<Category[]>('/categories', { params: type !== undefined ? { type } : {} }).then(r => r.data);

export const createCategory = (data: { name: string; type: number; icon?: string; color?: string }) =>
  client.post<Category>('/categories', data).then(r => r.data);

export const updateCategory = (id: string, data: { name: string; icon?: string; color?: string }) =>
  client.put<Category>(`/categories/${id}`, data).then(r => r.data);

export const deleteCategory = (id: string) =>
  client.delete(`/categories/${id}`);
