export class AuthResponse {
    message: string = ""
    result: boolean = true
    userId : string = ""
    userName : string = ""
    errors: any = ""
    twoFAEnabled : boolean = false
    emailConfirmed : boolean = false
}