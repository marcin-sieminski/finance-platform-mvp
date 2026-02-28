import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Link, useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { register as apiRegister } from '../../api/auth';
import { useAuthStore } from '../../store/authStore';
import Input from '../../components/ui/Input';
import Button from '../../components/ui/Button';

const schema = z.object({
  firstName: z.string().min(1, 'Required'),
  lastName: z.string().min(1, 'Required'),
  email: z.string().email('Invalid email'),
  password: z.string().min(8, 'At least 8 characters'),
  confirmPassword: z.string(),
}).refine(d => d.password === d.confirmPassword, {
  message: 'Passwords do not match',
  path: ['confirmPassword'],
});

type FormData = z.infer<typeof schema>;

export default function RegisterPage() {
  const nav = useNavigate();
  const { login: storeLogin } = useAuthStore();
  const [error, setError] = useState('');

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema),
  });

  const onSubmit = async (data: FormData) => {
    try {
      setError('');
      const result = await apiRegister(data);
      storeLogin(result.token, { email: result.email, firstName: result.firstName, lastName: result.lastName });
      nav('/', { replace: true });
    } catch (e: unknown) {
      const msg = (e as { response?: { data?: { title?: string } } })?.response?.data?.title;
      setError(msg ?? 'Registration failed. Please try again.');
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center">
      <div className="bg-white rounded-2xl shadow-lg w-full max-w-md p-8">
        <div className="text-center mb-8">
          <div className="text-4xl mb-3">💰</div>
          <h1 className="text-2xl font-bold text-gray-900">Create account</h1>
          <p className="text-gray-500 text-sm mt-1">Start tracking your finances</p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {error && <div className="bg-red-50 text-red-700 text-sm px-4 py-3 rounded-lg">{error}</div>}
          <div className="grid grid-cols-2 gap-3">
            <Input label="First name" placeholder="John" error={errors.firstName?.message} {...register('firstName')} />
            <Input label="Last name" placeholder="Doe" error={errors.lastName?.message} {...register('lastName')} />
          </div>
          <Input label="Email" type="email" placeholder="you@example.com" error={errors.email?.message} {...register('email')} />
          <Input label="Password" type="password" placeholder="Min. 8 characters" error={errors.password?.message} {...register('password')} />
          <Input label="Confirm password" type="password" placeholder="Repeat password" error={errors.confirmPassword?.message} {...register('confirmPassword')} />
          <Button type="submit" className="w-full" size="lg" loading={isSubmitting}>Create account</Button>
        </form>

        <p className="text-center text-sm text-gray-500 mt-6">
          Already have an account?{' '}
          <Link to="/login" className="text-indigo-600 font-medium hover:underline">Sign in</Link>
        </p>
      </div>
    </div>
  );
}
