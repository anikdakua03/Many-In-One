import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthResponse, IConfirmEmail, IResetPassword } from '../models/auth-response.model';
import { SsrCookieService } from 'ngx-cookie-service-ssr';
import { ApiResponse } from '../interfaces/api-response';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  // check user is logged in or not , this will available to whole application 
  // if user reloads then it will be set to initial , as here false 
  // so that time we will fill it from localstorage based on that user id
  isAuthenticated$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.getUserFromLocal());
  userName$: BehaviorSubject<string> = new BehaviorSubject<string>(this.getCurrentUserName());

  baseUrl: string = environment.apiBaseUrl;

  registerUrl: string = "auth/Register";
  loginUrl: string = "auth/Login";


  constructor(private http: HttpClient, private cookie: SsrCookieService) {
  }

  // registration
  public register(user: any): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}${this.registerUrl}`, user, { withCredentials: true });
  }

  // confirm email
  public confirmEmail(confirmEmailBody: IConfirmEmail): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/ConfirmEmail`, confirmEmailBody, { withCredentials: true });
  }

  // resend confirmation mail
  public resendConfirmationMail(userId: string): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/ResendConfirmationEmail/${userId}`, {}, { withCredentials: true });
  }

  // forgot password mail
  public forgotPasswordMail(userMail: string): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/ForgotPasswordEmail/${userMail}`, {}, { withCredentials: true });
  }

  // reset password 
  public resetPassword(resetPasswordBody: IResetPassword): Observable<ApiResponse<AuthResponse>> {
    return this.http.put<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/ResetPassword`, resetPasswordBody, { withCredentials: true });
  }

  // login
  public login(user: any): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}${this.loginUrl}`, user, { withCredentials: true });
  }

  // register with google . will give our api credential 
  public registerWithGoogle(credentials: string): Observable<ApiResponse<AuthResponse>> {
    const header = new HttpHeaders().set('Content-Type', 'application/json'); // I guess , , don't need this ,
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/RegisterWithGoogleLogIn`, JSON.stringify(credentials), { headers: header, withCredentials: true });
  }
  // log in with google
  public logInWithGoogle(credentials: string): Observable<ApiResponse<AuthResponse>> {
    const header = new HttpHeaders().set('Content-Type', 'application/json'); // I guess , , don't need this ,
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/LogInWithGoogle`, JSON.stringify(credentials), { headers: header, withCredentials: true });
  }

  // signout
  public signOut() : Observable<ApiResponse<AuthResponse>>
  {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/SignOut`, {}, { withCredentials: true });
  }

  public refreshToken(): Observable<any> {
    const header = new HttpHeaders();
    return this.http.get(`${this.baseUrl}Auth/GetRefreshToken`, { headers: header, withCredentials: true });
  }

  // this is for when user authentication fails 2 consecutive times
  // 1st time access token expired but 2nd time some how refresh token validity expired
  // sp clearing all cookies and need to log in again
  public revokeToken(): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/RevokeToken`, {}, { withCredentials: true });
  }

  // verify and login with 2fa
  public verifyAndLogin(twoFALogin: any): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/VerifyAndLoginWith2FA`, twoFALogin, { withCredentials: true });
  }

  // load and share qr and shared key
  public loadAndShareQR(userId: string): Observable<ApiResponse<any>> {
    const header = new HttpHeaders().set('Content-Type', 'application/json');
    return this.http.post<ApiResponse<any>>(`${this.baseUrl}Auth/Get2FAQRCode`, JSON.stringify(userId), { headers: header, withCredentials: true });
  }

  // verify 2 FA code
  public verifyFACode(code: string): Observable<ApiResponse<AuthResponse>> {
    const header = new HttpHeaders().set('Content-Type', 'application/json');
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/Verify2FA`, JSON.stringify(code), { headers: header, withCredentials: true });
  }

  // disabling authenticator
  public disableAuthenticator(): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/Disable2FA`, {}, { withCredentials: true });
  }

  // delete all data related to the user
  public deleteAllUserData() : Observable<ApiResponse<AuthResponse>>
  {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/DeleteUserData`, {}, { withCredentials: true });
  }
  
  // delete all data related to the user
  public GetCurrentUser() : Observable<ApiResponse<AuthResponse>>
  {
    return this.http.get<ApiResponse<AuthResponse>>(`${this.baseUrl}Auth/GetCurrentUser`, { withCredentials: true });
  }

  // ================================================Token related =====================================

  public saveToken(key : string, value : any) {
    //set that is authenticated behavior 
    this.isAuthenticated$.next(true);
    this.cookie.set(key, JSON.stringify(value));
  }

  // when reload happens but already a logged in , that time behaviour subject resets , so that time can get 
  // the user from cookie
  public getUserFromLocal(): any {
    
    let fromCookie = this.cookie.get("x-app-user");
    if (fromCookie !== null || fromCookie !== '') {
        return fromCookie;
      }
    return false;
  }

  // gets user name from local
  public getCurrentUserName() : string
  {
    let fromCookie = this.cookie.get("x-user-name");
    if (fromCookie !== null || fromCookie !== '' || fromCookie !== undefined) {
      return fromCookie;
    }
    return "";
  }
  
  // checks two factor status from local
  public CheckUser2FA() : boolean
  {
    const data = this.cookie.get("twofa-enable");
    return (data === null || data === undefined) ? false : JSON.parse(data) as boolean;
  }

  public removeToken() {
    this.cookie.delete("twofa-enable");
    this.cookie.deleteAll();
  }

}