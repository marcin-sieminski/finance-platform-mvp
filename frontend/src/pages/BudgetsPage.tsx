import { useEffect, useState, useCallback } from 'react';
import { getBudgets, createBudget, updateBudget, deleteBudget } from '../api/budgets';
import { getCategories } from '../api/categories';
import type { Budget, Category } from '../types';
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
import Spinner from '../components/ui/Spinner';
import clsx from 'clsx';

const schema = z.object({
  categoryId: z.string().min(1, 'Select a category'),
  limitAmount: z.number().positive('Must be positive'),
  month: z.number().min(1).max(12),
  year: z.number().min(2000).max(9999),
});

type FormData = z.infer<typeof schema>;

const MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

function fmt(n: number) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(n);
}

function BudgetCard({ budget, onEdit, onDelete }: { budget: Budget; onEdit: () => void; onDelete: () => void }) {
  const pct = Math.min(budget.progressPercent, 100);
  const overBudget = budget.progressPercent >= 90;

  return (
    <Card>
      <div className="flex items-center justify-between mb-3">
        <div className="flex items-center gap-2">
          <div className="w-8 h-8 rounded-full flex items-center justify-center text-base" style={{ backgroundColor: budget.categoryColor ?? '#e5e7eb' }}>
            {budget.categoryIcon ?? '📦'}
          </div>
          <div>
            <p className="font-medium text-gray-900 text-sm">{budget.categoryName}</p>
            <p className="text-xs text-gray-400">Limit: {fmt(budget.limitAmount)}</p>
          </div>
        </div>
        <div className="flex gap-1">
          <Button size="sm" variant="ghost" onClick={onEdit}>✏️</Button>
          <Button size="sm" variant="ghost" onClick={onDelete}>🗑️</Button>
        </div>
      </div>

      <div className="space-y-2">
        <div className="flex justify-between text-xs text-gray-500">
          <span>Spent: {fmt(budget.spentAmount)}</span>
          <span className={overBudget ? 'text-red-600 font-semibold' : ''}>{pct.toFixed(0)}%</span>
        </div>
        <div className="h-2 bg-gray-100 rounded-full overflow-hidden">
          <div className={clsx('h-full rounded-full transition-all', overBudget ? 'bg-red-500' : 'bg-indigo-500')} style={{ width: `${pct}%` }} />
        </div>
        <p className={clsx('text-xs', budget.remainingAmount < 0 ? 'text-red-600' : 'text-gray-400')}>
          {budget.remainingAmount >= 0 ? `${fmt(budget.remainingAmount)} remaining` : `${fmt(Math.abs(budget.remainingAmount))} over budget`}
        </p>
      </div>
    </Card>
  );
}

export default function BudgetsPage() {
  const now = new Date();
  const [month, setMonth] = useState(now.getMonth() + 1);
  const [year, setYear] = useState(now.getFullYear());
  const [budgets, setBudgets] = useState<Budget[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Budget | null>(null);
  const [deleting, setDeleting] = useState<Budget | null>(null);

  const { register, handleSubmit, reset, setValue, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema) as Resolver<FormData>,
    defaultValues: { month, year, limitAmount: 0 },
  });

  const load = useCallback(() => {
    setLoading(true);
    getBudgets(month, year).then(setBudgets).finally(() => setLoading(false));
  }, [month, year]);

  useEffect(() => { load(); }, [load]);
  useEffect(() => { getCategories().then(setCategories); }, []);

  const openCreate = () => {
    setEditing(null);
    reset({ categoryId: '', limitAmount: 0, month, year });
    setModalOpen(true);
  };

  const openEdit = (b: Budget) => {
    setEditing(b);
    reset({ categoryId: b.categoryId, limitAmount: b.limitAmount, month: b.month, year: b.year });
    setModalOpen(true);
  };

  const onSubmit = async (data: FormData) => {
    if (editing) {
      await updateBudget(editing.id, { limitAmount: data.limitAmount });
    } else {
      await createBudget(data);
    }
    setModalOpen(false);
    load();
  };

  const onDelete = async () => {
    if (!deleting) return;
    await deleteBudget(deleting.id);
    setDeleting(null);
    load();
  };

  const categoryOpts = categories.filter(c => c.type === 1).map(c => ({ value: c.id, label: c.name }));
  const monthOpts = MONTHS.map((m, i) => ({ value: i + 1, label: m }));
  const yearOpts = Array.from({ length: 5 }, (_, i) => now.getFullYear() - 2 + i).map(y => ({ value: y, label: String(y) }));

  return (
    <div className="space-y-6 max-w-5xl">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Budgets</h1>
        <Button onClick={openCreate}>+ Add Budget</Button>
      </div>

      <div className="flex gap-3 items-end">
        <div className="w-36">
          <Select label="Month" options={monthOpts} value={month} onChange={e => setMonth(Number(e.target.value))} />
        </div>
        <div className="w-28">
          <Select label="Year" options={yearOpts} value={year} onChange={e => setYear(Number(e.target.value))} />
        </div>
      </div>

      {loading
        ? <div className="flex justify-center items-center h-64"><Spinner /></div>
        : budgets.length === 0
          ? <Card><p className="text-gray-400 text-center py-12">No budgets for {MONTHS[month - 1]} {year}. Set one up!</p></Card>
          : <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">{budgets.map(b => <BudgetCard key={b.id} budget={b} onEdit={() => openEdit(b)} onDelete={() => setDeleting(b)} />)}</div>
      }

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Edit Budget' : 'New Budget'}>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {!editing && <Select label="Category (expense)" options={categoryOpts} error={errors.categoryId?.message} onChange={e => setValue('categoryId', e.target.value)} defaultValue="" />}
          <Input label="Monthly limit ($)" type="number" step="0.01" placeholder="500.00" error={errors.limitAmount?.message} {...register('limitAmount', { valueAsNumber: true })} />
          {!editing && (
            <div className="grid grid-cols-2 gap-3">
              <Select label="Month" options={monthOpts} error={errors.month?.message} onChange={e => setValue('month', Number(e.target.value))} defaultValue={month} />
              <Select label="Year" options={yearOpts} error={errors.year?.message} onChange={e => setValue('year', Number(e.target.value))} defaultValue={year} />
            </div>
          )}
          <div className="flex gap-3 justify-end pt-2">
            <Button type="button" variant="secondary" onClick={() => setModalOpen(false)}>Cancel</Button>
            <Button type="submit" loading={isSubmitting}>{editing ? 'Save' : 'Create'}</Button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog open={!!deleting} title="Delete Budget" message={`Delete the ${deleting?.categoryName} budget?`} onConfirm={onDelete} onCancel={() => setDeleting(null)} />
    </div>
  );
}
