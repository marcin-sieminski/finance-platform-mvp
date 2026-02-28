import { useEffect, useState } from 'react';
import { getCategories, createCategory, updateCategory, deleteCategory } from '../api/categories';
import type { Category } from '../types';
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
  name: z.string().min(1, 'Required'),
  type: z.number(),
  icon: z.string().optional(),
  color: z.string().optional(),
});

type FormData = z.infer<typeof schema>;

export default function CategoriesPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [tab, setTab] = useState<0 | 1>(1);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Category | null>(null);
  const [deleting, setDeleting] = useState<Category | null>(null);

  const { register, handleSubmit, reset, setValue, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema) as Resolver<FormData>,
    defaultValues: { type: 1 },
  });

  const load = () => getCategories().then(setCategories).finally(() => setLoading(false));
  useEffect(() => { load(); }, []);

  const filtered = categories.filter(c => c.type === tab);

  const openCreate = () => { setEditing(null); reset({ name: '', type: tab, icon: '', color: '' }); setModalOpen(true); };
  const openEdit = (c: Category) => { setEditing(c); reset({ name: c.name, type: c.type, icon: c.icon ?? '', color: c.color ?? '' }); setModalOpen(true); };

  const onSubmit = async (data: FormData) => {
    if (editing) {
      await updateCategory(editing.id, { name: data.name, icon: data.icon, color: data.color });
    } else {
      await createCategory(data);
    }
    setModalOpen(false);
    load();
  };

  const onDelete = async () => {
    if (!deleting) return;
    await deleteCategory(deleting.id);
    setDeleting(null);
    load();
  };

  if (loading) return <div className="flex justify-center items-center h-64"><Spinner /></div>;

  return (
    <div className="space-y-6 max-w-2xl">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Categories</h1>
        <Button onClick={openCreate}>+ Add Category</Button>
      </div>

      <div className="flex gap-2 bg-gray-100 p-1 rounded-lg w-fit">
        {[{ label: 'Income', value: 0 as const }, { label: 'Expense', value: 1 as const }].map(t => (
          <button key={t.value} onClick={() => setTab(t.value)} className={clsx('px-4 py-1.5 rounded-md text-sm font-medium transition-colors', tab === t.value ? 'bg-white shadow text-gray-900' : 'text-gray-500 hover:text-gray-700')}>
            {t.label}
          </button>
        ))}
      </div>

      <Card>
        {filtered.length === 0
          ? <p className="text-gray-400 text-center py-10">No {tab === 0 ? 'income' : 'expense'} categories</p>
          : (
            <div className="divide-y divide-gray-50">
              {filtered.map(c => (
                <div key={c.id} className="flex items-center justify-between py-3">
                  <div className="flex items-center gap-3">
                    <div className="w-8 h-8 rounded-full flex items-center justify-center text-base" style={{ backgroundColor: c.color ?? '#e5e7eb' }}>
                      {c.icon ?? '📦'}
                    </div>
                    <span className="text-sm font-medium text-gray-800">{c.name}</span>
                    {c.isDefault && <span className="text-xs bg-gray-100 text-gray-500 px-2 py-0.5 rounded-full">Default</span>}
                  </div>
                  <div className="flex gap-2">
                    {!c.isDefault && <Button size="sm" variant="ghost" onClick={() => openEdit(c)}>✏️</Button>}
                    {!c.isDefault && <Button size="sm" variant="ghost" onClick={() => setDeleting(c)}>🗑️</Button>}
                  </div>
                </div>
              ))}
            </div>
          )}
      </Card>

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Edit Category' : 'New Category'}>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Input label="Name" placeholder="Groceries" error={errors.name?.message} {...register('name')} />
          {!editing && (
            <Select label="Type" options={[{ value: 0, label: 'Income' }, { value: 1, label: 'Expense' }]} error={errors.type?.message} onChange={e => setValue('type', Number(e.target.value))} defaultValue={1} />
          )}
          <Input label="Icon (emoji)" placeholder="🛒" error={errors.icon?.message} {...register('icon')} />
          <Input label="Color (hex)" placeholder="#ef4444" error={errors.color?.message} {...register('color')} />
          <div className="flex gap-3 justify-end pt-2">
            <Button type="button" variant="secondary" onClick={() => setModalOpen(false)}>Cancel</Button>
            <Button type="submit" loading={isSubmitting}>{editing ? 'Save' : 'Create'}</Button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog open={!!deleting} title="Delete Category" message={`Delete "${deleting?.name}"? This cannot be undone.`} onConfirm={onDelete} onCancel={() => setDeleting(null)} />
    </div>
  );
}
