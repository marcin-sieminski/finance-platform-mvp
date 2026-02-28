import client from './client';
import type { Account, CreateAccountRequest, UpdateAccountRequest } from '../types';

export const getAccounts = () =>
  client.get<Account[]>('/accounts').then(r => r.data);

export const getAccount = (id: string) =>
  client.get<Account>(`/accounts/${id}`).then(r => r.data);

export const createAccount = (data: CreateAccountRequest) =>
  client.post<Account>('/accounts', data).then(r => r.data);

export const updateAccount = (id: string, data: UpdateAccountRequest) =>
  client.put<Account>(`/accounts/${id}`, data).then(r => r.data);

export const deleteAccount = (id: string) =>
  client.delete(`/accounts/${id}`);
