import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
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

  constructor(protected authService: AuthenticationService, private router: Router) {
  }

  ngOnInit(): void {
    this.authService.isAuthenticatedd.subscribe((data) => {
      this.checkCurrUser = data;
    }
    );
  }

  // logout
  onLogout() {
    debugger
    this.authService.signOut().subscribe({
      next: res => {
        if (res.result) {
          this.authService.isAuthenticatedd.next(false); // set false
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
