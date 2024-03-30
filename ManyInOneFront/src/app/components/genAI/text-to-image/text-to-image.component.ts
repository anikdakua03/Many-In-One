import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { GenAIService } from '../../../shared/services/gen-ai.service';

@Component({
  selector: 'app-text-to-image',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './text-to-image.component.html',
  styles: ``
})
export class TextToImageComponent {

  response: string = "";

  inputForm: FormGroup = new FormGroup({
    inputText: new FormControl("", [Validators.required, Validators.minLength(4)]),
  });

  isLoading: boolean = false;

  constructor(public service: GenAIService, private toaster: ToastrService) {
  }

  getAnswer() {
    const obj = this.inputForm.value;
    if (this.inputForm.valid) {
      this.service.formSubmitted = true; // making flag true
      this.isLoading = true;
      this.service.generateImage(obj)
        .subscribe(
          {
            next: res => {
              this.response = res.data.responseMessage;
              this.isLoading = false;
              this.toaster.success("Here is your response", "Response");
            },
            error: err => {
              this.isLoading = false;
            }
          }
        );
    }
    else {
      this.toaster.error("Please ask your query !!", " Error !!");
    }
  }

  clearAll() {
    this.inputForm.reset();
    this.response = "";
  }
}
