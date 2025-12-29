import { describe, expect, it, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { RouterProvider, createMemoryRouter } from 'react-router-dom';

import { ExamPage } from '../ExamPage';

vi.mock('../../../service/examsApi', () => ({
  fetchExams: vi.fn(),
  deleteExam: vi.fn(),
}));

const { fetchExams, deleteExam } = await import('../../../service/examsApi');

describe('ExamPage', () => {
  it('loads and renders exams from API', async () => {
    (fetchExams as any).mockResolvedValue([
      { id: 'e1', courseName: 'Math', dateTime: '2999-01-01T10:00', location: 'Room 101' },
    ]);

    const router = createMemoryRouter([{ path: '/faculty/exams', element: <ExamPage /> }], {
      initialEntries: ['/faculty/exams'],
    });

    render(<RouterProvider router={router} />);

    await waitFor(() => expect(fetchExams).toHaveBeenCalledTimes(1));

    expect(await screen.findByText('Math')).toBeInTheDocument();
    expect(screen.getByText('Room 101')).toBeInTheDocument();
    expect(screen.getByText('2999-01-01 10:00')).toBeInTheDocument();
  });

  it('shows success message from sessionStorage toast flag', async () => {
    (fetchExams as any).mockResolvedValue([]);

    sessionStorage.setItem('exams.toast', 'created');

    const router = createMemoryRouter([{ path: '/faculty/exams', element: <ExamPage /> }], {
      initialEntries: ['/faculty/exams'],
    });

    render(<RouterProvider router={router} />);

    expect(await screen.findByText('Exam created successfully.')).toBeInTheDocument();
    expect(sessionStorage.getItem('exams.toast')).toBeNull();
  });

  it('shows updated success message from sessionStorage toast flag', async () => {
    (fetchExams as any).mockResolvedValue([]);

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

    (fetchExams as any).mockResolvedValue([]);

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

    (fetchExams as any).mockResolvedValue([
      { id: 'e1', courseName: 'Math', dateTime: '2999-01-01T10:00', location: 'Room 101' },
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

    (fetchExams as any).mockResolvedValue([
      { id: 'e1', courseName: 'Math', dateTime: '2999-01-01T10:00', location: 'Room 101' },
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
    (fetchExams as any).mockRejectedValue(new Error('fail'));

    const router = createMemoryRouter([{ path: '/faculty/exams', element: <ExamPage /> }], {
      initialEntries: ['/faculty/exams'],
    });

    render(<RouterProvider router={router} />);

    expect(await screen.findByText('Failed to load exams.')).toBeInTheDocument();
  });

  it('deletes an exam via API and reloads list', async () => {
    const user = userEvent.setup();

    (fetchExams as any)
      .mockResolvedValueOnce([
        { id: 'e1', courseName: 'Math', dateTime: '2999-01-01T10:00', location: 'Room 101' },
      ])
      .mockResolvedValueOnce([]);

    (deleteExam as any).mockResolvedValue(undefined);

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

    await waitFor(() => expect(deleteExam).toHaveBeenCalledWith('e1'));
    await waitFor(() => expect(fetchExams).toHaveBeenCalledTimes(2));

    expect(await screen.findByText('No exams found.')).toBeInTheDocument();
  });

  it('shows error when delete fails', async () => {
    const user = userEvent.setup();

    (fetchExams as any).mockResolvedValue([
      { id: 'e1', courseName: 'Math', dateTime: '2999-01-01T10:00', location: 'Room 101' },
    ]);

    (deleteExam as any).mockRejectedValue(new Error('delete failed'));

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
