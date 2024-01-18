import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthenticationService } from '../../../shared/services/authentication.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './header.component.html',
  styles: ``
})
export class HeaderComponent {

  userStatus: boolean = false;

  constructor(private authService: AuthenticationService, private router: Router) {
    debugger
    this.userStatus = this.authService.isLoggegIn();
    console.log("From navbar-->", this.userStatus);
  }

  onLogout() {
    this.authService.logout();
    this.userStatus = false;
    this.router.navigate(['/login']);
  }
}
