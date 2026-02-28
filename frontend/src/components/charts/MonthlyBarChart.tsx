import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import type { MonthlyFlowDataPoint } from '../../types';
import Card from '../ui/Card';

export default function MonthlyBarChart({ data }: { data: MonthlyFlowDataPoint[] }) {
  return (
    <Card>
      <h3 className="text-base font-semibold text-gray-800 mb-4">Income vs Expenses (6 months)</h3>
      <ResponsiveContainer width="100%" height={240}>
        <BarChart data={data} margin={{ top: 0, right: 0, left: 0, bottom: 0 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
          <XAxis dataKey="month" tick={{ fontSize: 12 }} />
          <YAxis tick={{ fontSize: 12 }} />
          <Tooltip formatter={(v) => `$${Number(v).toLocaleString()}`} />
          <Legend />
          <Bar dataKey="income" name="Income" fill="#22c55e" radius={[4, 4, 0, 0]} />
          <Bar dataKey="expenses" name="Expenses" fill="#ef4444" radius={[4, 4, 0, 0]} />
        </BarChart>
      </ResponsiveContainer>
    </Card>
  );
}
