declare var google: any;

import { Component, NgZone } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap';
import { environment } from '../../../../environments/environment';
import { AuthResponse } from '../../../shared/models/auth-response.model';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { ToastrService } from 'ngx-toastr';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styles: ``
})
export class LoginComponent {

  clientId: string = environment.clientId;


  authResponseDto: AuthResponse = new AuthResponse();
  loginForm!: FormGroup;


  constructor(protected authService: AuthenticationService, private fb: FormBuilder, private toaster: ToastrService, private router: Router, private _ngZone: NgZone) 
  {
    // debugger
    // if (authService.currUserSignal !== null && authService.currUserSignal !== undefined) {
    //   router.navigateByUrl('/home'); // if user already logged in , why they should go to login page
    // }
    this.loginForm = this.fb.group({
      email: new FormControl("", [Validators.required, Validators.email]),
      password: new FormControl("", [Validators.required, Validators.minLength(6)])
    });
  }


  ngOnInit(): void {
    (window as any).onGoogleLibraryLoad = () => {

      // setting up th eclients id 
      // we could set up here , to our api url to hit and get some response or can also create callback to do some 
      // buit we can't have both 
      google.accounts.id.initialize({
        client_id: this.clientId,
        callback: this.handleCredentialResponse.bind(this),
        auto_select: false,
        cancel_on_tap_outside: true
      });

      // setting up th egoogle log in pop up
      google.accounts.id.renderButton(
        document.getElementById("onGoogleLogin")!,
        { theme: "outline", size: "large", width: 100 }
      );

      // will send to google with all above info 
      // google will give some info and token
      google.accounts.id.prompt((notification: PromptMomentNotification) => (""));
    }
  }

  handleCredentialResponse(response: CredentialResponse) {
    // then will send it to our api to confirm and cehck
    this.authService.logInWithGoogle(response.credential).subscribe(res => {
      // localStorage.setItem("x-access-token", res.token);
      this._ngZone.run(() => {
        // set the user signal for whole application
        this.authService.currUserSignal.set(res);
        this.authService.saveToken(res.userId);
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
      // console.log("login --> ", this.loginForm.value);

      this.authService.login(this.loginForm.value).subscribe({
        next:
          res => {
            console.log(res.userId);
            if (!res.twoFAEnabled) {
              // get the user user email or something and set to cookie for ui interaction according to it
              this.authService.saveToken(res.userId);
              // sessionStorage.setItem("two-fa", res.twoFAEnabled.toString());
              this.toaster.success("Login Successful !!", "User Logged in");
              this.router.navigateByUrl("/home");
            }
            else {
              this.router.navigateByUrl("/login/2FA");
            }
          },
        error:
          err => {
            // console.log(err);
            this.toaster.error("Login failed !!", "User Logged in failed !!");
          }
      });
    }
    else {
      this.loginForm.markAllAsTouched(); // will show all the errors
    }
  }

}
