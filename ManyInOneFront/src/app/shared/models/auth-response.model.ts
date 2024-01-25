export class AuthResponse {
    message: string = ""
    result: boolean = true
    userId : string = ""
    errors: any = ""
    twoFAEnabled : boolean = false
    emailConfirmed : boolean = false
}