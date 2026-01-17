import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, within } from "@testing-library/react";

const pushToastMock = vi.hoisted(() => vi.fn());

const getAvailableStudentExamsMock = vi.hoisted(() => vi.fn());
const getRegisteredStudentExamsMock = vi.hoisted(() => vi.fn());
const registerForStudentExamMock = vi.hoisted(() => vi.fn());

const toastContextMock = vi.hoisted(() => ({
  pushToast: pushToastMock,
}));

const apiContextMock = vi.hoisted(() => ({
  getAvailableStudentExams: getAvailableStudentExamsMock,
  getRegisteredStudentExams: getRegisteredStudentExamsMock,
  registerForStudentExam: registerForStudentExamMock,
}));

vi.mock("../../../context/toast", () => ({
  useToast: () => toastContextMock,
}));

vi.mock("../../../context/services", () => ({
  useAPI: () => apiContextMock,
}));

import ExamRegistrationPage from "./ExamRegistrationPage";

async function flushAll() {
  // Some async flows schedule timers + microtasks in multiple waves.
  // Drain a few times to let the handler and React updates settle.
  for (let i = 0; i < 3; i++) {
    await vi.runOnlyPendingTimersAsync();
    await Promise.resolve();
  }
}

function getCardRootByTitle(title: string): HTMLElement {
  const heading = screen.getAllByText(title)[0];
  const root = heading.closest(".card") ?? heading.closest(".ui-surface-glass-card");
  if (!root) {
    throw new Error(`Could not find card root for: ${title}`);
  }
  return root as HTMLElement;
}

