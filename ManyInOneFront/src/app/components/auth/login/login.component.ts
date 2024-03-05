declare var google: any;

import { Component, Inject, NgZone, PLATFORM_ID} from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap';
import { environment } from '../../../../environments/environment';
import { AuthResponse } from '../../../shared/models/auth-response.model';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { ToastrService } from 'ngx-toastr';
import { Router, RouterLink } from '@angular/router';
import { NgxLoadingModule } from 'ngx-loading';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgxLoadingModule],
  templateUrl: './login.component.html',
  styles: ``,
})
export class LoginComponent {

  clientId: string = environment.clientId;

  authResponseDto: AuthResponse = new AuthResponse();
  loginForm!: FormGroup;
  twoFALoginForm!: FormGroup;
  isLoading: boolean = false;
  showTwoFA: boolean = false;
  userIdWith2FA: string = ""; // when have 2fa login enabled then it will set and will be passed for 2fa code verification
  localUser : any = {
    userId: "",
    userName : "",
    is2FaEnabled : false
  };


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
    // console.log("platform id for login -->", this.platformId);
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
    this.authService.logInWithGoogle(response.credential).subscribe(res => {
      this._ngZone.run(() => {
        this.authService.isAuthenticated$.next(true);
        this.authService.saveToken("x-app-user",res.userId);
        this.authService.saveToken("x-user-name",res.userName);
        this.authService.saveToken("twofa-enable",res.twoFAEnabled);
        this.router.navigate(['/home']);
        window.location.reload();
        this.toaster.success("Login Successful !!", "User Logged in");
      });
    },
      (err: any) => {
        console.log(err);
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
            console.log(res.userId);
            if (!res.twoFAEnabled) {
              this.isLoading = false;
              // get the user user email or something and set to cookie for ui interaction according to it
              this.authService.saveToken("x-app-user", res.userId);
              this.authService.saveToken("x-user-name", res.userName);
              this.authService.saveToken("twofa-enable", res.twoFAEnabled);
              // sessionStorage.setItem("two-fa", res.twoFAEnabled.toString());
              this.toaster.success("Login Successful !!", "User Logged in");
              this.router.navigateByUrl("/home");
            }
            else {
              this.isLoading = false;
              // this.router.navigateByUrl("/login/2FA");
              this.toaster.info("Enter two FA code from authenticator !!", "Two Factor Verification !!");
              this.showTwoFA = true;
              this.userIdWith2FA = res.userId;
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
    // console.log(this.twoFALoginForm.value);
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
            this.authService.saveToken("x-app-user", res.userId);
            this.authService.saveToken("x-user-name", res.userName);
            this.authService.saveToken("twofa-enable", res.twoFAEnabled);
            // sessionStorage.setItem("two-fa", res.twoFAEnabled.toString());
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
