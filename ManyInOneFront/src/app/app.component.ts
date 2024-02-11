import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { HttpClientModule } from '@angular/common/http';
import { HeaderComponent } from './components/home/header/header.component';
import { RouterOutlet } from '@angular/router';
import { AuthenticationService } from './shared/services/authentication.service';

@Component({
    selector: 'app-root',
    standalone: true,
    templateUrl: './app.component.html',
    styles: [],
    imports: [CommonModule, RouterOutlet, HttpClientModule, HeaderComponent, ToastrModule]
})
export class AppComponent implements OnInit {
  title = 'Many In One';
  constructor(private authService: AuthenticationService) {

  }

  // any time user reloads the page will get the user based on token from server
  ngOnInit(): void {
    this.authService.getCurrentUser().subscribe({
      next: res => {
        // and set the curruserSignal and will be avalibale for whole application
        // console.log(res);
        this.authService.currUserSignal.set(res);
      },
      error: err => {
        this.authService.currUserSignal.set(null);
      }
    });

    if(this.authService.isLoggegIn())
    {
      
    }
  }
}
