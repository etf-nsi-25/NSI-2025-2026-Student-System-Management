import { beforeEach, describe, expect, it, vi } from 'vitest';

const fetchMock = vi.fn();
vi.stubGlobal('fetch', fetchMock);

const examsApi = await import('../examsApi');

beforeEach(() => {
  fetchMock.mockReset();
});

function mockOkJson(data: unknown) {
  fetchMock.mockResolvedValueOnce({
    ok: true,
    status: 200,
    text: async () => JSON.stringify(data),
  });
}

function mockOkEmpty() {
  fetchMock.mockResolvedValueOnce({
    ok: true,
    status: 204,
    text: async () => '',
  });
}

describe('examsApi service', () => {
  it('fetchExams calls GET /api/exams', async () => {
    mockOkJson([{ id: 'e1', courseName: 'Math', dateTime: 'x', location: 'y' }]);

    const result = await examsApi.fetchExams();

    expect(fetchMock).toHaveBeenCalledWith(
      '/api/exams',
      expect.objectContaining({
        headers: expect.objectContaining({
          Accept: 'application/json',
          'Content-Type': 'application/json',
        }),
      }),
    );
    expect(result).toHaveLength(1);
  });

  it('getExam calls GET /api/exams/:id', async () => {
    mockOkJson({ id: 'e1', courseName: 'Math', dateTime: 'x', location: 'y' });

    await examsApi.getExam('e1');

    expect(fetchMock).toHaveBeenCalledWith(
      '/api/exams/e1',
      expect.objectContaining({
        headers: expect.objectContaining({
          Accept: 'application/json',
          'Content-Type': 'application/json',
        }),
      }),
    );
  });

  it('createExam calls POST /api/exams', async () => {
    mockOkEmpty();

    const payload = { courseName: 'Math', dateTime: '2999-01-01T10:00', location: 'Room 101' };
    await examsApi.createExam(payload);

    expect(fetchMock).toHaveBeenCalledWith(
      '/api/exams',
      expect.objectContaining({
        method: 'POST',
        body: JSON.stringify(payload),
      }),
    );
  });

  it('updateExam calls PUT /api/exams/:id', async () => {
    mockOkEmpty();

    const payload = { courseName: 'Math', dateTime: '2999-01-01T10:00', location: 'Room 202' };
    await examsApi.updateExam('e1', payload);

    expect(fetchMock).toHaveBeenCalledWith(
      '/api/exams/e1',
      expect.objectContaining({
        method: 'PUT',
        body: JSON.stringify(payload),
      }),
    );
  });

  it('deleteExam calls DELETE /api/exams/:id', async () => {
    mockOkEmpty();

    await examsApi.deleteExam('e1');

    expect(fetchMock).toHaveBeenCalledWith(
      '/api/exams/e1',
      expect.objectContaining({ method: 'DELETE' }),
    );
  });
});
