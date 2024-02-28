import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthenticationService } from '../../../shared/services/authentication.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './header.component.html',
  styles: ``
})
export class HeaderComponent implements OnInit {

  checkCurrUser: boolean = false;

  constructor(protected authService: AuthenticationService, private router: Router, @Inject(PLATFORM_ID) private platformId: object) {
  }

  ngOnInit(): void {
    // if (isPlatformBrowser(this.platformId)) {
      this.authService.isAuthenticated$.subscribe((data) => {
      this.checkCurrUser = data;
    });
    // }
  }

  // logout
  onLogout() {
    this.authService.signOut().subscribe({
      next: res => {
        if (res.result) {
          this.authService.isAuthenticated$.next(false); // set false
          this.authService.removeToken();
          window.location.reload();
          this.router.navigateByUrl('/'); // go to home
        }
      },
      error: err => {
        console.log(err);
      }
    })
  }
}
