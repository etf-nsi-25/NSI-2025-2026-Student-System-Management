import { useState } from "react";
import {
  CForm,
  CFormLabel,
  CFormInput,
  CButton,
  CInputGroup,
  CInputGroupText,
  CAlert,
} from "@coreui/react";
import "./login.css";
import logoImage from "../../assets/images/login/unsa-sms-logo.png";
import { useAuthContext } from "../../init/auth.tsx";
import { useNavigate } from "react-router";
import { extractApiErrorMessage } from "../../utils/apiError.ts";
import { validateEmail, validatePassword } from "./loginUtils.ts";
import { getDashboardRoute } from "../../constants/roles.ts";
import { loginWithCredentials } from "../../utils/authUtils.ts";

export function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { setAuthInfo } = useAuthContext();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const emailError = validateEmail(email);
    const passwordError = validatePassword(password);
    
    if (emailError || passwordError) {
      setError(emailError ?? passwordError);
      return;
    }

    setError(null);

    try {

      const result = await loginWithCredentials(email, password);

      if (result.kind === '2fa') {
        sessionStorage.setItem('twoFactorToken', result.twoFactorToken);
        navigate('/2fa/verify');
        return;
      }

      setAuthInfo(result.authInfo);
      
      if (result.authInfo.forcePasswordChange) {
        navigate("/profile-settings", { replace: true });
        return;
      }

      const dashboardRoute = getDashboardRoute(result.authInfo.role);
      navigate(dashboardRoute);
    } catch (error) {
      setError(extractApiErrorMessage(error));
    }
  };

  return (
    <div className="login-container">
      <div className="background-decoration background-decoration-top-left"></div>
      <div className="background-decoration background-decoration-bottom-left"></div>
      <div className="background-decoration background-decoration-bottom-right"></div>
      <div className="login-card-wrapper">
        <div className="card-decoration decoration-card-bottom"></div>
        <div className="card-decoration decoration-card-top"></div>
        <div className="card-decoration decoration-card-right"></div>

        <div className="login-card">
          <img className="logo" src={logoImage} alt="UNSA SMS Logo" />

          <CForm className="login-form" onSubmit={handleSubmit}>
            <h1 className="login-title ui-heading-lg">Login</h1>
            <div className="form-content">
              <div className="ui-form-field">
                <CFormLabel htmlFor="email" className="form-label ui-field-label">
                  Email
                </CFormLabel>
                <CFormInput
                  id="email"
                  type="email"
                  className="form-input ui-input-base"
                  placeholder="username@faculty.unsa.ba"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
              </div>

              <div className="ui-form-field">
                <CFormLabel htmlFor="password" className="ui-field-label">
                  Password
                </CFormLabel>
                <CInputGroup className="password-input-wrapper">
                  <CFormInput
                    id="password"
                    type={showPassword ? "text" : "password"}
                    className="form-input ui-input-base"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                  />
                  <CInputGroupText className="password-toggle-wrapper">
                    <button
                      type="button"
                      className="password-toggle"
                      onClick={() => setShowPassword(!showPassword)}
                      aria-label={
                        showPassword ? "Hide password" : "Show password"
                      }
                    >
                      <svg
                        width="60"
                        height="60"
                        viewBox="3 3 30 30"
                        fill="none"
                        xmlns="http://www.w3.org/2000/svg"
                      >
                        <path
                          d="M23.8947 19.3515C24.1671 18.6235 24.3053 17.8522 24.3026 17.0749C24.3026 15.349 23.6169 13.6938 22.3966 12.4734C21.1762 11.253 19.521 10.5674 17.7951 10.5674C17.0274 10.5683 16.266 10.7064 15.5469 10.9753L17.0741 12.55C17.3063 12.5128 17.541 12.4938 17.7761 12.4931C18.9964 12.4905 20.1682 12.9709 21.0355 13.8294C21.9029 14.6878 22.3953 15.8546 22.4053 17.0749C22.4046 17.31 22.3855 17.5447 22.3484 17.7768L23.8947 19.3515Z"
                          fill="#C7D2D6"
                        />
                        <path
                          d="M32.5285 16.6287C29.3317 10.7188 23.7254 7.14258 17.5215 7.14258C15.8324 7.14654 14.1546 7.41871 12.5508 7.9489L14.078 9.48565C15.203 9.19493 16.3596 9.04516 17.5215 9.0398C22.8622 9.0398 27.7191 12.0184 30.6123 17.0366C29.551 18.8983 28.144 20.5404 26.4669 21.8745L27.8139 23.2215C29.7549 21.6557 31.3668 19.7213 32.557 17.5299L32.8036 17.0745L32.5285 16.6287Z"
                          fill="#C7D2D6"
                        />
                        <path
                          d="M4.61985 5.48277L8.85066 9.71357C6.17645 11.4354 3.99083 13.817 2.50445 16.6289L2.25781 17.0748L2.50445 17.5301C5.70127 23.44 11.3076 27.0162 17.5115 27.0162C19.933 27.0157 22.323 26.4677 24.5027 25.4131L29.2458 30.1561L30.9059 28.7332L6.24198 4.06934L4.61985 5.48277ZM13.8688 14.7317L20.1771 21.04C19.4643 21.4812 18.6438 21.7175 17.8055 21.723C17.1965 21.723 16.5934 21.6026 16.0311 21.3686C15.4687 21.1347 14.9582 20.7919 14.5288 20.3598C14.0994 19.9278 13.7597 19.4152 13.5293 18.8514C13.2988 18.2876 13.1821 17.6839 13.1858 17.0748C13.1965 16.2462 13.4326 15.4362 13.8688 14.7317ZM12.4933 13.3562C11.6058 14.6078 11.1899 16.1331 11.3191 17.662C11.4482 19.1908 12.1143 20.6247 13.1992 21.7096C14.2841 22.7945 15.718 23.4606 17.2468 23.5897C18.7757 23.7189 20.301 23.303 21.5526 22.4155L23.0703 23.9332C21.3133 24.6843 19.4223 25.0715 17.5115 25.0716C12.1708 25.0716 7.31391 22.0929 4.42065 17.0748C5.80913 14.6162 7.80882 12.5577 10.2261 11.0985L12.4933 13.3562Z"
                          fill="#C7D2D6"
                        />
                      </svg>
                    </button>
                  </CInputGroupText>
                </CInputGroup>
              </div>

              {error && (
                <CAlert color="danger" className="ui-alert ui-alert-error">
                  {error}
                </CAlert>
              )}

              <a href="#" className="forgot-password ui-field-label">
                Forgot Password?
              </a>
            </div>

            <CButton type="submit" className="submit-button ui-button-cta" color="primary">
              Sign in
            </CButton>
          </CForm>
        </div>
      </div>
    </div>
  );
}
