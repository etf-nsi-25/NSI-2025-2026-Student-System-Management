export const validateEmail = (email: string) => {
    if (!email) {
      return 'Email is required';
    }
    // Regex for email validation matching HTML5 email input behavior
    const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!pattern.test(email)) {
      return 'Invalid email format';
    }
    if (email.length > 256) {
      return 'Email must not exceed 256 characters';
    }
    return null;
};

export const validatePassword = (pwd: string) => {
    if (!pwd) {
      return 'Password is required';
    }
    if (pwd.length < 8 || pwd.length > 100) {
      return 'Password must be 8-100 characters';
    }
    // Pattern: must contain lowercase, uppercase, digit, and special character
    const pattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$/;
    if (!pattern.test(pwd)) {
      return 'Password must contain uppercase, lowercase, number, and special character';
    }
    return null;
};