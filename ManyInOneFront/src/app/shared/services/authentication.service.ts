import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthResponse } from '../models/auth-response.model';

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


  constructor(private http: HttpClient) {
    // will check if token is available or not 
    const token = this.getToken();
    // console.log("tokennnn --< ", token)
    if (token) {
      this.isAuthenticated = true;
      // this.updateToken(true);
    }
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

  // signout from google
  signOutExternal() {

    localStorage.removeItem("x-access-token");
    localStorage.removeItem("x-refresh-token");
    window.location.reload();
    console.log("token removed");
  }

  // logout
  public logout() {
    this.removeToken();
    window.location.reload();
    // return this.http.get(`${this.baseUrl}${this.loginUrl}`).pipe(
    //   map((res) => {
    //     console.log(res);
    //     if(res)
    //     {
    //       this.removeToken();
    //     }

    //     return res;
    //   })
    // );
  }

  // this can be sfifted to another service
  updateToken(status: boolean) {
    this.isAuthenticated = status;
    var ch = this.isAuthenticated;
    console.log("from  user status", ch)
  }

  isUserAuthenticated() {
    return this.isAuthenticated;
  }


  // ================================================Token related =====================================
  refreshToken(): Observable<any> {
    const header = new HttpHeaders();
    return this.http.get(`${this.baseUrl}Auth/GetRefreshToken`, { headers: header, withCredentials: true });
  }

  revokeToken(): Observable<any> {
    // const token = localStorage.getItem("x-refresh-token");
    const header = new HttpHeaders();
    return this.http.delete(`${this.baseUrl}Auth/RevokeToken`, {headers : header, withCredentials : true});
  }

  saveToken(token: string) {
    localStorage.setItem("x-access-token", token);
  }
  isLoggegIn(): boolean {
    if (localStorage.getItem('x-access-token')) {
      return true;
    }
    return false;
  }
  // ================================================ Token related Ends=====================================

  removeToken() {
    localStorage.removeItem("token");
  }

  setToken(token: string) {
    // this.updateToken(true);
    localStorage.setItem("token", token); // this need to be updated with old token , when we implent refresh token
  }

  getToken(): string | null {
    // will get current token , else return null
    return localStorage.getItem("token") || null;
  }
}
