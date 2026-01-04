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
    return null;
};