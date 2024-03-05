import { Component } from '@angular/core';

@Component({
  selector: 'app-copy-to-clipboard',
  standalone: true,
  imports: [],
  templateUrl: './copy-to-clipboard.component.html',
  styles: ``
})
export class CopyToClipboardComponent {
  clicked : boolean = false;

  onClickCopy()
  {
    this.clicked = true;
    // after 5 seconds make that changed to Copy
    setTimeout(() => {
      this.clicked = false;
    }, 5000);
  }
}
