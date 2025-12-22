import { describe, expect, it, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

import { ExamForm } from '../ExamForm';

describe('ExamForm', () => {
  it('validates required fields and future date', async () => {
    const user = userEvent.setup();
    const onSubmit = vi.fn();

    render(
      <ExamForm
        onSubmit={onSubmit}
        onCancel={() => {}}
        courses={[{ id: 'c1', name: 'Math' }]}
      />,
    );

    // Save is disabled until the form is valid
    const saveButton = screen.getByRole('button', { name: 'Save' });
    expect(saveButton).toBeDisabled();
    expect(onSubmit).not.toHaveBeenCalled();

    await user.selectOptions(screen.getByRole('combobox'), 'c1');
    await user.type(screen.getByPlaceholderText('e.g., Room 101'), 'Room 101');

    const dateInput = document.querySelector('input[type="datetime-local"]') as HTMLInputElement;
    await user.type(dateInput, '2000-01-01T10:00');

    // Past date keeps Save disabled
    expect(saveButton).toBeDisabled();
    expect(onSubmit).not.toHaveBeenCalled();

    await user.clear(dateInput);
    await user.type(dateInput, '2999-01-01T10:00');

    expect(saveButton).not.toBeDisabled();

    await user.click(screen.getByRole('button', { name: 'Save' }));

    expect(onSubmit).toHaveBeenCalledTimes(1);
    expect(onSubmit).toHaveBeenCalledWith({
      courseId: 'c1',
      dateTime: '2999-01-01T10:00',
      location: 'Room 101',
    });
  });

  it('preloads initialValues and calls onCancel', async () => {
    const user = userEvent.setup();
    const onCancel = vi.fn();

    render(
      <ExamForm
        onSubmit={() => {}}
        onCancel={onCancel}
        courses={[{ id: 'c1', name: 'Math' }]}
        initialValues={{
          courseId: 'c1',
          dateTime: '2999-01-01T10:00',
          location: 'Room 101',
        }}
      />,
    );

    expect(await screen.findByDisplayValue('Room 101')).toBeInTheDocument();
    expect((screen.getByRole('combobox') as HTMLSelectElement).value).toBe('c1');

    await user.click(screen.getByRole('button', { name: 'Cancel' }));
    expect(onCancel).toHaveBeenCalledTimes(1);
  });

  it('disables course select while coursesLoading', () => {
    render(
      <ExamForm
        onSubmit={() => {}}
        onCancel={() => {}}
        coursesLoading
        courses={[{ id: 'c1', name: 'Math' }]}
      />,
    );

    expect(screen.getByRole('combobox')).toBeDisabled();
  });
});
