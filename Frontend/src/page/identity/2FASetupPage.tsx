import React, { useEffect, useState } from "react";
import "./2FA.css";

import { useAPI } from "../../context/services.tsx";
import type { TwoFASetupResponse } from "../../models/2fa/TwoFA.types";
import { extractApiErrorMessage } from "../../utils/apiError.ts";

// CoreUI
import {
  CCard,
  CCardBody,
  CCardHeader,
  CRow,
  CCol,
  CForm,
  CFormLabel,
  CFormInput,
  CAlert,
  CBadge,
  CButton,
  CSpinner,
} from "@coreui/react";

const TwoFASetupPage: React.FC = () => {
  const api = useAPI();

  const [data, setData] = useState<TwoFASetupResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [code, setCode] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    const fetchSetupData = async () => {
      try {
        setLoading(true);
        setError(null);
        setSuccess(false);

        const json = await api.enableTwoFactor();
        setData(json);
      } catch (err) {
        setError(extractApiErrorMessage(err));
      } finally {
        setLoading(false);
      }
    };

    fetchSetupData();
  }, [api]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!code || code.length !== 6) {
      setError("Enter a 6 digit code.");
      setSuccess(false);
      return;
    }

    try {
      setSubmitting(true);
      setError(null);
      setSuccess(false);

      const json = await api.verifyTwoFactorSetup(code);

      if (!json.success) {
        throw new Error(
          json.message || "Invalid or expired code. Please try again."
        );
      }

      setSuccess(true);
    } catch (err) {
      setError(extractApiErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  // -------- LOADING STATE --------
  if (loading) {
    return (
      <div className="twofa-page">
        <CCard className="twofa-card ui-surface-glass-card border-0">
          <CCardBody className="d-flex justify-content-center align-items-center">
            <CSpinner size="sm" className="me-2" />
            <span className="twofa-loading">Loading 2FA setup…</span>
          </CCardBody>
        </CCard>
      </div>
    );
  }

  // -------- ERROR STATE (NO DATA) --------
  if (error && !data) {
    return (
      <div className="twofa-page">
        <CCard className="twofa-card ui-surface-glass-card border-0">
          <CCardBody>
            <CAlert color="danger" className="mb-0">
              {error}
            </CAlert>
          </CCardBody>
        </CCard>
      </div>
    );
  }

  // -------- MAIN CONTENT --------
  return (
    <div className="twofa-page">
      <CCard className="twofa-card ui-surface-glass-card border-0">
        <CCardHeader className="bg-transparent border-0 pb-0">
          <div className="twofa-header">
            <div className="twofa-badges">
              <CBadge
                color="secondary"
                shape="rounded-pill"
                className="ui-badge ui-badge-soft"
              >
                Security
              </CBadge>
              <CBadge
                color="primary"
                shape="rounded-pill"
                className="ui-badge ui-badge-primary"
              >
                STEP 1 · ENABLE 2FA
              </CBadge>
            </div>
            <h1 className="twofa-title">Protect your account</h1>
            <p className="twofa-subtitle">
              Scan the QR code with your authenticator app or use the manual key.
              Then enter the 6-digit code to finish setup.
            </p>
          </div>
        </CCardHeader>

        {data && (
          <CCardBody className="pt-4">
            <CRow className="twofa-body g-0 align-items-stretch">
              {/* LEFT SIDE – QR + MANUAL KEY */}
              <CCol md={5} className="twofa-left d-flex flex-column">
                {data.qrCodeImageBase64 && (
                  <div className="twofa-qr-wrapper">
                    <img
                      src={data.qrCodeImageBase64}
                      alt="2FA QR Code"
                      className="twofa-qr"
                    />
                  </div>
                )}

                <div className="twofa-manual">
                  <div className="twofa-manual-label">Manual key</div>
                  <div className="twofa-manual-value">{data.manualKey}</div>
                  <div className="twofa-manual-hint">
                    Use this only if you can&apos;t scan the QR code.
                  </div>
                </div>
              </CCol>

              {/* RIGHT SIDE – FORM */}
              <CCol md={7} className="twofa-right d-flex">
                <CForm onSubmit={handleSubmit} className="twofa-form w-100">
                  <CFormLabel className="twofa-label">
                    6-digit code from your authenticator app
                  </CFormLabel>

                  <CFormInput
                    type="text"
                    inputMode="numeric"
                    maxLength={6}
                    value={code}
                    onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
                      setCode(e.target.value.replace(/\D/g, ""));
                      setError(null);
                      setSuccess(false);
                    }}
                    className="ui-input-otp twofa-input text-center"
                    placeholder="●●●●●●"
                  />

                  {error && (
                    <CAlert
                      color="danger"
                      className="ui-alert ui-alert-error mt-2 twofa-alert"
                    >
                      {error}
                    </CAlert>
                  )}

                  {success && (
                    <CAlert
                      color="success"
                      className="ui-alert ui-alert-success mt-2 twofa-alert"
                    >
                      2FA is successfully enabled on your account.
                    </CAlert>
                  )}

                  <CButton
                    type="submit"
                    color="primary"
                    disabled={submitting}
                    className="ui-button-cta twofa-button mt-2"
                  >
                    {submitting ? "Verifying…" : "Confirm & Enable 2FA"}
                  </CButton>

                  <div className="twofa-footer-text mt-2">
                    Make sure you keep access to your authenticator app. You&apos;ll
                    need these codes every time you sign in.
                  </div>
                </CForm>
              </CCol>
            </CRow>
          </CCardBody>
        )}
      </CCard>
    </div>
  );
};

export default TwoFASetupPage;
