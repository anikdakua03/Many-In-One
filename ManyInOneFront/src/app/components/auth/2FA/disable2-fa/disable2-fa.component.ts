import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ToastrService } from 'ngx-toastr';
import { FAIcons } from '../../../../shared/constants/font-awesome-icons';
import { AuthenticationService } from '../../../../shared/services/authentication.service';

@Component({
  selector: 'app-disable2-fa',
  standalone: true,
  imports: [RouterLink, FontAwesomeModule],
  templateUrl: './disable2-fa.component.html',
  styles: ``
})
export class Disable2FAComponent {

  isLoading : boolean = false;
  dots = FAIcons.ELLIPSES;

  constructor(private authService: AuthenticationService, private toaster: ToastrService) {

  }

  onDisable2FA() 
  {
    var res = confirm("Do you really want to disable 2 factor authentication ??");
    if (res) 
    {
      this.authService.disableAuthenticator().subscribe({
        next: res => {
          this.isLoading = false;
          this.authService.saveToken("twofa-enable", "false");
          this.toaster.success("Disabled successfully !!", "2 FA code disable");
        },
        error: err => {
        this.isLoading = false;
          this.toaster.error("Unable to disable 2FA !!", "2 FA code disable");
        }
      });
    }
}
}
