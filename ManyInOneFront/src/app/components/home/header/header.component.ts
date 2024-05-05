import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { ToastrService } from 'ngx-toastr';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule, FontAwesomeModule],
  templateUrl: './header.component.html',
  styles: ``
})
export class HeaderComponent implements OnInit {

  checkCurrUser: boolean = false;
  currUserName: string = "";
  isSidebarShowing: boolean = false;
  isLoading : boolean = false;
  dots = FAIcons.ELLIPSES;

  constructor(protected authService: AuthenticationService, private router: Router, private toaster: ToastrService) {

  }

  ngOnInit(): void {
      this.authService.isAuthenticated$.subscribe((data) => {
      this.checkCurrUser = data;
    });
    const data = this.authService.userName$.subscribe({
      next : res => {
        this.currUserName = res === "" ? "User" : res.replaceAll('"', '');
      }
    });
  }

  openSidebar() {
    this.isSidebarShowing = true;
  }
    
    closeSidebar() {
      this.isSidebarShowing = false;
  }

  // logout
  onLogout() {
    this.isLoading = true;
    this.authService.signOut().subscribe({
      next: res => {
        if (res.isSuccess) {
          this.isLoading = false;
          this.authService.isAuthenticated$.next(false); // set false
          this.authService.removeToken();
          this.router.navigateByUrl('/'); // go to home
          this.toaster.success("Signed out successfully.", "User Sign out");
        }
        else {
          this.isLoading = false;
          this.toaster.info("Please try again..", "User Sign out");
        }
      },
      error: err => {
        this.isLoading = false;
        this.toaster.error("Service is not available right now !!", "Service Error");
      }
    })
  }
}
