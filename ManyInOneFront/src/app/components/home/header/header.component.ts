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


  constructor(protected authService: AuthenticationService, private router: Router) {
  }

  onLogout() {
    this.authService.logout();
    this.router.navigateByUrl('/'); // go to home
  }
}
