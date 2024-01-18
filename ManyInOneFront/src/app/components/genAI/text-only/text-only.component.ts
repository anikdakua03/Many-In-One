import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { TextOnlyService } from '../../../shared/services/text-only.service';
import { RouterLink } from '@angular/router';
import { MarkdownModule } from 'ngx-markdown';

@Component({
  selector: 'app-text-only',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, MarkdownModule],
  templateUrl: './text-only.component.html',
  styles: ``
})
export class TextOnlyComponent {

  // @ViewChild(LoaderComponent) loader?: LoaderComponent;

  response: any = "";

  inputForm: FormGroup = new FormGroup({
    inputText: new FormControl("", [Validators.required, Validators.minLength(4), Validators.maxLength(50)]),
  });

  constructor(public service: TextOnlyService, private toaster: ToastrService)//,
  {

  }

  getAnswer() {
    const obj = this.inputForm.value;
    console.log(" Your query is componeent --> " + this.inputForm);

    if (this.inputForm.valid) {
      this.service.formSubmitted = true; // making flag true

      this.service.askQuestion(obj)
        .subscribe(
          {
            next: res => {
              // console.log("From text compo ---> " , res.responseMessage);
              // this.loader?.showLoader(true);
              this.response = res.responseMessage;
              // this.loader = false;
              // this.loader?.showLoader(false);
            },
            error: err => {
              console.log("This is an error --> ", err.error.title);
              // this.loader = false;
              // this.toaster.show(err.error.title);
            }
          }
        );
    }
    else {
      console.error("Please ask your query !!");
    }
  }
}
