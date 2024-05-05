import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';

@Component({
  selector: 'app-welcome',
  standalone: true,
  imports: [RouterLink, FontAwesomeModule],
  templateUrl: './welcome.component.html',
  styles: ``
})
export class WelcomeComponent {

  longRightArrow = FAIcons.LONG_RIGHT_ARROW;
}
