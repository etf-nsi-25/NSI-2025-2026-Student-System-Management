import { describe, expect, it, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { RouterProvider, createMemoryRouter } from 'react-router-dom';

import { EditExamPage } from '../EditExamPage';

vi.mock('../../../service/examsApi', () => ({
  getExam: vi.fn(),
  updateExam: vi.fn(),
}));

vi.mock('../../../service/courseService', () => ({
  courseService: {
    getAll: vi.fn(),
  },
}));

const { getExam, updateExam } = await import('../../../service/examsApi');
const { courseService } = await import('../../../service/courseService');

describe('EditExamPage', () => {
  it('preloads exam, updates via PUT, and redirects', async () => {
    const user = userEvent.setup();

    (courseService.getAll as any).mockResolvedValue([{ id: 'c1', name: 'Math' }]);
    (getExam as any).mockResolvedValue({
      id: 'e1',
      courseName: 'Math',
      dateTime: '2999-01-01T10:00',
      location: 'Room 101',
    });
    (updateExam as any).mockResolvedValue(undefined);

    sessionStorage.clear();

    const router = createMemoryRouter(
      [
        { path: '/faculty/exams/:id/edit', element: <EditExamPage /> },
        { path: '/faculty/exams', element: <div>Exams list</div> },
      ],
      { initialEntries: ['/faculty/exams/e1/edit'] },
    );

    render(<RouterProvider router={router} />);

    await waitFor(() => expect(getExam).toHaveBeenCalledWith('e1'));
    await waitFor(() => expect(courseService.getAll).toHaveBeenCalled());

    const locationInput = await screen.findByDisplayValue('Room 101');
    expect(locationInput).toBeInTheDocument();

    await user.clear(locationInput);
    await user.type(locationInput, 'Room 202');

    await user.click(screen.getByRole('button', { name: 'Save' }));

    await waitFor(() => expect(updateExam).toHaveBeenCalledTimes(1));
    expect(updateExam).toHaveBeenCalledWith('e1', {
      courseName: 'Math',
      dateTime: '2999-01-01T10:00',
      location: 'Room 202',
    });

    await waitFor(() => expect(screen.getByText('Exams list')).toBeInTheDocument());
    expect(sessionStorage.getItem('exams.toast')).toBe('updated');
  });

  it('shows warning when no courses exist', async () => {
    (courseService.getAll as any).mockResolvedValue([]);
    (getExam as any).mockResolvedValue({
      id: 'e1',
      courseName: 'Math',
      dateTime: '2999-01-01T10:00',
      location: 'Room 101',
    });

    const router = createMemoryRouter(
      [{ path: '/faculty/exams/:id/edit', element: <EditExamPage /> }],
      { initialEntries: ['/faculty/exams/e1/edit'] },
    );

    render(<RouterProvider router={router} />);

    expect(await screen.findByText('No courses available.')).toBeInTheDocument();
    expect(await screen.findByDisplayValue('Room 101')).toBeInTheDocument();
  });

  it('shows API error when update fails (no redirect)', async () => {
    const user = userEvent.setup();

    (courseService.getAll as any).mockResolvedValue([{ id: 'c1', name: 'Math' }]);
    (getExam as any).mockResolvedValue({
      id: 'e1',
      courseName: 'Math',
      dateTime: '2999-01-01T10:00',
      location: 'Room 101',
    });
    (updateExam as any).mockRejectedValue(new Error('update boom'));

    const router = createMemoryRouter(
      [
        { path: '/faculty/exams/:id/edit', element: <EditExamPage /> },
        { path: '/faculty/exams', element: <div>Exams list</div> },
      ],
      { initialEntries: ['/faculty/exams/e1/edit'] },
    );

    render(<RouterProvider router={router} />);

    const locationInput = await screen.findByDisplayValue('Room 101');
    await user.clear(locationInput);
    await user.type(locationInput, 'Room 202');

    await user.click(screen.getByRole('button', { name: 'Save' }));

    expect(await screen.findByText('update boom')).toBeInTheDocument();
    expect(screen.queryByText('Exams list')).not.toBeInTheDocument();
  });

  it('shows error when preload fails', async () => {
    (courseService.getAll as any).mockResolvedValue([{ id: 'c1', name: 'Math' }]);
    (getExam as any).mockRejectedValue(new Error('nope'));

    const router = createMemoryRouter(
      [{ path: '/faculty/exams/:id/edit', element: <EditExamPage /> }],
      { initialEntries: ['/faculty/exams/e1/edit'] },
    );

    render(<RouterProvider router={router} />);

    expect(await screen.findByText('nope')).toBeInTheDocument();
  });
});
