declare var google: any;

import { Component, NgZone } from '@angular/core';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Router, RouterLink } from '@angular/router';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap';
import { AuthResponse } from '../../../shared/models/auth-response.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styles: ``
})
export class RegisterComponent {

  clientId: string = environment.clientId;

  // registerDto: Register = new Register();
  authResponseDto: AuthResponse = new AuthResponse();
  registerForm!: FormGroup;

  constructor(private authService: AuthenticationService, private fb: FormBuilder, private toaster: ToastrService, private router: Router, private _ngZone: NgZone) {
    
    this.registerForm = this.fb.group({
      name: new FormControl("", [Validators.required,]),
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
        document.getElementById("onGoogleRegister")!,
        { theme: "outline", size: "large", width: 100 }
      );

      // will send to google with all above info 
      // google will give some info and token
      google.accounts.id.prompt((notification: PromptMomentNotification) => (""));
    }
  }

  // google regisretin grelated
  handleCredentialResponse(response: CredentialResponse) {
    // then will send it to our api to confirm and cehck
    this.authService.registerWithGoogle(response.credential).subscribe(res => {
      // localStorage.setItem("x-access-token", res.token);
      this._ngZone.run(() => {
        this.router.navigateByUrl('/home');
        this.toaster.success("Registered with google Successful !!", "User Registered successfully !!");
      });
    },
      (err: any) => {
        console.log(err);
        this.toaster.error(err, "User registeration failed !!");
      });
  }

  //user registering with google
  onGoogleRegister() {
    console.log("ok registered with google");
    return "ok registered in with google";
  }

  // registerDto : Register
  onRegister() {
    if (this.registerForm.valid) {
      console.log("Register form ---> ", this.registerForm.value);
      this.authService.register(this.registerForm.value).subscribe({
        next:
          res => {
            console.log(res);
            // get the user user email or something and set to cookie for ui interaction according to it
            this.authService.saveToken(this.registerForm.value.email);
            this.toaster.success("Registeration Successful !!", "User registered");
          },
        error:
          err => {
            console.log(err);
            this.toaster.error(err.message, "User registration failed !!");
          }
      });
    }
    else {
      this.registerForm.markAllAsTouched(); // will show all the errors
    }
  }
}
