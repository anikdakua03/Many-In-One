import { Component, ElementRef, ViewChild } from '@angular/core';
import { GenAIService } from '../../../shared/services/gen-ai.service';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';

@Component({
  selector: 'app-generate-speech',
  standalone: true,
  imports: [ReactiveFormsModule, FontAwesomeModule],
  templateUrl: './generate-speech.component.html',
  styles: ``
})
export class GenerateSpeechComponent {

  @ViewChild('myTextArea') myTextArea!: ElementRef;
  
  response: string = "";
  dots = FAIcons.ELLIPSES;

  inputForm: FormGroup = new FormGroup({
    inputText: new FormControl("", [Validators.required, Validators.minLength(4)]),
  });

  isLoading: boolean = false;

  constructor(public service: GenAIService, private toaster: ToastrService) {

  }

  // for auto adjusting textarea heights
  onInput(event: Event) {
    const textArea = event.target as HTMLTextAreaElement;
    const borderBoxHeight = (textArea.offsetHeight - textArea.clientHeight) + 'px';
    textArea.style.height = 'auto';
    textArea.style.height = (textArea.scrollHeight + parseInt(borderBoxHeight)) + 'px';
  }
  
  getAnswer() {
    const obj = this.inputForm.value;
    if (this.inputForm.valid) {
      this.service.formSubmitted = true; // making flag true
      this.isLoading = true;
      this.service.transformTextToSpeech(obj)
        .subscribe(
          {
            next: res => {
              if(res.data !== null)
              {
                this.loadAudioClick(res.data.responseMessage);
                this.isLoading = false;
                this.toaster.success("Here is your response", "Response");
              }
              else
              {
                this.isLoading = false;
                this.toaster.success("Please try again", "Response");
              }
            },
            error: err => {
              this.isLoading = false;
            }
          }
        );
    }
    else {
      this.toaster.error("Please put up your query text !!", " Error !!");
    }
  }

  // convert those audio byte to audio file
  loadAudioClick(data: string) {
    let binary = this.convertDataURIToBinary(data);
    let blob = new Blob([binary], { type: 'audio/mpeg' });
    let blobUrl = URL.createObjectURL(blob);
    this.response = blobUrl;
  }

  convertDataURIToBinary(dataURI: string) {
    let BASE64_MARKER = ';base64,';
    let base64Index = dataURI.indexOf(BASE64_MARKER) + BASE64_MARKER.length;
    let base64 = dataURI.substring(base64Index);
    let raw = window.atob(base64);
    let rawLength = raw.length;
    let array = new Uint8Array(new ArrayBuffer(rawLength));

    for (let i = 0; i < rawLength; i++) {
      array[i] = raw.charCodeAt(i);
    }
    return array;
  }

  clearAll() {
    this.inputForm.reset();
    this.response = "";
  }
}
