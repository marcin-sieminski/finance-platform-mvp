import { PieChart, Pie, Cell, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import type { CategoryBreakdownItem } from '../../types';
import Card from '../ui/Card';

const FALLBACK_COLORS = ['#6366f1', '#f59e0b', '#10b981', '#ef4444', '#8b5cf6', '#ec4899', '#14b8a6', '#f97316'];

export default function ExpensePieChart({ data }: { data: CategoryBreakdownItem[] }) {
  if (data.length === 0) {
    return (
      <Card>
        <h3 className="text-base font-semibold text-gray-800 mb-4">Expenses by Category</h3>
        <p className="text-gray-400 text-sm text-center py-12">No expense data this month</p>
      </Card>
    );
  }

  return (
    <Card>
      <h3 className="text-base font-semibold text-gray-800 mb-4">Expenses by Category</h3>
      <ResponsiveContainer width="100%" height={240}>
        <PieChart>
          <Pie data={data} dataKey="amount" nameKey="name" cx="50%" cy="50%" outerRadius={80}>
            {data.map((entry, i) => (
              <Cell key={entry.categoryId} fill={entry.color ?? FALLBACK_COLORS[i % FALLBACK_COLORS.length]} />
            ))}
          </Pie>
          <Tooltip formatter={(v) => `$${Number(v).toLocaleString()}`} />
          <Legend />
        </PieChart>
      </ResponsiveContainer>
    </Card>
  );
}
