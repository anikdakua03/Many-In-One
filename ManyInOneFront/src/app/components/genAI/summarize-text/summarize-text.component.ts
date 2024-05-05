import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { KatexOptions, MarkdownModule } from 'ngx-markdown';
import { ToastrService } from 'ngx-toastr';
import { CopyToClipboardComponent } from '../../copy-to-clipboard/copy-to-clipboard.component';
import { GenAIService } from '../../../shared/services/gen-ai.service';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-summarize-text',
  standalone: true,
  imports: [ReactiveFormsModule, MarkdownModule, FontAwesomeModule],
  templateUrl: './summarize-text.component.html',
  styles: ``
})
export class SummarizeTextComponent implements OnInit {

  @ViewChild('myTextArea') myTextArea!: ElementRef;

  public options: KatexOptions = {
    displayMode: true,
    throwOnError: false,
    errorColor: 'red',
    // delimiters: [...],
  };

  readonly clipBoardButton = CopyToClipboardComponent;

  response: string = "";
  currentWordCount: number = 0;

  inputForm: FormGroup = new FormGroup({
    inputText: new FormControl("", [Validators.required, Validators.minLength(4)]),
  });

  isLoading: boolean = false;
  dots = FAIcons.ELLIPSES;

  constructor(public service: GenAIService, private toaster: ToastrService) {
  }

  ngOnInit(): void {
    // live word count when words added or removed
    this.inputForm.controls['inputText'].valueChanges.subscribe(value => {
      this.currentWordCount = this.wordCounter(value);
      }
    );
  }

  // for auto adjusting textarea heights
  onInput(event: Event) {
    const textArea = event.target as HTMLTextAreaElement;
    const borderBoxHeight = (textArea.offsetHeight - textArea.clientHeight) + 'px';
    textArea.style.height = 'auto';
    textArea.style.height = (textArea.scrollHeight + parseInt(borderBoxHeight)) + 'px';
  }

  wordCounter(text : string) : number
  {
    return text.trim().split(/\s+/).length;
  }

  getAnswer() {
    const obj = this.inputForm.value;
    if (this.inputForm.valid) {
      if (this.inputForm.value.inputText.split(' ').length > 510) {
        this.toaster.error("Currently not accepting text more than 510 words.", "Very long text error !")
      }
      this.service.formSubmitted = true; 
      this.isLoading = true;
      this.service.askToSummarize(obj)
        .subscribe(
          {
            next: res => {
              if(res.isSuccess)
              {
                this.response = res.data.responseMessage;
                this.isLoading = false;
                this.toaster.success("Here is your response", "Response");
              }
              else
              {
                this.isLoading = false;
                this.toaster.error("Maybe service isn't available right now, please try again now or later.", "Response");
              }
            },
            error: err => {
              this.isLoading = false;
              this.toaster.error("Maybe service isn't available right now, please try again now or later.", "Response");
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
    this.currentWordCount = 0;
  }
}
