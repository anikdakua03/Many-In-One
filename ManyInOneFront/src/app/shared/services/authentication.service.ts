import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthResponse } from '../models/auth-response.model';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  // check user is logged in or not
  // isAuthenticated : BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isAuthenticated: boolean = false;

  baseUrl: string = environment.apiBaseUrl;

  registerUrl: string = "auth/Register";
  loginUrl: string = "auth/Login";


  constructor(private http: HttpClient, private cookie : CookieService, private route : Router) {
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
  signOut() : Observable<any>
  {
    return this.http.post(`${this.baseUrl}Auth/SignOut`, { withCredentials: true });
  }

  // logout
  public logout() {
    
    this.signOut().subscribe({
      next : res => {
        console.log(res);
        if(res.result)
        {
          this.removeToken();
        }
      },
      error : err => {
        console.log(err);
      }
    })
  }

  refreshToken(): Observable<any> {
    const header = new HttpHeaders();
    return this.http.get(`${this.baseUrl}Auth/GetRefreshToken`, { headers: header, withCredentials: true });
  }

  // this is for when user authentication fails 2 consecutive times
  // 1st time access token expired but 2nd time some how refresh token validity expired
  // sp clearing all cookies and need to log in again
  revokeToken(): Observable<any> {
    const header = new HttpHeaders();
    return this.http.delete(`${this.baseUrl}Auth/RevokeToken`, {headers : header, withCredentials : true});
  }

  // ================================================Token related =====================================
  // this can be sfifted to another service
  updateToken(status: boolean) {
    this.isAuthenticated = status;
    let ch = this.isAuthenticated;
    console.log("from  user status", ch)
  }


  saveToken(token: string) {
    // get the user user email or something and set to cookie for ui interaction according to it
    this.cookie.set("curr-app-user", encodeURIComponent(token));
  }


  getCurrentUser() : Observable<any>
  {
    return this.http.get(`${this.baseUrl}Auth/GetCurrentUser`, { withCredentials: true });
  }

  isLoggegIn():  boolean
  {
    this.getCurrentUser().subscribe({
      next : res => {
        // console.log("After checking froms erver also --> ",res);
        this.saveToken(res.userEmail);
        this.isAuthenticated = res.result;
        // this.route.navigate(["/home"]);
        // console.log("autheee -->", this.isAuthenticated);
      },
      error : err => {
        // console.log("Invaliddd -->", err);
      }
    });

    // after matchis from server also
    if(this.isAuthenticated)
    {
      // window.location.reload();
      return true;
    }
    else
    {
      return false;
    }
  }

  removeToken() {
    // deleteing only the access token bcs it is loggin out manually
    // so next time user log s in , it only need s to get access token
    this.cookie.delete("curr-app-user");  
    // window.location.reload();
  }

  getToken(): string | null {
    // will get current token , else return null
    return this.cookie.get("curr-app-user") || null;
  }
}

// ================================================ Token related Ends=====================================