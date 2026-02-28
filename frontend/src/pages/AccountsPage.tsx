import { useEffect, useState } from 'react';
import { getAccounts, createAccount, updateAccount, deleteAccount } from '../api/accounts';
import type { Account } from '../types';
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

const ACCOUNT_TYPES = [
  { value: 0, label: 'Checking' },
  { value: 1, label: 'Savings' },
  { value: 2, label: 'Credit Card' },
  { value: 3, label: 'Cash' },
  { value: 4, label: 'Investment' },
];

const TYPE_LABELS: Record<number, string> = { 0: 'Checking', 1: 'Savings', 2: 'Credit Card', 3: 'Cash', 4: 'Investment' };
const TYPE_ICONS: Record<number, string> = { 0: '🏦', 1: '💰', 2: '💳', 3: '💵', 4: '📈' };

const schema = z.object({
  name: z.string().min(1, 'Required'),
  type: z.number(),
  balance: z.number(),
  currency: z.string().min(1),
  color: z.string().optional(),
});

type FormData = z.infer<typeof schema>;

function AccountCard({ account, onEdit, onDelete }: { account: Account; onEdit: () => void; onDelete: () => void }) {
  const type = account.type as unknown as number;
  const icon = TYPE_ICONS[type] ?? '🏦';
  return (
    <Card className="flex items-center justify-between">
      <div className="flex items-center gap-4">
        <div className="w-12 h-12 rounded-xl flex items-center justify-center text-2xl" style={{ backgroundColor: account.color ?? '#e0e7ff' }}>
          {icon}
        </div>
        <div>
          <p className="font-semibold text-gray-900">{account.name}</p>
          <p className="text-sm text-gray-400">{TYPE_LABELS[type] ?? 'Account'} · {account.currency}</p>
        </div>
      </div>
      <div className="flex items-center gap-4">
        <span className="text-xl font-bold text-gray-900">
          {new Intl.NumberFormat('en-US', { style: 'currency', currency: account.currency }).format(account.balance)}
        </span>
        <div className="flex gap-2">
          <Button size="sm" variant="ghost" onClick={onEdit}>✏️</Button>
          <Button size="sm" variant="ghost" onClick={onDelete}>🗑️</Button>
        </div>
      </div>
    </Card>
  );
}

export default function AccountsPage() {
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Account | null>(null);
  const [deleting, setDeleting] = useState<Account | null>(null);
  const [deleteLoading, setDeleteLoading] = useState(false);

  const { register, handleSubmit, reset, setValue, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema) as Resolver<FormData>,
    defaultValues: { currency: 'USD', type: 0, balance: 0 },
  });

  const load = () => getAccounts().then(setAccounts).finally(() => setLoading(false));
  useEffect(() => { load(); }, []);

  const openCreate = () => {
    setEditing(null);
    reset({ name: '', type: 0, balance: 0, currency: 'USD', color: '' });
    setModalOpen(true);
  };

  const openEdit = (a: Account) => {
    setEditing(a);
    reset({ name: a.name, type: a.type as unknown as number, balance: a.balance, currency: a.currency, color: a.color ?? '' });
    setModalOpen(true);
  };

  const onSubmit = async (data: FormData) => {
    if (editing) {
      await updateAccount(editing.id, { name: data.name, type: data.type, currency: data.currency, color: data.color });
    } else {
      await createAccount(data);
    }
    setModalOpen(false);
    load();
  };

  const onDelete = async () => {
    if (!deleting) return;
    setDeleteLoading(true);
    await deleteAccount(deleting.id);
    setDeleteLoading(false);
    setDeleting(null);
    load();
  };

  if (loading) return <div className="flex justify-center items-center h-64"><Spinner /></div>;

  return (
    <div className="space-y-6 max-w-3xl">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Accounts</h1>
        <Button onClick={openCreate}>+ Add Account</Button>
      </div>

      {accounts.length === 0
        ? <Card><p className="text-gray-400 text-center py-10">No accounts yet. Add your first account!</p></Card>
        : <div className="space-y-3">{accounts.map(a => <AccountCard key={a.id} account={a} onEdit={() => openEdit(a)} onDelete={() => setDeleting(a)} />)}</div>
      }

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Edit Account' : 'New Account'}>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Input label="Account name" placeholder="Chase Checking" error={errors.name?.message} {...register('name')} />
          <Select label="Type" options={ACCOUNT_TYPES} error={errors.type?.message} onChange={e => setValue('type', Number(e.target.value))} defaultValue={0} />
          {!editing && <Input label="Initial balance" type="number" step="0.01" placeholder="0.00" error={errors.balance?.message} {...register('balance', { valueAsNumber: true })} />}
          <Input label="Currency" placeholder="USD" maxLength={3} error={errors.currency?.message} {...register('currency')} />
          <Input label="Color (hex)" placeholder="#6366f1" error={errors.color?.message} {...register('color')} />
          <div className="flex gap-3 justify-end pt-2">
            <Button type="button" variant="secondary" onClick={() => setModalOpen(false)}>Cancel</Button>
            <Button type="submit" loading={isSubmitting}>{editing ? 'Save' : 'Create'}</Button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        open={!!deleting}
        title="Archive Account"
        message={`Archive "${deleting?.name}"? This will hide it from your list.`}
        onConfirm={onDelete}
        onCancel={() => setDeleting(null)}
        loading={deleteLoading}
      />
    </div>
  );
}
