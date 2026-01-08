import { beforeEach, describe, expect, it, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { RouterProvider, createMemoryRouter } from 'react-router-dom';

import { CreateExamPage } from '../CreateExamPage';

const apiMock = {
  getAllCourses: vi.fn(),
  createExam: vi.fn(),
};

vi.mock('../../../context/services.tsx', () => ({
  useAPI: () => apiMock,
}));

describe('CreateExamPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    sessionStorage.clear();
  });

  it('loads courses and creates an exam (happy path)', async () => {
    const user = userEvent.setup();

    apiMock.getAllCourses.mockResolvedValue([{ id: 'c1', name: 'Math' }]);
    apiMock.createExam.mockResolvedValue({ id: 1 });

    const router = createMemoryRouter(
      [
        { path: '/faculty/exams/create', element: <CreateExamPage /> },
        { path: '/faculty/exams', element: <div>Exams list</div> },
      ],
      { initialEntries: ['/faculty/exams/create'] },
    );

    render(<RouterProvider router={router} />);

    await waitFor(() => expect(apiMock.getAllCourses).toHaveBeenCalled());

    await user.selectOptions(screen.getAllByRole('combobox')[0], 'c1');

    const dateInputs = Array.from(document.querySelectorAll('input[type="datetime-local"]')) as HTMLInputElement[];
    const examDateInput = dateInputs[0];
    const deadlineInput = dateInputs[1];
    await user.type(examDateInput, '2999-01-01T10:00');
    await user.type(deadlineInput, '2999-01-01T09:00');

    await user.type(screen.getByPlaceholderText('e.g., Room 101'), 'Room 101');

    await user.click(screen.getByRole('button', { name: 'Save' }));

    await waitFor(() => expect(apiMock.createExam).toHaveBeenCalledTimes(1));
    expect(apiMock.createExam).toHaveBeenCalledWith({
      courseId: 'c1',
      name: 'Math',
      location: 'Room 101',
      examType: 'Written',
      examDate: '2999-01-01T10:00',
      regDeadline: '2999-01-01T09:00',
    });

    await waitFor(() => expect(screen.getByText('Exams list')).toBeInTheDocument());
    expect(sessionStorage.getItem('exams.toast')).toBe('created');
  });

  it('shows warning when courses fail to load', async () => {
    apiMock.getAllCourses.mockRejectedValue(new Error('courses down'));

    const router = createMemoryRouter(
      [{ path: '/faculty/exams/create', element: <CreateExamPage /> }],
      { initialEntries: ['/faculty/exams/create'] },
    );

    render(<RouterProvider router={router} />);

    expect(await screen.findByText('Failed to load courses.')).toBeInTheDocument();
  });

  it('shows API error when create fails', async () => {
    const user = userEvent.setup();

    apiMock.getAllCourses.mockResolvedValue([{ id: 'c1', name: 'Math' }]);
    apiMock.createExam.mockRejectedValue(new Error('boom'));

    const router = createMemoryRouter(
      [
        { path: '/faculty/exams/create', element: <CreateExamPage /> },
        { path: '/faculty/exams', element: <div>Exams list</div> },
      ],
      { initialEntries: ['/faculty/exams/create'] },
    );

    render(<RouterProvider router={router} />);

    await waitFor(() => expect(apiMock.getAllCourses).toHaveBeenCalled());

    await user.selectOptions(screen.getAllByRole('combobox')[0], 'c1');

    const dateInputs = Array.from(document.querySelectorAll('input[type="datetime-local"]')) as HTMLInputElement[];
    const examDateInput = dateInputs[0];
    const deadlineInput = dateInputs[1];
    await user.type(examDateInput, '2999-01-01T10:00');
    await user.type(deadlineInput, '2999-01-01T09:00');

    await user.type(screen.getByPlaceholderText('e.g., Room 101'), 'Room 101');

    await user.click(screen.getByRole('button', { name: 'Save' }));

    await waitFor(() => expect(apiMock.createExam).toHaveBeenCalledTimes(1));
    expect(await screen.findByText('boom')).toBeInTheDocument();
  });

  it('shows warning when no courses exist', async () => {
    apiMock.getAllCourses.mockResolvedValue([]);

    const router = createMemoryRouter(
      [{ path: '/faculty/exams/create', element: <CreateExamPage /> }],
      { initialEntries: ['/faculty/exams/create'] },
    );

    render(<RouterProvider router={router} />);

    expect(await screen.findByText('No courses available.')).toBeInTheDocument();
  });
});
