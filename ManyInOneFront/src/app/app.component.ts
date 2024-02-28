import { Component, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { HttpClientModule } from '@angular/common/http';
import { HeaderComponent } from './components/home/header/header.component';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    standalone: true,
    templateUrl: './app.component.html',
    styles: [],
    imports: [CommonModule, RouterOutlet, HttpClientModule, HeaderComponent, ToastrModule]
})
export class AppComponent implements OnInit {
  title = 'Many In One';
  constructor() {

  }

  isMenuScrolled: boolean = false;

  @HostListener('window:scroll', ['$event'])
  scrollCheck() {
    if (window.scrollY > 100) {
      this.isMenuScrolled = true;
    }
    else {
      this.isMenuScrolled = false;
    }
    // console.log("Scrolled or not", this.isMenuScrolled);
  }

  // any time user reloads the page will get the user based on token from server
  ngOnInit(): void {
    // this.authService.getCurrentUser().subscribe({
    //   next: res => {
    //     // and set the curr userSignal and will be avalibale for whole application
    //     // console.log(res);
    //     this.authService.currUserSignal.set(res);
    //   },
    //   error: err => {
    //     this.authService.currUserSignal.set(null);
    //   }
    // });

    // if(this.authService.isLoggegIn())
    // {
      
    // }
  }

  scrollToTop() {
    document.body.scrollIntoView({
      behavior : 'smooth'
    });
  }
}
