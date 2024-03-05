declare var google: any;

import { Component, Inject, NgZone, PLATFORM_ID } from '@angular/core';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Router, RouterLink } from '@angular/router';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap';
import { AuthResponse } from '../../../shared/models/auth-response.model';
import { environment } from '../../../../environments/environment';
import { NgxLoadingModule } from 'ngx-loading';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgxLoadingModule],
  templateUrl: './register.component.html',
  styles: ``
})
export class RegisterComponent {

  clientId: string = environment.clientId;


  authResponseDto: AuthResponse = new AuthResponse();
  registerForm!: FormGroup;
  isLoading: boolean = false;
  localUser: any = {
    userId: "",
    userName: "",
    is2FaEnabled: false
  };

  constructor(private authService: AuthenticationService, private fb: FormBuilder, private toaster: ToastrService, private router: Router, private _ngZone: NgZone, @Inject(PLATFORM_ID) private platformId: Object) {
    
    this.registerForm = this.fb.group({
      // name: new FormControl("", [Validators.required,]),
      email: new FormControl("", [Validators.required, Validators.email]),
      password: new FormControl("", [Validators.required, Validators.minLength(6)])
    });
  }

  ngOnInit(): void {
    // console.log("platform id for register -->", this.platformId);
    if (isPlatformBrowser(this.platformId)) {
      // we do not need this on google library load , it is causing not rendering button properly
      // (window as any).onGoogleLibraryLoad = () => {
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
          document.getElementById("onGoogleRegister")!,
          { theme: "outline", size: "large", width: 100 }
        );
        
        // will send to google with all above info 
        // google will give some info and token
        google.accounts.id.prompt((notification: PromptMomentNotification) => (""));
      // }
    }
  }

  // google registering related
  handleCredentialResponse(response: CredentialResponse) {
    // then will send it to our api to confirm and check
    this.authService.registerWithGoogle(response.credential).subscribe(res => {
      // localStorage.setItem("x-access-token", res.token);
      this._ngZone.run(() => {
        // set the user signal for whole application
        this.authService.isAuthenticated$.next(true);
        this.authService.saveToken("x-app-user", res.userId);
        this.authService.saveToken("x-user-name", res.userName);
        this.authService.saveToken("twofa-enable", res.twoFAEnabled);
        this.router.navigateByUrl('/home');
        this.toaster.success("Registered with google Successful !!", "User Registered successfully !!");
      });
    },
      (err: any) => {
        console.log(err);
        this.toaster.error(err, "User registration failed !!");
      });
  }

  // registerDto : Register
  onRegister() {
    if (this.registerForm.valid) {
      this.isLoading = true;
      // console.log("Register form ---> ", this.registerForm.value);
      this.authService.register(this.registerForm.value).subscribe({
        next:
        res => {
          // roue to login
          this.isLoading = false;
          this.router.navigateByUrl('/login');
          this.toaster.success(res.message, "User registered");
        },
        error:
        err => {
          console.log(err);
          this.isLoading = false;
            this.toaster.error(err.message, "User registration failed !!");
          }
      });
    }
    else {
      this.registerForm.markAllAsTouched(); // will show all the errors
    }
  }
}
