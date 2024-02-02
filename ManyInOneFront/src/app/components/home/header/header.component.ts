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

  // logout
  onLogout() {
  
    this.authService.signOut().subscribe({
      next: res => {
        console.log(res);
        this.authService.currUserSignal.set(null);
        if (res.result) {
          this.authService.removeToken();
          this.router.navigateByUrl('/'); // go to home
          window.location.reload();
        }
      },
      error: err => {
        console.log(err);
      }
    })
  }
}
