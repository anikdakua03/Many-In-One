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
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, FontAwesomeModule],
  templateUrl: './register.component.html',
  styles: ``
})
export class RegisterComponent {

  clientId: string = environment.clientId;
  dots = FAIcons.ELLIPSES;
  user = FAIcons.USER;
  longRightArrow = FAIcons.LONG_RIGHT_ARROW;
  authResponseDto: AuthResponse = new AuthResponse();
  registerForm!: FormGroup;
  isLoading: boolean = false;

  constructor(private authService: AuthenticationService, private fb: FormBuilder, private toaster: ToastrService, private router: Router, private _ngZone: NgZone, @Inject(PLATFORM_ID) private platformId: Object) {
    
    this.registerForm = this.fb.group({
      // name: new FormControl("", [Validators.required,]),
      email: new FormControl("", [Validators.required, Validators.email]),
      password: new FormControl("", [Validators.required, Validators.minLength(6)])
    });
  }

  ngOnInit(): void {
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
    this.isLoading = true;
    this.authService.registerWithGoogle(response.credential).subscribe(res => {
      if (res.isSuccess) {
        this._ngZone.run(() => {
          this.isLoading = false;
          // set the user signal for whole application
          this.authService.isAuthenticated$.next(true);
          this.authService.saveToken("x-app-user", res.data.userId);
          this.authService.saveToken("x-user-name", res.data.userName);
          this.authService.saveToken("twofa-enable", res.data.twoFAEnabled);
          this.router.navigateByUrl('/home');
          this.toaster.success("Registration with google Successful !!", "User Registered successfully !!");
        });
      }
      else {
        this.isLoading = false;
        this.toaster.error(res.error?.description, "User Registration error!!");
      }
    },
      (err: any) => {
        this.isLoading = false;
        this.toaster.error("User registration unsuccessful.", "User registration failed !!");
      });
  }

  // registerDto : Register
  onRegister() {
    if (this.registerForm.valid) {
      this.isLoading = true;
      this.authService.register(this.registerForm.value).subscribe({
        next:
        res => {
          if (res.isSuccess) {
            // roue to login
            this.isLoading = false;
            this.router.navigateByUrl('/login');
            this.toaster.success("Registration successful, please confirm email from your inbox to continue.", "User registered");
          }
          else {
            this.isLoading = false;
            this.router.navigateByUrl('/register');
            this.toaster.success(res.error.description, "User registered");
          }
        },
        error:
          err => {
          this.isLoading = false;
            this.toaster.error("Registration failed", "User registration failed !!");
          }
      });
    }
    else {
      this.registerForm.markAllAsTouched(); // will show all the errors
    }
  }
}
