import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ToastrService } from 'ngx-toastr';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';
import { IResetPassword } from '../../../shared/models/auth-response.model';
import { AuthenticationService } from '../../../shared/services/authentication.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, FontAwesomeModule],
  templateUrl: './reset-password.component.html',
  styles: ``
})
export class ResetPasswordComponent {

  resetPasswordForm!: FormGroup;
  isLoading: boolean = false;
  formSubmitted: boolean = false;
  resetCode: string | undefined;
  userEmail: string | undefined;
  dots = FAIcons.ELLIPSES;

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
      const currentMode = this.activatedRoute.queryParamMap.subscribe({
        next: qp => {
          const qps = qp.keys.length;
          if (qps !== 2) // means must need these two params
          {
            this.router.navigateByUrl("/login");
            return; // no need to proceed further
          }
          this.userEmail = qp.get("userEmail") || "";
          this.resetCode = qp.get("code") || "";
          // initialize the form
          if ((this.userEmail !== undefined || this.userEmail !== "") && (this.resetCode !== undefined || this.resetCode !== "")) {
            this.initializeForm(this.userEmail);
          }
        }
      });
    }
  }

  initializeForm(userEmail: string) {
    this.resetPasswordForm = this.fb.group({
      email: [{ value: userEmail, disabled: true }],
      newPassword: ["", [Validators.required, Validators.minLength(6)]],
      confirmPassword: ["", [Validators.required, Validators.minLength(6)]], // validator with new password
    });
  }

  onResetPasswordClick() {
    if (this.resetPasswordForm.valid) {
      if(this.resetPasswordForm.value.newPassword !== this.resetPasswordForm.value.confirmPassword)
      {
        this.toaster.error("New password and confirm password doesn't match!", "Password reset error !");
        return;
      }
      this.formSubmitted = true;
      this.isLoading = true;
      const resetPasswordObj : IResetPassword = {
        email : this.userEmail || "",
        code: this.resetCode || "",
        newPassword: this.resetPasswordForm.get("newPassword")?.value,
        confirmPassword: this.resetPasswordForm.get("confirmPassword")?.value
      };
      this.authService.resetPassword(resetPasswordObj).subscribe({
          next:
            res => {
              // roue to login
              this.isLoading = false;
              this.router.navigateByUrl('/login');
              this.toaster.success("Password reset successfully !", "Password reset !");
            },
          error:
            err => {
              this.isLoading = false;
              this.toaster.error("Password reset failed ! Please try again !", "Password reset error !");
            }
        });
    }
    else {
      // this.router.navigateByUrl("/login"); 
      this.resetPasswordForm.markAllAsTouched(); // will show all the errors
    }
  }

}
