import { describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { fireEvent } from "@testing-library/react";
import { ExamCard } from "./ExamCard";

const baseExam = {
  id: 123,
  courseName: "Test Course",
  courseCode: "TC101",
  examDate: "2026-02-12T11:00:00Z",
  regDeadline: "2026-02-07T23:59:00Z",
  location: "Room 1",
};

describe("ExamCard", () => {
  it("disables Register and shows validation text when deadline has passed", async () => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2025-12-22T10:00:00Z"));

    const onRegister = vi.fn();

    render(
      <ExamCard
        exam={{ ...baseExam, regDeadline: "2025-01-01T00:00:00Z" }}
        isRegistered={false}
        loading={false}
        onRegister={onRegister}
      />
    );

    const button = screen.getByRole("button", { name: /register/i });
    expect(button).toBeDisabled();
    expect(screen.getByText(/Registration deadline has passed\./i)).toBeInTheDocument();

    fireEvent.click(button);
    expect(onRegister).not.toHaveBeenCalled();
  });

  it("disables the button while loading", () => {
    render(
      <ExamCard
        exam={baseExam}
        isRegistered={false}
        loading
        onRegister={vi.fn()}
      />
    );

    const button = screen.getByRole("button", { name: /registering/i });
    expect(button).toBeDisabled();
  });
});
