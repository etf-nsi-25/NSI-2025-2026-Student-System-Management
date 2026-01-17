import { describe, expect, it, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { RouterProvider, createMemoryRouter } from 'react-router-dom';

import { ExamPage } from '../ExamPage';

const apiMock = {
  getExams: vi.fn(),
  deleteExam: vi.fn(),
};

vi.mock('../../../context/services.tsx', () => ({
  useAPI: () => apiMock,
}));

describe('ExamPage', () => {
  it('loads and renders exams from API', async () => {
    apiMock.getExams.mockResolvedValue([
      {
        id: 1,
        courseId: 'c1',
        courseName: 'Math',
        examType: 'Written',
        examDate: '2999-01-01T10:00',
        regDeadline: '2999-01-01T09:00',
        location: 'Room 101',
        createdAt: '2026-01-01T00:00:00Z',
      },
    ]);

    const router = createMemoryRouter([{ path: '/faculty/exams', element: <ExamPage /> }], {
      initialEntries: ['/faculty/exams'],
    });

    render(<RouterProvider router={router} />);

    await waitFor(() => expect(apiMock.getExams).toHaveBeenCalledTimes(1));

    expect(await screen.findByText('Math')).toBeInTheDocument();
    expect(screen.getByText('Room 101')).toBeInTheDocument();
    expect(screen.getByText('2999-01-01 10:00')).toBeInTheDocument();
  });

  it('shows success message from sessionStorage toast flag', async () => {
    apiMock.getExams.mockResolvedValue([]);

    sessionStorage.setItem('exams.toast', 'created');

    const router = createMemoryRouter([{ path: '/faculty/exams', element: <ExamPage /> }], {
      initialEntries: ['/faculty/exams'],
    });

    render(<RouterProvider router={router} />);

    expect(await screen.findByText('Exam created successfully.')).toBeInTheDocument();
    expect(sessionStorage.getItem('exams.toast')).toBeNull();
  });

  it('shows updated success message from sessionStorage toast flag', async () => {
    apiMock.getExams.mockResolvedValue([]);

    sessionStorage.setItem('exams.toast', 'updated');

    const router = createMemoryRouter([{ path: '/faculty/exams', element: <ExamPage /> }], {
      initialEntries: ['/faculty/exams'],
    });

    render(<RouterProvider router={router} />);

    expect(await screen.findByText('Exam updated successfully.')).toBeInTheDocument();
    expect(sessionStorage.getItem('exams.toast')).toBeNull();
  });

  it('navigates to create page when clicking "+ Create exam"', async () => {
    const user = userEvent.setup();

    apiMock.getExams.mockResolvedValue([]);

    const router = createMemoryRouter(
      [
        { path: '/faculty/exams', element: <ExamPage /> },
        { path: '/faculty/exams/create', element: <div>Create page</div> },
      ],
      { initialEntries: ['/faculty/exams'] },
    );

    render(<RouterProvider router={router} />);

    await user.click(screen.getByRole('button', { name: '+ Create exam' }));
    expect(await screen.findByText('Create page')).toBeInTheDocument();
  });

  it('navigates to edit page when clicking Edit', async () => {
    const user = userEvent.setup();

    apiMock.getExams.mockResolvedValue([
      {
        id: 1,
        courseId: 'c1',
        courseName: 'Math',
        examType: 'Written',
        examDate: '2999-01-01T10:00',
        regDeadline: '2999-01-01T09:00',
        location: 'Room 101',
        createdAt: '2026-01-01T00:00:00Z',
      },
    ]);

    const router = createMemoryRouter(
      [
        { path: '/faculty/exams', element: <ExamPage /> },
        { path: '/faculty/exams/:id/edit', element: <div>Edit page</div> },
      ],
      { initialEntries: ['/faculty/exams'] },
    );

    render(<RouterProvider router={router} />);

    await screen.findByText('Math');
    await user.click(screen.getByRole('button', { name: 'Edit' }));

    expect(await screen.findByText('Edit page')).toBeInTheDocument();
  });

  it('closes delete modal when clicking Cancel', async () => {
    const user = userEvent.setup();

    apiMock.getExams.mockResolvedValue([
      {
        id: 1,
        courseId: 'c1',
        courseName: 'Math',
        examType: 'Written',
        examDate: '2999-01-01T10:00',
        regDeadline: '2999-01-01T09:00',
        location: 'Room 101',
        createdAt: '2026-01-01T00:00:00Z',
      },
    ]);

    const router = createMemoryRouter([{ path: '/faculty/exams', element: <ExamPage /> }], {
      initialEntries: ['/faculty/exams'],
    });

    render(<RouterProvider router={router} />);

    await screen.findByText('Math');

    await user.click(screen.getAllByRole('button', { name: 'Delete' })[0]);
    expect(screen.getByText('Confirm deletion')).toBeInTheDocument();

    await user.click(screen.getByRole('button', { name: 'Cancel' }));
    await waitFor(() => expect(screen.queryByText('Confirm deletion')).not.toBeInTheDocument());
  });

  it('shows error when API load fails', async () => {
    apiMock.getExams.mockRejectedValue(new Error('fail'));

    const router = createMemoryRouter([{ path: '/faculty/exams', element: <ExamPage /> }], {
      initialEntries: ['/faculty/exams'],
    });

    render(<RouterProvider router={router} />);

    expect(await screen.findByText('Failed to load exams.')).toBeInTheDocument();
  });

  it('deletes an exam via API and reloads list', async () => {
    const user = userEvent.setup();

    apiMock.getExams.mockResolvedValue([
      {
        id: 1,
        courseId: 'c1',
        courseName: 'Math',
        examType: 'Written',
        examDate: '2999-01-01T10:00',
        regDeadline: '2999-01-01T09:00',
        location: 'Room 101',
        createdAt: '2026-01-01T00:00:00Z',
      },
    ]);

    apiMock.deleteExam.mockResolvedValue(undefined);

    const router = createMemoryRouter([{ path: '/faculty/exams', element: <ExamPage /> }], {
      initialEntries: ['/faculty/exams'],
    });

    render(<RouterProvider router={router} />);

    await screen.findByText('Math');

    // first Delete opens the modal (row action)
    await user.click(screen.getAllByRole('button', { name: 'Delete' })[0]);
    expect(screen.getByText('Confirm deletion')).toBeInTheDocument();

    // second Delete confirms inside modal
    const deleteButtons = screen.getAllByRole('button', { name: 'Delete' });
    await user.click(deleteButtons[deleteButtons.length - 1]);

    await waitFor(() => expect(apiMock.deleteExam).toHaveBeenCalledWith(1));

    expect(await screen.findByText('No exams found.')).toBeInTheDocument();
  });

  it('shows error when delete fails', async () => {
    const user = userEvent.setup();

    apiMock.getExams.mockResolvedValue([
      {
        id: 1,
        courseId: 'c1',
        courseName: 'Math',
        examType: 'Written',
        examDate: '2999-01-01T10:00',
        regDeadline: '2999-01-01T09:00',
        location: 'Room 101',
        createdAt: '2026-01-01T00:00:00Z',
      },
    ]);

    apiMock.deleteExam.mockRejectedValue(new Error('delete failed'));

    const router = createMemoryRouter([{ path: '/faculty/exams', element: <ExamPage /> }], {
      initialEntries: ['/faculty/exams'],
    });

    render(<RouterProvider router={router} />);

    await screen.findByText('Math');

    await user.click(screen.getAllByRole('button', { name: 'Delete' })[0]);

    const deleteButtons = screen.getAllByRole('button', { name: 'Delete' });
    await user.click(deleteButtons[deleteButtons.length - 1]);

    expect(await screen.findByText('delete failed')).toBeInTheDocument();
  });
});
