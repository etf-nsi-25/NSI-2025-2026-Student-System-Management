import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, within } from "@testing-library/react";

const pushToastMock = vi.hoisted(() => vi.fn());

const registerForExamMock = vi.hoisted(() => vi.fn());
const unregisterExamMock = vi.hoisted(() => vi.fn());

vi.mock("../../../context/toast", () => ({
  useToast: () => ({
    pushToast: pushToastMock,
  }),
}));

vi.mock("../../../service/examService", () => ({
  examService: {
    getEligibleExams: vi.fn(),
    getRegisteredExams: vi.fn(),
    registerForExam: registerForExamMock,
    unregisterExam: unregisterExamMock,
  },
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
    registerForExamMock.mockReset();
    unregisterExamMock.mockReset();
  });

  it("renders only upcoming eligible exams (filters out past exam dates)", () => {
    // Make the first mock (2026-02-10) be in the past, while others remain upcoming.
    vi.setSystemTime(new Date("2026-02-11T00:00:00Z"));

    render(<ExamRegistrationPage />);

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
    registerForExamMock.mockResolvedValueOnce(undefined);

    render(<ExamRegistrationPage />);

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

    expect(registerForExamMock).toHaveBeenCalledTimes(1);
    expect(registerForExamMock).toHaveBeenCalledWith(2);
  });

  it("shows a user-facing error toast for 4xx registration errors", async () => {
    vi.setSystemTime(new Date("2026-02-01T10:00:00Z"));

    const err: any = new Error("Already registered");
    err.status = 400;
    registerForExamMock.mockRejectedValueOnce(err);

    render(<ExamRegistrationPage />);

    const card = getCardRootByTitle("MOCK: Already registered (4xx)");
    const registerButton = within(card).getByRole("button", { name: /register/i });

    fireEvent.click(registerButton);

    await flushAll();

    expect(pushToastMock).toHaveBeenCalledWith(
      "error",
      "Registration failed",
      "Already registered"
    );

    expect(registerForExamMock).toHaveBeenCalledWith(999);
  });

  it("shows a generic toast for server/network failures", async () => {
    vi.setSystemTime(new Date("2025-12-22T10:00:00Z"));

    const err: any = new Error("Server down");
    err.status = 500;
    registerForExamMock.mockRejectedValueOnce(err);

    render(<ExamRegistrationPage />);

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

  it("filters available exams by search text", () => {
    vi.setSystemTime(new Date("2025-12-22T10:00:00Z"));

    render(<ExamRegistrationPage />);

    const input = screen.getByPlaceholderText(/Search for a course\.{3}/i);
    fireEvent.change(input, { target: { value: "networks" } });

    expect(screen.getByText("Computer Networks")).toBeInTheDocument();
    expect(screen.queryByText("Operating Systems")).not.toBeInTheDocument();
  });

  it("shows empty state when no available exams exist", () => {
    // Move time far in the future so all mock exam dates are in the past.
    vi.setSystemTime(new Date("2027-01-01T00:00:00Z"));

    render(<ExamRegistrationPage />);

    expect(screen.getByText(/No available exams/i)).toBeInTheDocument();
    expect(
      screen.getByText(/There are currently no exams you can register for\./i)
    ).toBeInTheDocument();
  });

  it("shows a validation toast if deadline passes after render but before click", async () => {
    // Render while deadline is still in the future (button enabled), then advance time.
    vi.setSystemTime(new Date("2026-02-07T00:00:00Z"));
    registerForExamMock.mockResolvedValueOnce(undefined);

    render(<ExamRegistrationPage />);

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
    expect(registerForExamMock).not.toHaveBeenCalled();
  });

  it("unregisters successfully and (if before deadline) moves exam back to available", async () => {
    // Before deadlines in mockRegistered (Feb 2025), so it will be re-added to available.
    vi.setSystemTime(new Date("2025-01-10T10:00:00Z"));
    unregisterExamMock.mockResolvedValueOnce(undefined);

    render(<ExamRegistrationPage />);

    // Go to registered tab.
    fireEvent.click(screen.getByRole("button", { name: /Registered Exams/i }));

    const card = getCardRootByTitle("Software Engineering");
    const unregisterButton = within(card).getByRole("button", { name: /unregister/i });

    fireEvent.click(unregisterButton);
    await flushAll();

    expect(screen.getByText(/Successfully unregistered from the exam\./i)).toBeInTheDocument();
    expect(unregisterExamMock).toHaveBeenCalledWith(101);

    // Now verify it appears back in Available tab.
    fireEvent.click(screen.getByRole("button", { name: /Available Exams/i }));
    expect(screen.getAllByText("Software Engineering")[0]).toBeInTheDocument();
  });

  it("shows a user-facing error toast for 4xx unregister errors", async () => {
    vi.setSystemTime(new Date("2025-01-10T10:00:00Z"));

    const err: any = new Error("Cannot unregister");
    err.status = 400;
    unregisterExamMock.mockRejectedValueOnce(err);

    render(<ExamRegistrationPage />);

    fireEvent.click(screen.getByRole("button", { name: /Registered Exams/i }));

    const card = getCardRootByTitle("Software Engineering");
    const unregisterButton = within(card).getByRole("button", { name: /unregister/i });

    fireEvent.click(unregisterButton);
    await flushAll();

    expect(pushToastMock).toHaveBeenCalledWith(
      "error",
      "Unregister failed",
      "Cannot unregister"
    );
  });
});