describe("ExamRegistrationPage", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    pushToastMock.mockReset();
    getAvailableStudentExamsMock.mockReset();
    getRegisteredStudentExamsMock.mockReset();
    registerForStudentExamMock.mockReset();

    // Default in-memory API behavior (per-test)
    let available: any[] = [
      {
        examId: 1,
        courseId: "00000000-0000-0000-0000-000000000001",
        courseName: "Algorithms and Data Structures",
        courseCode: "ADS101",
        examName: "Algorithms and Data Structures",
        examDate: "2026-02-10T09:00:00Z",
        registrationDeadline: "2026-02-06T23:59:00Z",
      },
      {
        examId: 999,
        courseId: "00000000-0000-0000-0000-000000000999",
        courseName: "MOCK: Already registered (4xx)",
        courseCode: "MOCK400",
        examName: "MOCK: Already registered (4xx)",
        examDate: "2026-02-09T09:00:00Z",
        registrationDeadline: "2026-02-08T23:59:00Z",
      },
      {
        examId: 2,
        courseId: "00000000-0000-0000-0000-000000000002",
        courseName: "Computer Networks",
        courseCode: "CN201",
        examName: "Computer Networks",
        examDate: "2026-02-12T11:00:00Z",
        registrationDeadline: "2026-02-07T23:59:00Z",
      },
      {
        examId: 3,
        courseId: "00000000-0000-0000-0000-000000000003",
        courseName: "Operating Systems",
        courseCode: "OS150",
        examName: "Operating Systems",
        examDate: "2026-02-14T08:00:00Z",
        registrationDeadline: "2026-02-09T23:59:00Z",
      },
    ];

    let registered: any[] = [
      {
        registrationId: 101,
        examId: 105,
        courseId: "00000000-0000-0000-0000-000000000105",
        courseName: "Software Engineering",
        courseCode: "SE310",
        examName: "Software Engineering",
        examDate: "2025-02-20T10:00:00Z",
        registrationDeadline: "2025-02-13T23:59:00Z",
        registrationDate: "2025-01-01T10:00:00Z",
        status: "Registered",
      },
    ];

    getAvailableStudentExamsMock.mockImplementation(async () => available);
    getRegisteredStudentExamsMock.mockImplementation(async () => registered);

    registerForStudentExamMock.mockImplementation(async (examId: number) => {
      const found = available.find((x) => x.examId === examId);
      if (found) {
        available = available.filter((x) => x.examId !== examId);
        registered = [
          ...registered,
          {
            registrationId: 9999,
            examId: found.examId,
            courseId: found.courseId,
            courseName: found.courseName,
            courseCode: found.courseCode,
            examName: found.examName,
            examDate: found.examDate,
            registrationDeadline: found.registrationDeadline,
            registrationDate: new Date().toISOString(),
            status: "Registered",
          },
        ];
      }

      return { registrationId: 9999, message: "OK" };
    });
  });

  it("renders only upcoming eligible exams (filters out past exam dates)", async () => {
    // Make the first mock (2026-02-10) be in the past, while others remain upcoming.
    vi.setSystemTime(new Date("2026-02-11T00:00:00Z"));

    render(<ExamRegistrationPage />);

    await flushAll();

    expect(screen.getByText(/Exam Registration/i)).toBeInTheDocument();

    // Past exam should be filtered out.
    expect(
      screen.queryByText(/Algorithms and Data Structures/i)
    ).not.toBeInTheDocument();

    // Upcoming exam should remain.
    expect(screen.getByText(/Computer Networks/i)).toBeInTheDocument();
  });

  it("registers successfully and moves exam from available to registered", async () => {
    vi.setSystemTime(new Date("2025-12-22T10:00:00Z"));
    render(<ExamRegistrationPage />);

    await flushAll();

    const card = getCardRootByTitle("Computer Networks");
    const registerButton = within(card).getByRole("button", { name: /register/i });

    fireEvent.click(registerButton);

    // Resolve user-event timers + the UX delay (600ms) used in the handler.
    await flushAll();

    expect(screen.getByText(/Successfully registered for the exam\./i)).toBeInTheDocument();

    // Removed from available tab after registration.
    expect(screen.queryByText("Computer Networks")).not.toBeInTheDocument();

    // Shows in registered tab.
    fireEvent.click(screen.getByRole("button", { name: /Registered Exams/i }));
    expect(screen.getByText("Computer Networks")).toBeInTheDocument();

    expect(registerForStudentExamMock).toHaveBeenCalledTimes(1);
    expect(registerForStudentExamMock).toHaveBeenCalledWith(2);
  });

  it("shows a user-facing error toast for 4xx registration errors", async () => {
    vi.setSystemTime(new Date("2026-02-01T10:00:00Z"));

    const err: any = new Error("Already registered");
    err.status = 400;
    registerForStudentExamMock.mockRejectedValueOnce(err);

    render(<ExamRegistrationPage />);

    await flushAll();

    const card = getCardRootByTitle("MOCK: Already registered (4xx)");
    const registerButton = within(card).getByRole("button", { name: /register/i });

    fireEvent.click(registerButton);

    await flushAll();

    expect(pushToastMock).toHaveBeenCalledWith(
      "error",
      "Registration failed",
      "Already registered"
    );

    expect(registerForStudentExamMock).toHaveBeenCalledWith(999);
  });

  it("shows a generic toast for server/network failures", async () => {
    vi.setSystemTime(new Date("2025-12-22T10:00:00Z"));

    const err: any = new Error("Server down");
    err.status = 500;
    registerForStudentExamMock.mockRejectedValueOnce(err);

    render(<ExamRegistrationPage />);

    await flushAll();

    const card = getCardRootByTitle("Computer Networks");
    const registerButton = within(card).getByRole("button", { name: /register/i });

    fireEvent.click(registerButton);

    await flushAll();

    expect(pushToastMock).toHaveBeenCalledWith(
      "error",
      "Something went wrong",
      "Please try again later."
    );
  });

  it("filters available exams by search text", async () => {
    vi.setSystemTime(new Date("2025-12-22T10:00:00Z"));

    render(<ExamRegistrationPage />);

    await flushAll();

    const input = screen.getByPlaceholderText(/Search for a course\.{3}/i);
    fireEvent.change(input, { target: { value: "networks" } });

    expect(screen.getByText("Computer Networks")).toBeInTheDocument();
    expect(screen.queryByText("Operating Systems")).not.toBeInTheDocument();
  });

  it("shows empty state when no available exams exist", async () => {
    // Move time far in the future so all mock exam dates are in the past.
    vi.setSystemTime(new Date("2027-01-01T00:00:00Z"));

    render(<ExamRegistrationPage />);

    await flushAll();

    expect(screen.getByText(/No available exams/i)).toBeInTheDocument();
    expect(
      screen.getByText(/There are currently no exams you can register for\./i)
    ).toBeInTheDocument();
  });

  it("shows a validation toast if deadline passes after render but before click", async () => {
    // Render while deadline is still in the future (button enabled), then advance time.
    vi.setSystemTime(new Date("2026-02-07T00:00:00Z"));
    registerForStudentExamMock.mockResolvedValueOnce(undefined);

    render(<ExamRegistrationPage />);

    await flushAll();

    // Advance past Computer Networks deadline: 2026-02-07T23:59:00Z
    vi.setSystemTime(new Date("2026-02-08T10:00:00Z"));

    const card = getCardRootByTitle("Computer Networks");
    const registerButton = within(card).getByRole("button", { name: /register/i });

    fireEvent.click(registerButton);
    await flushAll();

    expect(pushToastMock).toHaveBeenCalledWith(
      "error",
      "Cannot register",
      "Registration deadline has passed."
    );

    // Should not call API when deadline has passed.
    expect(registerForStudentExamMock).not.toHaveBeenCalled();
  });

});
