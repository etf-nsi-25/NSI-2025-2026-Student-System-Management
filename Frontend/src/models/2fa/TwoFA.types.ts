export type TwoFASetupResponse = {
    qrCodeBase64: string;
    manualEntryKey: string;
    otpAuthUri: string;
    message?: string;
};

export type TwoFAConfirmResponse = {
    success: boolean;
    message?: string;
    recoveryCodes?: string[];
};
