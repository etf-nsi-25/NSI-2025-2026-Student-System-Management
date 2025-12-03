/**
 * Normalizes different error formats coming from RestClient or thrown manually.
 * This ensures every page gets a clean string message instead of raw JSON or Promise content.
 */
export function extractApiErrorMessage(error: unknown): string {
    if (!error) return "Unexpected error.";
  
    if (typeof error === "string") return error;
  
    // RestClient throws objects with { message: Promise<string>, status: number }
    const anyErr = error as { message?: unknown };
  
    if (anyErr.message instanceof Promise) return "Unexpected server error."; 
  
    if (typeof anyErr.message === "string") {
      const raw = anyErr.message.trim();
      if (raw.startsWith("{")) {
        try {
          const parsed = JSON.parse(raw);
  
          if (
            parsed &&
            typeof parsed === "object" &&
            "message" in parsed &&
            typeof parsed.message === "string"
          ) {
            return parsed.message;
          }
        } catch {
          /* swallow JSON parse errors */
        }
      }
  
      return raw || "Unexpected error.";
    }
  
    return "Unexpected error.";
  }
  