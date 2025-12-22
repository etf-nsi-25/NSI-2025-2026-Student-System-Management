import { describe, expect, it, vi } from 'vitest';

vi.mock('../../api/rest', () => {
  return {
    api: {
      get: vi.fn(),
      post: vi.fn(),
      put: vi.fn(),
      delete: vi.fn(),
    },
  };
});

const { api } = await import('../../api/rest');
const examsApi = await import('../examsApi');

describe('examsApi service', () => {
  it('fetchExams calls GET /api/exams', async () => {
    (api.get as any).mockResolvedValue([{ id: 'e1', courseName: 'Math', dateTime: 'x', location: 'y' }]);

    const result = await examsApi.fetchExams();

    expect(api.get).toHaveBeenCalledWith('/api/exams');
    expect(result).toHaveLength(1);
  });

  it('getExam calls GET /api/exams/:id', async () => {
    (api.get as any).mockResolvedValue({ id: 'e1', courseName: 'Math', dateTime: 'x', location: 'y' });

    await examsApi.getExam('e1');

    expect(api.get).toHaveBeenCalledWith('/api/exams/e1');
  });

  it('createExam calls POST /api/exams', async () => {
    (api.post as any).mockResolvedValue(undefined);

    const payload = { courseName: 'Math', dateTime: '2999-01-01T10:00', location: 'Room 101' };
    await examsApi.createExam(payload);

    expect(api.post).toHaveBeenCalledWith('/api/exams', payload);
  });

  it('updateExam calls PUT /api/exams/:id', async () => {
    (api.put as any).mockResolvedValue(undefined);

    const payload = { courseName: 'Math', dateTime: '2999-01-01T10:00', location: 'Room 202' };
    await examsApi.updateExam('e1', payload);

    expect(api.put).toHaveBeenCalledWith('/api/exams/e1', payload);
  });

  it('deleteExam calls DELETE /api/exams/:id', async () => {
    (api.delete as any).mockResolvedValue(undefined);

    await examsApi.deleteExam('e1');

    expect(api.delete).toHaveBeenCalledWith('/api/exams/e1');
  });
});
