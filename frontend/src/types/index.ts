// Auth
export interface AuthResponse {
  token: string;
  email: string;
  firstName: string;
  lastName: string;
  expiresAt: string;
}

export interface UserProfile {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  createdAt: string;
}

// Accounts
export type AccountType = 'Checking' | 'Savings' | 'CreditCard' | 'Cash' | 'Investment';

export interface Account {
  id: string;
  name: string;
  type: AccountType;
  balance: number;
  currency: string;
  color: string | null;
  isArchived: boolean;
  createdAt: string;
}

export interface CreateAccountRequest {
  name: string;
  type: number;
  balance: number;
  currency: string;
  color?: string;
}

export interface UpdateAccountRequest {
  name: string;
  type: number;
  currency: string;
  color?: string;
}

// Categories
export type CategoryType = 'Income' | 'Expense';

export interface Category {
  id: string;
  name: string;
  type: number; // 0=Income, 1=Expense
  icon: string | null;
  color: string | null;
  isDefault: boolean;
}

// Transactions
export type TransactionType = 'Income' | 'Expense';

export interface Transaction {
  id: string;
  accountId: string;
  accountName: string;
  categoryId: string;
  categoryName: string;
  categoryColor: string | null;
  categoryIcon: string | null;
  amount: number;
  type: number; // 0=Income, 1=Expense
  description: string | null;
  date: string;
  createdAt: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// Budgets
export interface Budget {
  id: string;
  categoryId: string;
  categoryName: string;
  categoryColor: string | null;
  categoryIcon: string | null;
  limitAmount: number;
  spentAmount: number;
  remainingAmount: number;
  progressPercent: number;
  month: number;
  year: number;
}

// Dashboard
export interface MonthlyFlowDataPoint {
  month: string;
  monthNumber: number;
  year: number;
  income: number;
  expenses: number;
}

export interface CategoryBreakdownItem {
  categoryId: string;
  name: string;
  color: string | null;
  amount: number;
  percent: number;
}

export interface DashboardSummary {
  totalBalance: number;
  monthlyIncome: number;
  monthlyExpenses: number;
  monthlySavings: number;
  last6MonthsFlow: MonthlyFlowDataPoint[];
  expensesByCategory: CategoryBreakdownItem[];
  recentTransactions: Transaction[];
}
