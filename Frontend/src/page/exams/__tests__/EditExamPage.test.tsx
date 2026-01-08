import { describe, expect, it, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { RouterProvider, createMemoryRouter } from 'react-router-dom';

import { EditExamPage } from '../EditExamPage';

const apiMock = {
  getAllCourses: vi.fn(),
  getExam: vi.fn(),
  updateExam: vi.fn(),
};

vi.mock('../../../context/services.tsx', () => ({
  useAPI: () => apiMock,
}));

describe('EditExamPage', () => {
  it('preloads exam, updates via PUT, and redirects', async () => {
    const user = userEvent.setup();

    apiMock.getAllCourses.mockResolvedValue([{ id: 'c1', name: 'Math' }]);
    apiMock.getExam.mockResolvedValue({
      id: 1,
      courseId: 'c1',
      courseName: 'Math',
      examType: 'Written',
      examDate: '2999-01-01T10:00',
      regDeadline: '2999-01-01T09:00',
      location: 'Room 101',
      createdAt: '2026-01-01T00:00:00Z',
    });
    apiMock.updateExam.mockResolvedValue({ id: 1 });

    sessionStorage.clear();

    const router = createMemoryRouter(
      [
        { path: '/faculty/exams/:id/edit', element: <EditExamPage /> },
        { path: '/faculty/exams', element: <div>Exams list</div> },
      ],
      { initialEntries: ['/faculty/exams/e1/edit'] },
    );

    render(<RouterProvider router={router} />);

    await waitFor(() => expect(apiMock.getExam).toHaveBeenCalledWith('e1'));
    await waitFor(() => expect(apiMock.getAllCourses).toHaveBeenCalled());

    const locationInput = await screen.findByDisplayValue('Room 101');
    expect(locationInput).toBeInTheDocument();

    await user.clear(locationInput);
    await user.type(locationInput, 'Room 202');

    await user.click(screen.getByRole('button', { name: 'Save' }));

    await waitFor(() => expect(apiMock.updateExam).toHaveBeenCalledTimes(1));
    expect(apiMock.updateExam).toHaveBeenCalledWith('e1', {
      courseId: 'c1',
      name: 'Math',
      location: 'Room 202',
      examType: 'Written',
      examDate: '2999-01-01T10:00',
      regDeadline: '2999-01-01T09:00',
    });

    await waitFor(() => expect(screen.getByText('Exams list')).toBeInTheDocument());
    expect(sessionStorage.getItem('exams.toast')).toBe('updated');
  });

  it('shows warning when no courses exist', async () => {
    apiMock.getAllCourses.mockResolvedValue([]);
    apiMock.getExam.mockResolvedValue({
      id: 1,
      courseId: 'c1',
      courseName: 'Math',
      examType: 'Written',
      examDate: '2999-01-01T10:00',
      regDeadline: '2999-01-01T09:00',
      location: 'Room 101',
      createdAt: '2026-01-01T00:00:00Z',
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

    apiMock.getAllCourses.mockResolvedValue([{ id: 'c1', name: 'Math' }]);
    apiMock.getExam.mockResolvedValue({
      id: 1,
      courseId: 'c1',
      courseName: 'Math',
      examType: 'Written',
      examDate: '2999-01-01T10:00',
      regDeadline: '2999-01-01T09:00',
      location: 'Room 101',
      createdAt: '2026-01-01T00:00:00Z',
    });
    apiMock.updateExam.mockRejectedValue(new Error('update boom'));

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
    apiMock.getAllCourses.mockResolvedValue([{ id: 'c1', name: 'Math' }]);
    apiMock.getExam.mockRejectedValue(new Error('nope'));

    const router = createMemoryRouter(
      [{ path: '/faculty/exams/:id/edit', element: <EditExamPage /> }],
      { initialEntries: ['/faculty/exams/e1/edit'] },
    );

    render(<RouterProvider router={router} />);

    expect(await screen.findByText('nope')).toBeInTheDocument();
  });
});
