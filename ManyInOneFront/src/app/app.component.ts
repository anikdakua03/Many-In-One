import { Component, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { HttpClientModule } from '@angular/common/http';
import { HeaderComponent } from './components/home/header/header.component';
import { RouterOutlet } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FAIcons } from './shared/constants/font-awesome-icons';

@Component({
    selector: 'app-root',
    standalone: true,
    templateUrl: './app.component.html',
    styles: [],
    imports: [CommonModule, RouterOutlet, HttpClientModule, HeaderComponent, ToastrModule, FontAwesomeModule]
})
export class AppComponent {
  title = 'Many In One';

  isMenuScrolled: boolean = false;
  up = FAIcons.UP_ANGLE;
  circle = FAIcons.BLACK_CIRCLE;

  @HostListener('window:scroll', ['$event'])
  scrollCheck() {
    if (window.scrollY > 50) {
      this.isMenuScrolled = true;
    }
    else {
      this.isMenuScrolled = false;
    }
  }

  scrollToTop() {
    document.body.scrollIntoView({
      behavior : 'smooth'
    });
  }
}
