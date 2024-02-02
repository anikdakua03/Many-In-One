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

  twoFAForm!: FormGroup;
  constructor(private authService : AuthenticationService, private fb : FormBuilder, private toaster : ToastrService)
  {
    // this.is2FAEnabled = sessionStorage.getItem("two-fa") ==  "true" ? true : false;

    // if two factor is not enabled , then will show and load qr and then cod eto verufy
    // const userId  = sessionStorage.getItem("curr-app-user");
    if(this.authService.currUserSignal()?.twoFAEnabled === false && this.authService.currUserSignal()?.userId !== null)
    {
      const userid = this.authService.currUserSignal()?.userId || "";
      // call the load and share qr code
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
      let code = this.twoFAForm.get('faCode')?.value;
      this.authService.verifyFACode(code.toString()).subscribe({
        next : res => {
          console.log("code verification", res);
          this.toaster.success("Code verifed successfully !!", "2 FA code verification");
          // sessionStorage.setItem("two-fa", "true");
        }
      });
    }
    else
    {
      this.toaster.error("Invalid code format"," Two Factor code");
    }
  }
}
