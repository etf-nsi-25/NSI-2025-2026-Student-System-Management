import '@testing-library/jest-dom/vitest';
import { afterAll, afterEach, beforeAll, vi } from 'vitest';
import { cleanup } from '@testing-library/react';

beforeAll(() => {
  if (process.env.VITEST_SHOW_CONSOLE === '1') return;

  vi.spyOn(console, 'error').mockImplementation(() => {});
  vi.spyOn(console, 'warn').mockImplementation(() => {});
});

afterAll(() => {
  if (process.env.VITEST_SHOW_CONSOLE === '1') return;

  (console.error as unknown as { mockRestore?: () => void }).mockRestore?.();
  (console.warn as unknown as { mockRestore?: () => void }).mockRestore?.();
});

afterEach(() => {
  cleanup();
  vi.useRealTimers();
  vi.clearAllMocks();
  sessionStorage.clear();
});
