import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-send-email',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './send-email.component.html',
  styles: ``
})
export class SendEmailComponent implements OnInit {

  sendEmailForm!: FormGroup;
  isLoading: boolean = false;
  formSubmitted: boolean = false;
  mode: string | undefined;

  constructor(private authService: AuthenticationService, private fb: FormBuilder, private toaster: ToastrService, private router: Router, private activatedRoute: ActivatedRoute) {

  }

  ngOnInit(): void {
    // will check if user is authenticated then will go to home
    const check = this.authService.isAuthenticated$.value;
    if (check) {
      // go to home
      this.router.navigateByUrl("/home");
    }
    else {
      // try to get the mode and based on it will perform
      const currentMode = this.activatedRoute.snapshot.paramMap.get("mode");
      if (currentMode) {
        this.mode = currentMode;
        // initialize the form
        this.sendEmailForm = this.fb.group({
          userEmail: new FormControl("", [Validators.required, Validators.email]),
        });
      }
    }
  }

  sendEmail() {
    if (this.sendEmailForm.valid && this.mode) {
      if (this.mode.includes("resend-confirmation-mail")) {
        
        this.formSubmitted = true;
        this.isLoading = true;
        this.authService.resendConfirmationMail(this.sendEmailForm.get("userEmail")?.value).subscribe({
          next:
            res => {
              if(res.isSuccess)
              {
                // roue to login
                this.isLoading = false;
                this.router.navigateByUrl('/login');
                this.toaster.success("Email confirmation mail sent successfully !", "Confirmation mail Sent");
              }
              else
              {
                this.isLoading = false;
                this.toaster.error("Invalid user or doesn't exist, please check again !", "Confirmation mail sending error !");
              }
            },
          error:
            err => {
              this.isLoading = false;
              this.toaster.error("Not able to send, please try again !", "Confirmation mail failed to send !", {tapToDismiss : true});
            }
        });
      }
      else if (this.mode.includes("forgot-password-mail")) {
        
        this.formSubmitted = true;
        this.isLoading = true;
        this.authService.forgotPasswordMail(this.sendEmailForm.get("userEmail")?.value).subscribe({
          next:
            res => {
              if (res.isSuccess) {
              // roue to login
              this.isLoading = false;
              this.router.navigateByUrl('/login');
              this.toaster.success("Password reset mail sent successfully !", "Forgot Password Mail");
              }
              else {
                this.isLoading = false;
                this.toaster.error("Invalid user or doesn't exist, please check again !", "Confirmation mail sending error !");
              }
            },
          error:
            err => {
              this.isLoading = false;
              this.toaster.error("Invalid email, please check email !", "Forgot Password Mail");
            }
        });
      }
    }
    else {
      this.sendEmailForm.markAllAsTouched(); // will show all the errors
    }
  }

  cancel() {
    this.router.navigateByUrl("/login");
  }
}
