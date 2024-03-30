import { Component } from '@angular/core';
import { QRCodeModule } from 'angularx-qrcode';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SsrCookieService } from 'ngx-cookie-service-ssr';
import { AuthenticationService } from '../../../../shared/services/authentication.service';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-enable2-fa',
    standalone: true,
    templateUrl: './enable2-fa.component.html',
    styles: ``,
    imports: [QRCodeModule, ReactiveFormsModule, RouterLink]
})
export class Enable2FAComponent {

  sharedKey : string = "";
  qrURI: string = "";
  is2FAEnabled : boolean = false;
  isLoading: boolean = false;

  twoFAForm!: FormGroup;
  
  constructor(private authService : AuthenticationService, private fb : FormBuilder, private toaster : ToastrService, private cookie : SsrCookieService, private router : Router)
  {
    // if two factor is not enabled , then will show and load qr and then cod eto verify
    this.is2FAEnabled = authService.CheckUser2FA();
    if (!this.is2FAEnabled)
    {
      // call the load and share qr ;
      const userid = JSON.parse(cookie.get("x-app-user")) || "";
      this.authService.loadAndShareQR(userid).subscribe({
        next : res => {
          this.sharedKey = res.data.sharedKey;
          this.qrURI = res.data.qr;
        }
      });
    }
    this.twoFAForm = this.fb.group({
      faCode: new FormControl("", [Validators.required,]),
    });
  }
  

  onVerify2fa()
  {
    if(this.twoFAForm.valid)
    {
      this.isLoading = true;
      let code = this.twoFAForm.get('faCode')?.value;
      this.authService.verifyFACode(code.toString()).subscribe({
        next : res => {
          if(res.isSuccess)
          {
            this.isLoading = false;
            this.authService.saveToken("twofa-enable", "true");
            this.toaster.success("Code verified successfully !!", "2 FA code verification");
            this.router.navigateByUrl("/manage");
          }
          else
          {
            this.isLoading = false;
            this.toaster.error(res.error.description!, "2 FA code verification");
          }
        },
        error : err => {
          this.isLoading = false;
          this.toaster.error("Some error occurred, please try again ", " Two Factor code verification");
        }
      });
    }
    else
    {
      this.isLoading = false;
      this.twoFAForm.markAllAsTouched();
    }
  }
}
