import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { IConfirmEmail } from '../../../shared/models/auth-response.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './confirm-email.component.html',
  styles: ``
})
export class ConfirmEmailComponent implements OnInit {

  emailConfirmed: boolean = true;

  constructor(private authService: AuthenticationService, private router: Router, private activatedRoute: ActivatedRoute, private toaster: ToastrService) {

  }

  ngOnInit(): void {
    // will check if user is authenticated then will go to home
    const check = this.authService.isAuthenticated$.value;
    if (check) {
      // go to home
      this.router.navigateByUrl("/home");
    }
    else {
      // other wise take the token and user id from link and check for confirmation from api response
      this.activatedRoute.queryParamMap.subscribe({
        next: qp => {
          const qps = qp.keys.length;
          if(qps !== 2) // means must need these two params
          {
            this.router.navigateByUrl("/login");
            return; // no need to proceed further
          }
          // if(qp.getAll(""))
          // extract user id and code and make the body
          const confirmEmailBody: IConfirmEmail = {
            userId: qp.get("userId") || "",
            confirmationCode: qp.get("code") || ""
          };
          // send for confirmation
          this.authService.confirmEmail(confirmEmailBody).subscribe({
            next: res => {
              // toaster success message
              if (res.isSuccess && res.data.result) {
                this.toaster.success("Email confirmed successfully !", "Email Confirmation ");
              }
              else if (!res.isSuccess && res.error.description.includes("User email confirmed already , please log in to continue !!")) {
                this.toaster.info("User email confirmed already, Please login to continue. ", "Email Confirmation ");
              }
            },
            error: err => {
              // some toaster or some validation flag
              this.emailConfirmed = false;
            }
          });
        },
        error: err => {
          // if there no query params
          this.router.navigateByUrl("/login");
        }
      });
    }
  }

  resendConfirmationEmailLink() {
    this.router.navigateByUrl("/account/send-email/resend-confirmation-mail");
  }
}
