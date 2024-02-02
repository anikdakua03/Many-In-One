import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from '../../../../shared/services/authentication.service';

@Component({
  selector: 'app-disable2-fa',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './disable2-fa.component.html',
  styles: ``
})
export class Disable2FAComponent {

  constructor(private authService: AuthenticationService, private toaster: ToastrService) {

  }

  onDisable2FA() 
  {
    this.authService.disableAuthenticator().subscribe({
      next: res => {
        this.toaster.success("Disabled successfully !!", "2 FA code disable");
      },
      error: err => {
        this.toaster.error("Unable to disable 2FA !!", "2 FA code disable");
      }
    });
  }
}
