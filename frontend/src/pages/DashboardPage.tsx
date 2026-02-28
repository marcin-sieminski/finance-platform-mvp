import { useEffect, useState } from 'react';
import { getDashboardSummary } from '../api/dashboard';
import type { DashboardSummary, Transaction } from '../types';
import Card from '../components/ui/Card';
import Spinner from '../components/ui/Spinner';
import MonthlyBarChart from '../components/charts/MonthlyBarChart';
import ExpensePieChart from '../components/charts/ExpensePieChart';

function SummaryCard({ label, value, icon, color }: { label: string; value: string; icon: string; color: string }) {
  return (
    <Card className="flex items-center gap-4">
      <div className={`w-12 h-12 rounded-xl flex items-center justify-center text-2xl ${color}`}>{icon}</div>
      <div>
        <p className="text-sm text-gray-500">{label}</p>
        <p className="text-2xl font-bold text-gray-900">{value}</p>
      </div>
    </Card>
  );
}

function fmt(n: number) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(n);
}

function RecentTransactionRow({ t }: { t: Transaction }) {
  const isIncome = t.type === 0;
  return (
    <div className="flex items-center justify-between py-3 border-b border-gray-50 last:border-0">
      <div className="flex items-center gap-3">
        <div className="w-8 h-8 rounded-full flex items-center justify-center text-sm" style={{ backgroundColor: t.categoryColor ?? '#e5e7eb' }}>
          {t.categoryIcon ?? '📦'}
        </div>
        <div>
          <p className="text-sm font-medium text-gray-900">{t.categoryName}</p>
          <p className="text-xs text-gray-400">{t.accountName} · {new Date(t.date).toLocaleDateString()}</p>
        </div>
      </div>
      <span className={`font-semibold text-sm ${isIncome ? 'text-green-600' : 'text-red-600'}`}>
        {isIncome ? '+' : '-'}{fmt(t.amount)}
      </span>
    </div>
  );
}

export default function DashboardPage() {
  const [data, setData] = useState<DashboardSummary | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getDashboardSummary().then(setData).finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="flex justify-center items-center h-64"><Spinner /></div>;
  if (!data) return <p className="text-gray-500">Could not load dashboard data.</p>;

  return (
    <div className="space-y-6 max-w-7xl">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-500 text-sm">{new Date().toLocaleDateString('en-US', { month: 'long', year: 'numeric' })}</p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
        <SummaryCard label="Total Balance" value={fmt(data.totalBalance)} icon="💳" color="bg-indigo-100" />
        <SummaryCard label="Monthly Income" value={fmt(data.monthlyIncome)} icon="📈" color="bg-green-100" />
        <SummaryCard label="Monthly Expenses" value={fmt(data.monthlyExpenses)} icon="📉" color="bg-red-100" />
        <SummaryCard label="Savings" value={fmt(data.monthlySavings)} icon="💰" color="bg-yellow-100" />
      </div>

      <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
        <MonthlyBarChart data={data.last6MonthsFlow} />
        <ExpensePieChart data={data.expensesByCategory} />
      </div>

      <Card>
        <h3 className="text-base font-semibold text-gray-800 mb-1">Recent Transactions</h3>
        {data.recentTransactions.length === 0
          ? <p className="text-gray-400 text-sm py-8 text-center">No transactions yet</p>
          : data.recentTransactions.map(t => <RecentTransactionRow key={t.id} t={t} />)
        }
      </Card>
    </div>
  );
}
