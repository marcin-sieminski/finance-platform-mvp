import { type ReactNode } from 'react';
import clsx from 'clsx';

interface CardProps {
  className?: string;
  children: ReactNode;
}

export default function Card({ className, children }: CardProps) {
  return (
    <div className={clsx('bg-white rounded-xl shadow-sm border border-gray-100 p-5', className)}>
      {children}
    </div>
  );
}
