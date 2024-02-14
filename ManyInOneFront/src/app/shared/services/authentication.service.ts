import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthResponse } from '../models/auth-response.model';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  // check user is logged in or not , this will availble to whole application 
  // if user reloads then it will be set to inital , as here false 
  // so that time we will fill it from localstorage based on that user id
  isAuthenticatedd: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  currUserSignal = signal<AuthResponse | null | undefined>(undefined);// when not null or not logged in in -> undefined , when not authenticated -> null , when authenticated -> got response , where ther will be user id
  isAuthenticated: boolean = false;

  baseUrl: string = environment.apiBaseUrl;

  registerUrl: string = "auth/Register";
  loginUrl: string = "auth/Login";


  constructor(private http: HttpClient, private cookie: CookieService) {
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
  public registerWithGoogle(credentials: string): Observable<any> {
    const header = new HttpHeaders().set('Content-Type', 'application/json'); // I guess , , don't need this ,
    return this.http.post(`${this.baseUrl}Auth/RegisterWithGoogleLogIn`, JSON.stringify(credentials), { headers: header, withCredentials: true });
  }
  // log in with google
  public logInWithGoogle(credentials: string): Observable<any> {
    const header = new HttpHeaders().set('Content-Type', 'application/json'); // I guess , , don't need this ,
    return this.http.post(`${this.baseUrl}Auth/LogInWithGoogle`, JSON.stringify(credentials), { headers: header, withCredentials: true });
  }

  // signout
  public signOut() : Observable<any>
  {
    return this.http.post(`${this.baseUrl}Auth/SignOut`, { withCredentials: true });
  }

  public refreshToken(): Observable<any> {
    const header = new HttpHeaders();
    return this.http.get(`${this.baseUrl}Auth/GetRefreshToken`, { headers: header, withCredentials: true });
  }

  // this is for when user authentication fails 2 consecutive times
  // 1st time access token expired but 2nd time some how refresh token validity expired
  // sp clearing all cookies and need to log in again
  public revokeToken(): Observable<any> {
    const header = new HttpHeaders();
    return this.http.delete(`${this.baseUrl}Auth/RevokeToken`, {headers : header, withCredentials : true});
  }

  // verify and login with 2fa
  public verifyAndLogin(twoFACode: any): Observable<any> {
    return this.http.post(`${this.baseUrl}Auth/VerifyAndLoginWith2FA`, twoFACode, { withCredentials: true });
  }

  // load and share qr and shared key
  public loadAndShareQR(userId: string): Observable<any> {
    const header = new HttpHeaders().set('Content-Type', 'application/json');
    return this.http.post(`${this.baseUrl}Auth/Get2FAQRCodeeeeeee`, JSON.stringify(userId), { headers: header, withCredentials: true });
  }

  // verify 2 FA code
  public verifyFACode(code: string): Observable<any> {
    const header = new HttpHeaders().set('Content-Type', 'application/json');
    return this.http.post(`${this.baseUrl}Auth/Verify2FA`, JSON.stringify(code), { headers: header, withCredentials: true });
  }

  // disbling authenticator
  public disableAuthenticator(): Observable<any> {
    // const header = new HttpHeaders().set('Content-Type', 'application/json');headers: header,
    return this.http.post(`${this.baseUrl}Auth/Disable2FA`, {  withCredentials: true });
  }

  // delete all data relted to the user
  public deleteAllUserData() : Observable<any>
  {
    // const header = new HttpHeaders().set('Content-Type', 'application/json');headers: header, 
    return this.http.post(`${this.baseUrl}Auth/DeleteUserData`, { withCredentials: true });
  }

  // ================================================Token related =====================================
  // this can be sfifted to another service
  // public updateToken(status: boolean) {
  //   this.isAuthenticated = status;
  //   let ch = this.isAuthenticated;
  //   console.log("from  user status", ch)
  // }


  public saveToken(userId: string) {
    //set that is authenticated behavior 
    this.isAuthenticatedd.next(true);
    // and set the user to localstorage
    localStorage.setItem("curr-app-user", userId);
    // get the user user email or something and set to cookie for ui interaction according to it
    // this.cookie.set("curr-app-user", userId, 1, "/", "localhost", true, "None");
  }

  // when reload happens but already alogged in , that time behavioursubjoct resets , so that time can get 
  // the user from localstorage
  public getUserFromLocal() {
    if (this.isAuthenticatedd.value === false) {
      let fromLocal = localStorage.getItem("curr-app-user");
      if (fromLocal !== null || fromLocal === '') {
        // exists then set again 
        this.isAuthenticatedd.next(true);
      }
    }
    return this.isAuthenticatedd.value;
  }


  public getCurrentUser() : Observable<any>
  {
    return this.http.get(`${this.baseUrl}Auth/GetCurrentUser`, { withCredentials: true });
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
    localStorage.removeItem("curr-app-user");
    this.cookie.deleteAll();
    // window.location.reload();
  }

  // public getToken(): string | null {
  //   // will get current token , else return null
  //   return this.cookie.get("curr-app-user") || null;
  // }
}

// ================================================ Token related Ends=====================================
