import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthResponse } from '../models/auth-response.model';
import { SsrCookieService } from 'ngx-cookie-service-ssr';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  // check user is logged in or not , this will available to whole application 
  // if user reloads then it will be set to initial , as here false 
  // so that time we will fill it from localstorage based on that user id
  isAuthenticated$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.getUserFromLocal());

  baseUrl: string = environment.apiBaseUrl;

  registerUrl: string = "auth/Register";
  loginUrl: string = "auth/Login";


  constructor(private http: HttpClient, private cookie: SsrCookieService) {
  }

  // registration
  public register(user: any): Observable<AuthResponse> {
    return this.http.post<any>(`${this.baseUrl}${this.registerUrl}`, user, { withCredentials: true });
  }

  // login
  public login(user: any): Observable<AuthResponse> {
    return this.http.post<any>(`${this.baseUrl}${this.loginUrl}`, user, { withCredentials: true });//
  }

  // register with google . will give our api credential 
  public registerWithGoogle(credentials: string): Observable<AuthResponse> {
    const header = new HttpHeaders().set('Content-Type', 'application/json'); // I guess , , don't need this ,
    return this.http.post<AuthResponse>(`${this.baseUrl}Auth/RegisterWithGoogleLogIn`, JSON.stringify(credentials), { headers: header, withCredentials: true });
  }
  // log in with google
  public logInWithGoogle(credentials: string): Observable<AuthResponse> {
    const header = new HttpHeaders().set('Content-Type', 'application/json'); // I guess , , don't need this ,
    return this.http.post<AuthResponse>(`${this.baseUrl}Auth/LogInWithGoogle`, JSON.stringify(credentials), { headers: header, withCredentials: true });
  }

  // signout
  public signOut() : Observable<AuthResponse>
  {
    return this.http.post<AuthResponse>(`${this.baseUrl}Auth/SignOut`, { withCredentials: true });
  }

  public refreshToken(): Observable<any> {
    const header = new HttpHeaders();
    return this.http.get(`${this.baseUrl}Auth/GetRefreshToken`, { headers: header, withCredentials: true });
  }

  // this is for when user authentication fails 2 consecutive times
  // 1st time access token expired but 2nd time some how refresh token validity expired
  // sp clearing all cookies and need to log in again
  public revokeToken(): Observable<AuthResponse> {
    const header = new HttpHeaders();
    return this.http.delete<AuthResponse>(`${this.baseUrl}Auth/RevokeToken`, {headers : header, withCredentials : true});
  }

  // verify and login with 2fa
  public verifyAndLogin(twoFALogin: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}Auth/VerifyAndLoginWith2FA`, twoFALogin, { withCredentials: true });
  }

  // load and share qr and shared key
  public loadAndShareQR(userId: string): Observable<any> {
    const header = new HttpHeaders().set('Content-Type', 'application/json');
    return this.http.post<any>(`${this.baseUrl}Auth/Get2FAQRCode`, JSON.stringify(userId), { headers: header, withCredentials: true });
  }

  // verify 2 FA code
  public verifyFACode(code: string): Observable<AuthResponse> {
    const header = new HttpHeaders().set('Content-Type', 'application/json');
    return this.http.post<AuthResponse>(`${this.baseUrl}Auth/Verify2FA`, JSON.stringify(code), { headers: header, withCredentials: true });
  }

  // disabling authenticator
  public disableAuthenticator(): Observable<AuthResponse> {
    // const header = new HttpHeaders().set('Content-Type', 'application/json');headers: header,
    return this.http.post<AuthResponse>(`${this.baseUrl}Auth/Disable2FA`, {  withCredentials: true });
  }

  // delete all data related to the user
  public deleteAllUserData() : Observable<AuthResponse>
  {
    // const header = new HttpHeaders().set('Content-Type', 'application/json');headers: header, 
    return this.http.post<AuthResponse>(`${this.baseUrl}Auth/DeleteUserData`, { withCredentials: true });
  }
  
  // delete all data related to the user
  public GetCurrentUser() : Observable<AuthResponse>
  {
    return this.http.get<AuthResponse>(`${this.baseUrl}Auth/GetCurrentUser`, { withCredentials: true });
  }
  // ================================================Token related =====================================
  // this can be sfifted to another service
  // public updateToken(status: boolean) {
  //   this.isAuthenticated = status;
  //   let ch = this.isAuthenticated;
  //   console.log("from  user status", ch)
  // }


  public saveToken(key : string, value : any) {
    //set that is authenticated behavior 
    this.isAuthenticated$.next(true);
    // not using localstorage because converted this to SSR so uisng ssr cookie
    // and set the user to localstorage
    // localStorage.setItem("curr-app-user", userId);
    this.cookie.set(key, JSON.stringify(value));
    // this.cookie.set("x-user-name", username);
    // get the user user email or something and set to cookie for ui interaction according to it
    // this.cookie.set("curr-app-user", userId, 1, "/", "localhost", true, "None");
  }

  // when reload happens but already a logged in , that time behaviour subject resets , so that time can get 
  // the user from localstorage
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
    // this.authService.saveToken("x-app-user", res.userId);
    // this.authService.saveToken("x-user-name", res.userName);
    // this.authService.saveToken("twofa-enable", res.twoFAEnabled);
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
    return JSON.parse(data) as boolean;
  }

  // public isLoggegIn(): boolean
  // {
  //   this.getCurrentUser().subscribe({
  //     next: res => {
  //       // console.log("After checking froms erver also --> ", res);
  //       this.saveToken(res.userId);
  //       // this.isAuthenticated = check == null || undefined ? false : true;
  //       // console.log("autheee -->", this.isAuthenticated);
  //     },
  //     error: err => {
  //       console.log("Invaliddd -->", err);
  //     }
  //   });

  //   // after matchis from server also
  //   const user = this.getToken();
  //   if(user !== null)
  //   {
  //     this.isAuthenticated = true;
  //     return true;
  //   }
  //   else
  //   {
  //     this.isAuthenticated = false;
  //     return false;
  //   }
  // }

  public removeToken() {
    // localStorage.removeItem("curr-app-user");
    this.cookie.deleteAll();
    // window.location.reload();
  }

  // public getToken(): string | null {
  //   // will get current token , else return null
  //   return this.cookie.get("curr-app-user") || null;
  // }
}

// ================================================ Token related Ends=====================================
