export type TwoFASetupResponse = {
    qrCodeImageBase64: string;
    manualKey: string;
};

export type TwoFAConfirmResponse = {
    success: boolean;
    message?: string;
};
