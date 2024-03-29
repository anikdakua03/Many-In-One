import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Enable2FAComponent } from "../2FA/enable2-fa/enable2-fa.component";
import { Disable2FAComponent } from "../2FA/disable2-fa/disable2-fa.component";
import { TwoFALoginComponent } from "../two-falogin/two-falogin.component";
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { ToastrService } from 'ngx-toastr';
import { NgxLoadingModule } from 'ngx-loading';

@Component({
    selector: 'app-manage-auth',
    standalone: true,
    templateUrl: './manage-auth.component.html',
    styles: ``,
    imports: [RouterLink, Enable2FAComponent, Disable2FAComponent, TwoFALoginComponent, NgxLoadingModule]
})
export class ManageAuthComponent {

    isLoading : boolean = false;

  constructor(private authService : AuthenticationService, private toaster : ToastrService, private route : Router)
  {}


  deleteAllUserData()
  {
    var res = confirm("Do you really want to delete all data ??");
    if(res)
    {
      this.isLoading = true;
      this.authService.deleteAllUserData().subscribe({
        next: re => {
          this.isLoading = false;
          this.route.navigateByUrl('/');
          this.authService.removeToken();
          this.toaster.success("User's all data deleted successfully", "User data deletion");
        }
      });
    }
  }
}
