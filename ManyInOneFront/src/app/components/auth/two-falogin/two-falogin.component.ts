import { Component } from '@angular/core';
import { AuthResponse } from '../../../shared/models/auth-response.model';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { ToastrService } from 'ngx-toastr';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-two-falogin',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './two-falogin.component.html',
  styles: ``
})
export class TwoFALoginComponent {

  authResponseDto: AuthResponse = new AuthResponse();
  twoFALoginForm!: FormGroup;


  constructor(protected authService: AuthenticationService, private fb: FormBuilder, private toaster: ToastrService, private router: Router, private cookie: CookieService) {

    this.twoFALoginForm = this.fb.group({
      twoFACode: new FormControl("", [Validators.required, Validators.minLength(6), Validators.maxLength(6)])
    });
  }

  on2FALogin() {
    this.authService.verifyAndLogin(this.twoFALoginForm.value).subscribe({
      next:
        res => {
          console.log(res);
          // get the user user email or something and set to cookie for ui interaction according to it
          this.authService.saveToken(res.userId);
          sessionStorage.setItem("two-fa", res.twoFAEnabled.toString());
          this.toaster.success("Login Successful !!", "User Logged in");
          this.router.navigateByUrl("/home");
        }
    });
  }
}
