import { NavLink } from 'react-router-dom';
import clsx from 'clsx';
import { useAuthStore } from '../../store/authStore';

const navItems = [
  { to: '/', label: 'Dashboard', icon: '📊' },
  { to: '/accounts', label: 'Accounts', icon: '🏦' },
  { to: '/transactions', label: 'Transactions', icon: '↕️' },
  { to: '/budgets', label: 'Budgets', icon: '🎯' },
  { to: '/categories', label: 'Categories', icon: '🏷️' },
];

export default function Sidebar() {
  const { user, logout } = useAuthStore();

  return (
    <aside className="w-64 min-h-screen bg-indigo-900 text-white flex flex-col">
      <div className="p-6 border-b border-indigo-800">
        <h1 className="text-xl font-bold">💰 FinanceApp</h1>
        {user && (
          <p className="text-indigo-300 text-sm mt-1">{user.firstName} {user.lastName}</p>
        )}
      </div>

      <nav className="flex-1 p-4 space-y-1">
        {navItems.map(item => (
          <NavLink
            key={item.to}
            to={item.to}
            end={item.to === '/'}
            className={({ isActive }) => clsx(
              'flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm font-medium transition-colors',
              isActive
                ? 'bg-indigo-700 text-white'
                : 'text-indigo-200 hover:bg-indigo-800 hover:text-white'
            )}
          >
            <span>{item.icon}</span>
            {item.label}
          </NavLink>
        ))}
      </nav>

      <div className="p-4 border-t border-indigo-800">
        <button
          onClick={logout}
          className="w-full text-left px-4 py-2.5 text-sm text-indigo-300 hover:text-white hover:bg-indigo-800 rounded-lg transition-colors"
        >
          🚪 Sign out
        </button>
      </div>
    </aside>
  );
}
