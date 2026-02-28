import { useEffect, useState, useCallback } from 'react';
import { getTransactions, createTransaction, updateTransaction, deleteTransaction } from '../api/transactions';
import type { TransactionFilters } from '../api/transactions';
import { getAccounts } from '../api/accounts';
import { getCategories } from '../api/categories';
import type { Transaction, Account, Category, PagedResult } from '../types';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import type { Resolver } from 'react-hook-form';
import Card from '../components/ui/Card';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import Select from '../components/ui/Select';
import Modal from '../components/ui/Modal';
import ConfirmDialog from '../components/ui/ConfirmDialog';
import Pagination from '../components/ui/Pagination';
import Spinner from '../components/ui/Spinner';

const schema = z.object({
  accountId: z.string().min(1, 'Select an account'),
  categoryId: z.string().min(1, 'Select a category'),
  amount: z.number().positive('Must be positive'),
  type: z.number(),
  description: z.string().optional(),
  date: z.string().min(1, 'Required'),
});

type FormData = z.infer<typeof schema>;

function fmt(n: number) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(n);
}

export default function TransactionsPage() {
  const [result, setResult] = useState<PagedResult<Transaction> | null>(null);
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [filters, setFilters] = useState<TransactionFilters>({ page: 1, pageSize: 20 });
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Transaction | null>(null);
  const [deleting, setDeleting] = useState<Transaction | null>(null);

  const { register, handleSubmit, reset, setValue, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema) as Resolver<FormData>,
    defaultValues: { type: 1, date: new Date().toISOString().slice(0, 10), amount: 0 },
  });

  const load = useCallback(() => {
    setLoading(true);
    getTransactions(filters).then(setResult).finally(() => setLoading(false));
  }, [filters]);

  useEffect(() => { load(); }, [load]);
  useEffect(() => {
    getAccounts().then(setAccounts);
    getCategories().then(setCategories);
  }, []);

  const openCreate = () => {
    setEditing(null);
    reset({ type: 1, date: new Date().toISOString().slice(0, 10), accountId: accounts[0]?.id ?? '', categoryId: '', amount: 0 });
    setModalOpen(true);
  };

  const openEdit = (t: Transaction) => {
    setEditing(t);
    reset({ accountId: t.accountId, categoryId: t.categoryId, amount: t.amount, type: t.type, description: t.description ?? '', date: t.date.slice(0, 10) });
    setModalOpen(true);
  };

  const onSubmit = async (data: FormData) => {
    const payload = { ...data, date: new Date(data.date).toISOString() };
    if (editing) {
      await updateTransaction(editing.id, payload);
    } else {
      await createTransaction(payload);
    }
    setModalOpen(false);
    load();
  };

  const onDelete = async () => {
    if (!deleting) return;
    await deleteTransaction(deleting.id);
    setDeleting(null);
    load();
  };

  const accountOpts = accounts.map(a => ({ value: a.id, label: a.name }));
  const categoryOpts = categories.map(c => ({ value: c.id, label: c.name }));

  return (
    <div className="space-y-6 max-w-5xl">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Transactions</h1>
        <Button onClick={openCreate}>+ Add Transaction</Button>
      </div>

      <Card>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
          <Select label="Account" options={[{ value: '', label: 'All accounts' }, ...accountOpts]} value={filters.accountId ?? ''} onChange={e => setFilters(f => ({ ...f, accountId: e.target.value || undefined, page: 1 }))} />
          <Select label="Type" options={[{ value: '', label: 'All types' }, { value: 0, label: 'Income' }, { value: 1, label: 'Expense' }]} value={filters.type ?? ''} onChange={e => setFilters(f => ({ ...f, type: e.target.value !== '' ? Number(e.target.value) : undefined, page: 1 }))} />
          <Input label="From date" type="date" value={filters.from ?? ''} onChange={e => setFilters(f => ({ ...f, from: e.target.value || undefined, page: 1 }))} />
          <Input label="To date" type="date" value={filters.to ?? ''} onChange={e => setFilters(f => ({ ...f, to: e.target.value || undefined, page: 1 }))} />
        </div>
      </Card>

      <Card className="p-0 overflow-hidden">
        {loading
          ? <div className="flex justify-center items-center h-48"><Spinner /></div>
          : !result || result.items.length === 0
            ? <p className="text-gray-400 text-center py-12">No transactions found</p>
            : (
              <div className="overflow-x-auto">
                <table className="w-full text-sm">
                  <thead className="bg-gray-50 border-b border-gray-100">
                    <tr>
                      {['Date', 'Category', 'Account', 'Description', 'Amount', ''].map(h => (
                        <th key={h} className="text-left text-xs font-medium text-gray-500 uppercase px-4 py-3">{h}</th>
                      ))}
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-50">
                    {result.items.map(t => (
                      <tr key={t.id} className="hover:bg-gray-50">
                        <td className="px-4 py-3 text-gray-500">{new Date(t.date).toLocaleDateString()}</td>
                        <td className="px-4 py-3">
                          <div className="flex items-center gap-2">
                            <span>{t.categoryIcon ?? '📦'}</span>
                            <span className="text-gray-700">{t.categoryName}</span>
                          </div>
                        </td>
                        <td className="px-4 py-3 text-gray-500">{t.accountName}</td>
                        <td className="px-4 py-3 text-gray-500 max-w-[160px] truncate">{t.description ?? '-'}</td>
                        <td className={`px-4 py-3 font-semibold ${t.type === 0 ? 'text-green-600' : 'text-red-600'}`}>
                          {t.type === 0 ? '+' : '-'}{fmt(t.amount)}
                        </td>
                        <td className="px-4 py-3">
                          <div className="flex gap-1">
                            <Button size="sm" variant="ghost" onClick={() => openEdit(t)}>✏️</Button>
                            <Button size="sm" variant="ghost" onClick={() => setDeleting(t)}>🗑️</Button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
                <div className="p-4">
                  <Pagination page={result.page} totalPages={result.totalPages} onPageChange={p => setFilters(f => ({ ...f, page: p }))} />
                </div>
              </div>
            )
        }
      </Card>

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Edit Transaction' : 'New Transaction'}>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Select label="Type" options={[{ value: 0, label: '📈 Income' }, { value: 1, label: '📉 Expense' }]} error={errors.type?.message} onChange={e => setValue('type', Number(e.target.value))} defaultValue={1} />
          <Select label="Account" options={accountOpts} error={errors.accountId?.message} onChange={e => setValue('accountId', e.target.value)} defaultValue={accounts[0]?.id ?? ''} />
          <Select label="Category" options={categoryOpts} error={errors.categoryId?.message} onChange={e => setValue('categoryId', e.target.value)} defaultValue="" />
          <Input label="Amount" type="number" step="0.01" placeholder="0.00" error={errors.amount?.message} {...register('amount', { valueAsNumber: true })} />
          <Input label="Date" type="date" error={errors.date?.message} {...register('date')} />
          <Input label="Description (optional)" placeholder="Notes..." error={errors.description?.message} {...register('description')} />
          <div className="flex gap-3 justify-end pt-2">
            <Button type="button" variant="secondary" onClick={() => setModalOpen(false)}>Cancel</Button>
            <Button type="submit" loading={isSubmitting}>{editing ? 'Save' : 'Add'}</Button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        open={!!deleting}
        title="Delete Transaction"
        message={`Delete this ${deleting?.categoryName} transaction of ${deleting ? fmt(deleting.amount) : ''}?`}
        onConfirm={onDelete}
        onCancel={() => setDeleting(null)}
      />
    </div>
  );
}
