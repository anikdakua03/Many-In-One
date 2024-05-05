import { CommonModule } from '@angular/common';
import { AfterViewChecked, Component, ElementRef, ViewChild } from '@angular/core';
import { GenAIService } from '../../../shared/services/gen-ai.service';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CopyToClipboardComponent } from '../../copy-to-clipboard/copy-to-clipboard.component';
import { KatexOptions, MarkdownModule } from 'ngx-markdown';
import { Content, Conversation } from '../../../shared/interfaces/conversation';
import "prismjs"
import "prismjs/plugins/line-numbers/prism-line-numbers.js"
import "prismjs/plugins/line-highlight/prism-line-highlight.js"
import "prismjs/components/prism-csharp.min.js"
import "prismjs/components/prism-typescript.min.js"
import "prismjs/components/prism-javascript.min.js"
import "prismjs/components/prism-c.min.js"
import "prismjs/components/prism-python.min.js"
import "prismjs/components/prism-java.min.js"
import "prismjs/components/prism-sql.min.js"
import "prismjs/components/prism-docker.min.js"
import "prismjs/components/prism-yaml.min.js"
import "prismjs/components/prism-v.min.js"
import "prismjs/components/prism-jsx.min.js"
import "prismjs/components/prism-css.min.js"
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';

@Component({
  selector: 'app-conversation',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MarkdownModule, FontAwesomeModule],
  templateUrl: './conversation.component.html',
  styles: ``
})
export class ConversationComponent implements AfterViewChecked{

  currDate: Date = new Date();
  @ViewChild("scrollMe") scrollContainer!: ElementRef;
  @ViewChild('myTextArea') myTextArea!: ElementRef;

  public options: KatexOptions = {
    displayMode: true,
    throwOnError: false,
    errorColor: 'red',
  };

  readonly clipBoardButton = CopyToClipboardComponent;

  response: string = "";
  allChats : Content[] = [];
  sendPaperPlane = FAIcons.PAPER_PLANE;

  sendChat: Conversation = {
    contents: this.allChats
  };

  isLoading: boolean = false;

  inputForm: FormGroup = new FormGroup({
    inputText: new FormControl("", [Validators.required, Validators.minLength(4)]),
  });


  constructor(public service: GenAIService, private toaster: ToastrService) {
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  // this will keep chat the end by scrolling if any things occurred on the view
  scrollToBottom() {
      const el: HTMLDivElement = this.scrollContainer.nativeElement;
      el.scrollTop = el.scrollHeight;
      // el.scrollTop = Math.max(0, el.scrollHeight - el.offsetHeight);
  }


  // for auto adjusting textarea heights
  onInput(event: Event) {
    const textArea = event.target as HTMLTextAreaElement;
    const borderBoxHeight = (textArea.offsetHeight - textArea.clientHeight) + 'px';
    textArea.style.height = 'auto';
    textArea.style.height = (textArea.scrollHeight + parseInt(borderBoxHeight)) + 'px';
  }

  getAnswer() {
    if (this.inputForm.valid) {
      const data = { role: "user", parts: [{ text: this.inputForm.value.inputText }] } as Content;
      this.allChats.push(data);
      this.service.formSubmitted = true; // making flag true
      this.isLoading = true;
      this.clearAll();
      this.service.conversationAndAnswer(this.sendChat)
        .subscribe(
          {
            next: res => {
              this.response = res.data.responseMessage;
              // fill the chat
              const mod = { role: "model", parts: [{ text: res.data.responseMessage }] } as Content;
              this.allChats.push(mod);
              this.isLoading = false;
              this.toaster.success("Here is your response", "Response")
            },
            error: err => {
              this.isLoading = false;
            }
          }
        );
    }
  }

  clearAll() {
    this.inputForm.reset();
  }
}
