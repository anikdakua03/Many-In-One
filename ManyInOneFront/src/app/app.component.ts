import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { HttpClientModule } from '@angular/common/http';
import { HeaderComponent } from './components/home/header/header.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HttpClientModule, HeaderComponent, ToastrModule],
  templateUrl: './app.component.html',
  styles: [],
})
export class AppComponent {
  title = 'Many In One';
}
