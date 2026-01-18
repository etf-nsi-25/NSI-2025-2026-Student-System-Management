import React, { useEffect, useState } from 'react';
import './2FA.css';

import { CAlert, CBadge, CButton, CCard, CCardBody, CCardHeader, CCol, CForm, CFormInput, CFormLabel, CRow } from '@coreui/react';
import { useNavigate } from 'react-router';
import { useAuthContext } from '../../init/auth.tsx';
import { extractApiErrorMessage } from '../../utils/apiError.ts';
import { verifyTwoFactorLogin } from '../../utils/authUtils.ts';
import { getDashboardRoute } from '../../constants/roles.ts';

const TwoFAVerifyLoginPage: React.FC = () => {
  const navigate = useNavigate();
  const { setAuthInfo } = useAuthContext();

  const [code, setCode] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    const token = sessionStorage.getItem('twoFactorToken');
    if (!token) {
      navigate('/login');
    }
  }, [navigate]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const twoFactorToken = sessionStorage.getItem('twoFactorToken');
    if (!twoFactorToken) {
      navigate('/login');
      return;
    }

    if (!code || code.length !== 6) {
      setError('Enter a 6 digit code.');
      return;
    }

    try {
      setSubmitting(true);
      setError(null);

      const authInfo = await verifyTwoFactorLogin(twoFactorToken, code);
      sessionStorage.removeItem('twoFactorToken');

      setAuthInfo(authInfo);

      const dashboardRoute = getDashboardRoute(authInfo.role);
      navigate(dashboardRoute);
    } catch (err) {
      setError(extractApiErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="twofa-page">
      <CCard className="twofa-card ui-surface-glass-card border-0">
        <CCardHeader className="bg-transparent border-0 pb-0">
          <div className="twofa-header">
            <div className="twofa-badges">
              <CBadge color="primary" shape="rounded-pill" className="ui-badge ui-badge-primary">
                VERIFY 2FA
              </CBadge>
            </div>
            <h1 className="twofa-title">Enter your verification code</h1>
            <p className="twofa-subtitle">
              Enter the 6-digit code from your authenticator app to complete sign-in.
            </p>
          </div>
        </CCardHeader>

        <CCardBody className="pt-4">
          <CRow className="twofa-body g-0 align-items-stretch">
            <CCol md={12} className="twofa-right d-flex">
              <CForm onSubmit={handleSubmit} className="twofa-form w-100">
                <CFormLabel htmlFor="twofa-code" className="twofa-label">
                  6-digit code
                </CFormLabel>

                <CFormInput
                  id="twofa-code"
                  name="twofaCode"
                  type="text"
                  inputMode="numeric"
                  autoComplete="one-time-code"
                  maxLength={6}
                  value={code}
                  onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
                    setCode(e.target.value.replace(/\D/g, ''));
                    setError(null);
                  }}
                  className="ui-input-otp twofa-input text-center"
                  placeholder="●●●●●●"
                />

                {error && (
                  <CAlert color="danger" className="ui-alert ui-alert-error mt-2 twofa-alert">
                    {error}
                  </CAlert>
                )}

                <CButton
                  type="submit"
                  color="primary"
                  disabled={submitting}
                  className="ui-button-cta twofa-button mt-2"
                >
                  {submitting ? 'Verifying…' : 'Verify & Sign in'}
                </CButton>
              </CForm>
            </CCol>
          </CRow>
        </CCardBody>
      </CCard>
    </div>
  );
};

export default TwoFAVerifyLoginPage;
