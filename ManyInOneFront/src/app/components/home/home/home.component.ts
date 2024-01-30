import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthenticationService } from '../../../shared/services/authentication.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './home.component.html',
  styles: ``
})
export class HomeComponent  {
  constructor(private authService: AuthenticationService) {
  }
// implements OnInit
  // // any time user reloads the page will get the user based on token from server
  // ngOnInit(): void {
  //   this.authService.getCurrentUser().subscribe({
  //     next: res => {
  //       // and set the curruserSignal and will be avalibale for whole application
  //       console.log(res);
  //       this.authService.currUserSignal.set(res.userId);
  //     },
  //     error: err => {
  //       this.authService.currUserSignal.set(null);
  //     }
  //   });
  // }
}
