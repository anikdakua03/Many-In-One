import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TextAndImageOnlyService } from '../../../shared/services/text-and-image-only.service';
import { ToastrService } from 'ngx-toastr';
import { RouterLink } from '@angular/router';
import { KatexOptions, MarkdownModule } from 'ngx-markdown';
import { NgxLoadingModule } from 'ngx-loading';
import { CopyToClipboardComponent } from '../../copy-to-clipboard/copy-to-clipboard.component';

@Component({
  selector: 'app-text-and-image-only',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, MarkdownModule, NgxLoadingModule],
  templateUrl: './text-and-image-only.component.html',
  styles: ``
})
export class TextAndImageOnlyComponent {

  public options: KatexOptions = {
    displayMode: true,
    throwOnError: false,
    errorColor: 'red',
    // delimiters: [...],
};
  response: any = "";

  
  
  isLoading : boolean = false;
  readonly clipBoardButton = CopyToClipboardComponent;
  myForm: FormGroup;

  constructor(public service: TextAndImageOnlyService, private toaster: ToastrService) {
    this.myForm = new FormGroup({
      image: new FormControl(null),
      inputText: new FormControl('', Validators.required)
    });
    
  }

  imageUrl: any;

  onFileSelected(event: any) {
    const file = event.target.files[0];

    // for previewing
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.imageUrl = e.target.result;
    };
    reader.readAsDataURL(file);

    // only png or jpg or jpeg
    if (file.type === 'image/png' || file.type === 'image/jpg' || file.type === 'image/jpeg') {
      this.myForm.patchValue({ image: file });

    }
    else {
      alert("Only png, jpg file ");
      this.toaster.error("Only png , jpg file ")
    }
  }

  onSubmitt() {

    const formData = new FormData();
    formData.append('image', this.myForm.value.image, this.myForm.value.image.name);
    formData.append('textInput', this.myForm.value.inputText);

    if(this.myForm.valid)
    {
      this.isLoading = true;
      this.service.askQuestion(formData).subscribe(
        {
          next: res => {
            this.response = res.responseMessage;
            this.isLoading = false;
            this.toaster.success("Got response", "Success");
          },
          error: err => {
            console.log("This is error --> ", err.error);
            this.toaster.show(err.error);
          }
        }
      );
    }
    else
    {
      this.toaster.error("Fill out correctly," , "Error!!");
    }
  }

  clearAll() {
    this.myForm.reset();
    this.response = "";
  }
}
