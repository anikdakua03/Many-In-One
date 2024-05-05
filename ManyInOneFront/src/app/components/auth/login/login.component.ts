declare var google: any;

import { isPlatformBrowser } from '@angular/common';
import { Component, Inject, NgZone, PLATFORM_ID } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../../environments/environment';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';
import { AuthResponse } from '../../../shared/models/auth-response.model';
import { AuthenticationService } from '../../../shared/services/authentication.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, FontAwesomeModule],
  templateUrl: './login.component.html',
  styles: ``,
})
export class LoginComponent {

  clientId: string = environment.clientId;
  dots = FAIcons.ELLIPSES;
  longRightArrow = FAIcons.LONG_RIGHT_ARROW;
  at = FAIcons.AT;
  lock = FAIcons.LOCK;
  authResponseDto: AuthResponse = new AuthResponse();
  loginForm!: FormGroup;
  twoFALoginForm!: FormGroup;
  isLoading: boolean = false;
  showTwoFA: boolean = false;
  userIdWith2FA: string = ""; // when have 2fa login enabled then it will set and will be passed for 2fa code verification
  emailNotConfirmed: boolean = false;


  constructor(protected authService: AuthenticationService, private fb: FormBuilder, private toaster: ToastrService, private router: Router, private _ngZone: NgZone, @Inject(PLATFORM_ID) private platformId: Object) 
  {
    this.loginForm = this.fb.group({
      email: new FormControl("", [Validators.required, Validators.email]),
      password: new FormControl("", [Validators.required, Validators.minLength(6)])
    });
    this.twoFALoginForm = this.fb.group({
      twoFACode: new FormControl("", [Validators.required, Validators.minLength(6), Validators.pattern("[0-9]*")]),
      currUserId: new FormControl(""),
    });
  }
  
  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      // we do not need this on google library load , it is causing not rendering button properly
      // (window as any).onGoogleLibraryLoad = async () => {
        // setting up the clients id 
        // we could set up here , to our api url to hit and get some response or can also create callback to do some 
        // but we can't have both 
        google.accounts.id.initialize({
          client_id: this.clientId,
          callback: this.handleCredentialResponse.bind(this),
          auto_select: false,
          cancel_on_tap_outside: true
        });
  
        // setting up the google log in pop up
        google.accounts.id.renderButton(
          document.getElementById("onGoogleLogin")!,
          { theme: "outline", size: "large", width: 100, shape : "rectangle" }
        );
  
        // will send to google with all above info 
        // google will give some info and token
        google.accounts.id.prompt((notification: PromptMomentNotification) => (""));
      // }
    }
  }

  handleCredentialResponse(response: CredentialResponse) {
  // then will send it to our api to confirm and check
    this.isLoading = true;
    this.authService.logInWithGoogle(response.credential).subscribe(res => {
      if (res.isSuccess) {      
        this._ngZone.run(() => {
          this.isLoading = false;
          this.authService.isAuthenticated$.next(true);
          this.authService.userName$.next(res.data.userName);
          this.authService.saveToken("x-app-user", res.data.userId);
          this.authService.saveToken("x-user-name", res.data.userName);
          this.authService.saveToken("twofa-enable", res.data.twoFAEnabled);
          this.router.navigate(['/home']);
          this.toaster.success("Login Successful !!", "User Logged in");
      });
      }
      else
      {
        this.isLoading = false;
        this.toaster.error(res.error?.description, "User Login error !");
      }
    },
      (err: any) => {
        this.isLoading = false;
        this.toaster.error(err.message, "User Logged in failed !!");
      });
  }

  // loginDto : Login
  onLogin() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.authService.login(this.loginForm.value).subscribe({
        next:
          res => {
            if (res.isSuccess && !res.data.twoFAEnabled) {
              this.isLoading = false;
              // get the user user email or something and set to cookie for ui interaction according to it
              this.authService.isAuthenticated$.next(true);
              this.authService.userName$.next(res.data.userName);
              this.authService.saveToken("x-app-user", res.data.userId);
              this.authService.saveToken("x-user-name", res.data.userName);
              this.authService.saveToken("twofa-enable", res.data.twoFAEnabled);
              this.toaster.success("Login Successful !!", "User Logged in");
              this.router.navigateByUrl("/home");
            }
            else if (res.isSuccess && res.data.twoFAEnabled) {
              this.isLoading = false;
              this.toaster.info("Enter two FA code from authenticator !!", "Two Factor Verification !!");
              this.showTwoFA = true;
              this.userIdWith2FA = res.data.userId;
            }
            else if (!res.isSuccess && res.error.description.includes("User email not confirmed yet. ")) {
              // not a confirmed email
              this.isLoading = false;
              this.emailNotConfirmed = true;
              this.router.navigateByUrl("/account/send-email/resend-confirmation-mail");
            }
            else {
              this.isLoading = false;
              this.toaster.error(res.error.description, "User Log in Error");
            }
          },
        error:
          err => {
            this.isLoading = false;
            this.toaster.error("Login failed !!", "User Logged in failed !!");
          }
      });
    }
    else {
      this.loginForm.markAllAsTouched(); // will show all the errors
    }
  }

  on2FALogin() {
    if (this.twoFALoginForm.valid) {
      this.isLoading = true;
      // let code = this.twoFALoginForm.get('twoFACode')?.value;
      this.twoFALoginForm.value.currUserId = this.userIdWith2FA;
      this.twoFALoginForm.value.currUserId = this.userIdWith2FA;
      this.authService.verifyAndLogin(this.twoFALoginForm.value).subscribe({
        next:
          res => {
            this.isLoading = false;
            // get the user user email or something and set to cookie for ui interaction according to it
            this.authService.isAuthenticated$.next(true);
            this.authService.userName$.next(res.data.userName);
            this.authService.saveToken("x-app-user", res.data.userId);
            this.authService.saveToken("x-user-name", res.data.userName);
            this.authService.saveToken("twofa-enable", res.data.twoFAEnabled);
            this.toaster.success("Login Successful !!", "User Logged in");
            this.router.navigateByUrl("/home");
          },
        error: err => {
          this.isLoading = false;
          this.toaster.error("Invalid code !!", "User Log in Failed");
        }
      });
    }
    else {
      this.twoFALoginForm.markAllAsTouched();
    }
  }
}
