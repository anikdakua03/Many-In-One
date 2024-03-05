import { Component, OnInit } from '@angular/core';
import { AuthResponse } from '../../../shared/models/auth-response.model';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { NgxLoadingModule } from 'ngx-loading';

@Component({
  selector: 'app-two-falogin',
  standalone: true,
  imports: [ReactiveFormsModule, NgxLoadingModule],
  templateUrl: './two-falogin.component.html',
  styles: ``
})
export class TwoFALoginComponent implements OnInit {

  authResponseDto: AuthResponse = new AuthResponse();
  twoFALoginForm!: FormGroup;
  isLoading: boolean = false;


  constructor(protected authService: AuthenticationService, private fb: FormBuilder, private toaster: ToastrService, private router: Router) {
  }
  
  ngOnInit(): void {
    this.twoFALoginForm = this.fb.group({
      twoFACode: new FormControl("", [Validators.required, Validators.minLength(6)])
    });
    this.twoFALoginForm.markAsPristine();
  }

  
  on2FALogin() {
    console.log(this.twoFALoginForm.value.twoFACode);
    if (this.twoFALoginForm.valid) {
    this.isLoading = true;
    this.authService.verifyAndLogin(this.twoFALoginForm.value).subscribe({
      next:
      res => {
        this.isLoading = false;
          // get the user user email or something and set to cookie for ui interaction according to it
        // this.authService.saveToken(res.userId, res.userName);
          // sessionStorage.setItem("two-fa", res.twoFAEnabled.toString());
          this.toaster.success("Login Successful !!", "User Logged in");
          this.router.navigateByUrl("/home");
        },
      error: err => {
        this.isLoading = false;
        console.log(err);
        }
    });
    }
    else {
      this.toaster.error("Invalid code !!", "Two FA Code error !");
    }
  }
}
