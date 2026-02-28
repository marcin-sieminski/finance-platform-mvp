import client from './client';
import type { Transaction, PagedResult } from '../types';

export interface TransactionFilters {
  page?: number;
  pageSize?: number;
  accountId?: string;
  categoryId?: string;
  type?: number;
  from?: string;
  to?: string;
}

export const getTransactions = (filters: TransactionFilters = {}) =>
  client.get<PagedResult<Transaction>>('/transactions', { params: filters }).then(r => r.data);

export const createTransaction = (data: {
  accountId: string;
  categoryId: string;
  amount: number;
  type: number;
  description?: string;
  date: string;
}) => client.post<Transaction>('/transactions', data).then(r => r.data);

export const updateTransaction = (id: string, data: {
  accountId: string;
  categoryId: string;
  amount: number;
  type: number;
  description?: string;
  date: string;
}) => client.put<Transaction>(`/transactions/${id}`, data).then(r => r.data);

export const deleteTransaction = (id: string) =>
  client.delete(`/transactions/${id}`);
