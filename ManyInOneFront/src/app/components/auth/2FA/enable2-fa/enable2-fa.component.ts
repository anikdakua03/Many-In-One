import { Component } from '@angular/core';
import { QRCodeModule } from 'angularx-qrcode';
import { RouterLink } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CookieService } from 'ngx-cookie-service';
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
  constructor(private authService : AuthenticationService, private fb : FormBuilder, private toaster : ToastrService, private cookie : CookieService)
  {
    // if two factor is not enabled , then will show and load qr and then cod eto verufy
    // const userId  = sessionStorage.getItem("curr-app-user");
    console.log("ghifdhgifudhg",this.authService.currUserSignal()?.userId) ;
    // if(authService.currUserSignal() !== undefined || authService.currUserSignal() !== null)
    // {
    //   this.is2FAEnabled = authService.currUserSignal()?.twoFAEnabled ?? false;
    // }

    // if (this.authService.currUserSignal)
    if (this.authService.isAuthenticatedd)
    {
      // call the load and share qr 
      // const userid = this.authService.currUserSignal || "";
      const userid = localStorage.getItem("curr-app-user") || "";
      this.authService.loadAndShareQR(userid).subscribe({
        next : res => {
          this.sharedKey = res.sharedKey;
          this.qrURI = res.qr;
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
          this.isLoading = false;
          console.log("code verification", res);
          this.toaster.success("Code verifed successfully !!", "2 FA code verification");
        },
        error : err => {
          this.isLoading = false;
          this.toaster.error("Invalid code, try again ", " Two Factor code verifiaction");
        }
      });
    }
    else
    {
      this.isLoading = false;
      this.toaster.error("Invalid code format"," Two Factor code");
    }
  }
}
