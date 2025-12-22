import type { PropsWithChildren, ReactElement } from "react";
import { createContext, useCallback, useContext, useMemo, useState } from "react";
import { CToast, CToastBody, CToastHeader, CToaster } from "@coreui/react";
import type { CToastProps } from "@coreui/react/dist/esm/components/toast/CToast";

export type ToastType = "success" | "error";

export type ToastMessage = {
  id: number;
  type: ToastType;
  title: string;
  message: string;
};

type ToastContextValue = {
  pushToast: (type: ToastType, title: string, message: string, timeoutMs?: number) => void;
};

const ToastContext = createContext<ToastContextValue>({} as ToastContextValue);

export function useToast() {
  return useContext(ToastContext);
}

export function ToastProvider({ children }: PropsWithChildren) {
  const [toast, setToast] = useState<ReactElement<CToastProps> | undefined>(undefined);

  const pushToast = useCallback(
    (type: ToastType, title: string, message: string, timeoutMs = 4500) => {
      setToast(
        <CToast
          key={`${Date.now()}-${Math.random().toString(16).slice(2)}`}
          className={`ui-toast ui-toast-${type}`}
          autohide
          delay={timeoutMs}
          animation
        >
          <CToastHeader closeButton>{title}</CToastHeader>
          <CToastBody>{message}</CToastBody>
        </CToast>
      );
    },
    []
  );

  const value = useMemo(() => ({ pushToast }), [pushToast]);

  return (
    <ToastContext.Provider value={value}>
      {children}
      <CToaster placement="top-end" push={toast} />
    </ToastContext.Provider>
  );
}
