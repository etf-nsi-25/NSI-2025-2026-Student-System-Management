// @vitest-environment jsdom
import { cleanup, render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { Login } from '../../../page/login/login';
import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest';
import { useAuthContext } from '../../../init/auth';
import { useNavigate } from 'react-router';
import { getDashboardRoute } from '../../../constants/roles';

// Mock dependencies
vi.mock('react-router', () => ({
  useNavigate: vi.fn(),
}));

vi.mock('../../../init/auth', () => ({
  useAuthContext: vi.fn(),
}));

// Mock image
vi.mock('../../../assets/images/login/unsa-sms-logo.png', () => ({
  default: 'test-file-stub',
}));

const mockUser = { 
    email: 'test@example.com', 
    role: 'Student', 
    password: 'password123!' 
};

describe('Login', () => {
  const mockNavigate = vi.fn();
  const mockSetAuthInfo = vi.fn();
  let mockFetch: ReturnType<typeof vi.spyOn>;

  beforeEach(() => {
    vi.clearAllMocks();
    (useNavigate as unknown as ReturnType<typeof vi.fn>).mockReturnValue(mockNavigate);
    (useAuthContext as unknown as ReturnType<typeof vi.fn>).mockReturnValue({ setAuthInfo: mockSetAuthInfo });
    
    globalThis.fetch = vi.fn();
    mockFetch = vi.spyOn(globalThis, 'fetch');
  });

  afterEach(() => {
    cleanup();
  });

  it('should render login form correctly', () => {
    render(<Login />);
    
    expect(screen.getByRole('heading', { name: /login/i })).toBeInTheDocument();
    expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
    expect(screen.getByLabelText('Password', { selector: 'input' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Sign in' })).toBeInTheDocument();
  });

  it('should update input fields on user type', async () => {
    render(<Login />);
    const user = userEvent.setup();
    
    const emailInput = screen.getByLabelText(/email/i);
    const passwordInput = screen.getByLabelText('Password', { selector: 'input' });

    await user.type(emailInput, mockUser.email);
    await user.type(passwordInput, mockUser.password);

    expect(emailInput).toHaveValue(mockUser.email);
    expect(passwordInput).toHaveValue(mockUser.password);
  });

  it('should toggle password visibility', async () => {
    render(<Login />);
    const user = userEvent.setup();
    
    const passwordInput = screen.getByLabelText('Password', { selector: 'input' });
    const toggleButton = screen.getByRole('button', { name: /show password/i });

    expect(passwordInput).toHaveAttribute('type', 'password');

    await user.click(toggleButton);
    expect(passwordInput).toHaveAttribute('type', 'text');
    expect(screen.getByRole('button', { name: /hide password/i })).toBeInTheDocument();

    await user.click(toggleButton);
    expect(passwordInput).toHaveAttribute('type', 'password');
  });

  it('should display validation errors', async () => {
    render(<Login />);
    const user = userEvent.setup();

    // Attempt to submit with empty fields
    await user.click(screen.getByRole('button', { name: 'Sign in' }));

    expect(screen.getByText('Email is required')).toBeInTheDocument();
    expect(mockFetch).not.toHaveBeenCalled();
    
    // Fix email, check password error
    await user.type(screen.getByLabelText(/email/i), 'test@example.com');
    await user.click(screen.getByRole('button', { name: 'Sign in' }));
    
    expect(screen.getByText('Password is required')).toBeInTheDocument();
    expect(mockFetch).not.toHaveBeenCalled();
  });

  it('should handle successful login', async () => {
    // Create a dummy JWT
    const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
    const payload = btoa(JSON.stringify({
      exp: Math.floor(Date.now() / 1000) + 3600,
      email: mockUser.email,
      userId: '123',
      role: mockUser.role,
      tenantId: 'tenant1',
      fullName: 'Test User'
    }));
    const signature = 'dummy-signature';
    const mockToken = `${header}.${payload}.${signature}`;
    
    mockFetch.mockResolvedValue({
      ok: true,
      json: async () => ({ accessToken: mockToken }),
    });

    render(<Login />);
    const user = userEvent.setup();

    await user.type(screen.getByLabelText(/email/i), mockUser.email);
    await user.type(screen.getByLabelText('Password', { selector: 'input' }), mockUser.password);
    await user.click(screen.getByRole('button', { name: 'Sign in' }));

    await waitFor(() => {
      expect(mockFetch).toHaveBeenCalledWith('/api/auth/login', expect.objectContaining({
        method: 'POST',
        body: JSON.stringify({ email: mockUser.email, password: mockUser.password }),
      }));
    });
    
    expect(mockSetAuthInfo).toHaveBeenCalledWith(expect.objectContaining({
      accessToken: mockToken,
      email: mockUser.email,
      role: mockUser.role,
    }));

    const dashboardRoute = getDashboardRoute(mockUser.role);
    expect(mockNavigate).toHaveBeenCalledWith(dashboardRoute);
  });

  it('should handle login failure', async () => {
    mockFetch.mockResolvedValue({
      ok: false,
      status: 401,
    });

    render(<Login />);
    const user = userEvent.setup();

    await user.type(screen.getByLabelText(/email/i), mockUser.email);
    await user.type(screen.getByLabelText('Password', { selector: 'input' }), mockUser.password);
    await user.click(screen.getByRole('button', { name: 'Sign in' }));

    expect(await screen.findByText(/login failed/i)).toBeInTheDocument();
        
    expect(mockSetAuthInfo).not.toHaveBeenCalled();
    expect(mockNavigate).not.toHaveBeenCalled();
  });
});
