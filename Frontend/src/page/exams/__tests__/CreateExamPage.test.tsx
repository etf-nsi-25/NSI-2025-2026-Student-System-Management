import { describe, expect, it, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { RouterProvider, createMemoryRouter } from 'react-router-dom';

import { CreateExamPage } from '../CreateExamPage';

vi.mock('../../../service/examsApi', () => ({
  createExam: vi.fn(),
}));

vi.mock('../../../service/courseService', () => ({
  courseService: {
    getAll: vi.fn(),
  },
}));

const { createExam } = await import('../../../service/examsApi');
const { courseService } = await import('../../../service/courseService');

describe('CreateExamPage', () => {
  it('loads courses and creates an exam (happy path)', async () => {
    const user = userEvent.setup();

    (courseService.getAll as any).mockResolvedValue([{ id: 'c1', name: 'Math' }]);
    (createExam as any).mockResolvedValue(undefined);

    sessionStorage.clear();

    const router = createMemoryRouter(
      [
        { path: '/faculty/exams/create', element: <CreateExamPage /> },
        { path: '/faculty/exams', element: <div>Exams list</div> },
      ],
      { initialEntries: ['/faculty/exams/create'] },
    );

    render(<RouterProvider router={router} />);

    await waitFor(() => expect(courseService.getAll).toHaveBeenCalled());

    await user.selectOptions(screen.getByRole('combobox'), 'c1');

    const dateInput = document.querySelector('input[type="datetime-local"]') as HTMLInputElement;
    await user.type(dateInput, '2999-01-01T10:00');

    await user.type(screen.getByPlaceholderText('e.g., Room 101'), 'Room 101');

    await user.click(screen.getByRole('button', { name: 'Save' }));

    await waitFor(() => expect(createExam).toHaveBeenCalledTimes(1));
    expect(createExam).toHaveBeenCalledWith({
      courseName: 'Math',
      dateTime: '2999-01-01T10:00',
      location: 'Room 101',
    });

    await waitFor(() => expect(screen.getByText('Exams list')).toBeInTheDocument());
    expect(sessionStorage.getItem('exams.toast')).toBe('created');
  });

  it('shows warning when courses fail to load', async () => {
    (courseService.getAll as any).mockRejectedValue(new Error('courses down'));

    const router = createMemoryRouter(
      [{ path: '/faculty/exams/create', element: <CreateExamPage /> }],
      { initialEntries: ['/faculty/exams/create'] },
    );

    render(<RouterProvider router={router} />);

    expect(await screen.findByText('Failed to load courses.')).toBeInTheDocument();
  });

  it('shows API error when create fails', async () => {
    const user = userEvent.setup();

    (courseService.getAll as any).mockResolvedValue([{ id: 'c1', name: 'Math' }]);
    (createExam as any).mockRejectedValue(new Error('boom'));

    const router = createMemoryRouter(
      [
        { path: '/faculty/exams/create', element: <CreateExamPage /> },
        { path: '/faculty/exams', element: <div>Exams list</div> },
      ],
      { initialEntries: ['/faculty/exams/create'] },
    );

    render(<RouterProvider router={router} />);

    await waitFor(() => expect(courseService.getAll).toHaveBeenCalled());

    await user.selectOptions(screen.getByRole('combobox'), 'c1');

    const dateInput = document.querySelector('input[type="datetime-local"]') as HTMLInputElement;
    await user.type(dateInput, '2999-01-01T10:00');

    await user.type(screen.getByPlaceholderText('e.g., Room 101'), 'Room 101');

    await user.click(screen.getByRole('button', { name: 'Save' }));

    await waitFor(() => expect(createExam).toHaveBeenCalledTimes(1));
    expect(await screen.findByText('boom')).toBeInTheDocument();
  });

  it('shows warning when no courses exist', async () => {
    (courseService.getAll as any).mockResolvedValue([]);

    const router = createMemoryRouter(
      [{ path: '/faculty/exams/create', element: <CreateExamPage /> }],
      { initialEntries: ['/faculty/exams/create'] },
    );

    render(<RouterProvider router={router} />);

    expect(await screen.findByText('No courses available.')).toBeInTheDocument();
  });
});
