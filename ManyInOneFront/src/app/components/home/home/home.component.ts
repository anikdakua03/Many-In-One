import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, FontAwesomeModule],
  templateUrl: './home.component.html',
  styles: ``
})
export class HomeComponent {
  longRightArrow = FAIcons.LONG_RIGHT_ARROW;
}
