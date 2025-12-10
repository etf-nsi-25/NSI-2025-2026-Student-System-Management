import { useState, useEffect } from 'react';
import { CToast, CToastBody, CToaster } from '@coreui/react';

interface ToastItem {
  id: number;
  message: string;
  type: 'success' | 'error';
}

let toastCounter = 0;

interface ToastManagerProps {
  toast: { message: string; type: 'success' | 'error'; visible?: boolean } | null;
}

export default function ToastManager({ toast }: ToastManagerProps) {
  const [toasts, setToasts] = useState<ToastItem[]>([]);

  useEffect(() => {
    if (!toast || toast.visible === false) return;

    const id = toastCounter++;
    setToasts(prev => [...prev, { id, message: toast.message, type: toast.type }]);
    return;
  }, [toast]);

  return (
    <CToaster>
      {toasts.map(t => (
        <CToast
          key={t.id}
          autohide={true}
          delay={2000}
          visible={true}
          onClose={() => setToasts(prev => prev.filter(x => x.id !== t.id))}
          className={t.type === 'success' ? 'bg-success text-white' : 'bg-danger text-white'}
        >
          <CToastBody>{t.message}</CToastBody>
        </CToast>
      ))}
    </CToaster>
  );
}
