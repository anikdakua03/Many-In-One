import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { HttpClientModule } from '@angular/common/http';
import { HeaderComponent } from './components/home/header/header.component';
import { RouterOutlet } from '@angular/router';
import { Enable2FAComponent } from "./components/auth/2FA/enable2-fa/enable2-fa.component";

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  styles: [],
  imports: [CommonModule, RouterOutlet, HttpClientModule, HeaderComponent, ToastrModule, Enable2FAComponent]
})
export class AppComponent {
  title = 'Many In One';
}
