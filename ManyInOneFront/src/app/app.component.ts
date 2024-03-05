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
    if (window.scrollY > 50) {
      this.isMenuScrolled = true;
    }
    else {
      this.isMenuScrolled = false;
    }
  }

  // any time user reloads the page will get the user based on token from server
  ngOnInit(): void {

  }

  scrollToTop() {
    document.body.scrollIntoView({
      behavior : 'smooth'
    });
  }
}
