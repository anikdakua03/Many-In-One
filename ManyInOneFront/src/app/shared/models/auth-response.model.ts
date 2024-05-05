export class AuthResponse {
    message: string = ""
    result: boolean = true
    userId : string = ""
    userName : string = ""
    errors: string[] = []
    twoFAEnabled : boolean = false
    emailConfirmed : boolean = false
}

export interface IConfirmEmail {
    userId: string;
    confirmationCode: string;
}

export interface IResetPassword {
    email: string;
    code: string;
    newPassword: string;
    confirmPassword: string;
}