import { describe, expect, it } from "vitest";
import { extractApiErrorMessage } from "./apiError";

describe("extractApiErrorMessage", () => {
  it("returns default message for nullish errors", () => {
    expect(extractApiErrorMessage(null)).toBe("Unexpected error.");
    expect(extractApiErrorMessage(undefined)).toBe("Unexpected error.");
  });

  it("returns the string error as-is", () => {
    expect(extractApiErrorMessage("Boom")).toBe("Boom");
  });

  it("extracts message from JSON {message}", () => {
    const err = { message: '{"message":"Invalid input"}' };
    expect(extractApiErrorMessage(err)).toBe("Invalid input");
  });

  it("extracts message from JSON {error}", () => {
    const err = { message: '{"error":"Already registered"}' };
    expect(extractApiErrorMessage(err)).toBe("Already registered");
  });

  it("falls back to raw message when JSON parse fails", () => {
    const err = { message: "{not json" };
    expect(extractApiErrorMessage(err)).toBe("{not json");
  });

  it("hides Promise message content", () => {
    const err = { message: Promise.resolve("details") };
    expect(extractApiErrorMessage(err)).toBe("Unexpected server error.");
  });
});
