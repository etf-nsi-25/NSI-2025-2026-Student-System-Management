import React from 'react';
import { CModal, CModalBody } from '@coreui/react';

interface ModalProps {
  onClose: () => void;
  children: React.ReactNode;
}

export default function Modal({ children, onClose }: ModalProps) {
  return (
    <CModal visible onClose={onClose} alignment="center" backdrop={false}>
      <CModalBody>{children}</CModalBody>
    </CModal>
  );
}
