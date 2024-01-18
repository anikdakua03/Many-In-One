import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TextAndImageOnlyService } from '../../../shared/services/text-and-image-only.service';
import { ToastrService } from 'ngx-toastr';
import { RouterLink } from '@angular/router';
import { KatexOptions, MarkdownComponent } from 'ngx-markdown';

@Component({
  selector: 'app-text-and-image-only',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, MarkdownComponent],
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
  // response: any = "";
  response: any = "Classical bits can only be either 0 or 1, while quantum bits (qubits) can be 0, 1, or both at the same time. This is called superposition.\n\nQubit can be represented in many ways, such as the spin of an electron, the polarization of a photon, or the energy level of an atom.\n\nThe state of a qubit is not known until it is measured. When a qubit is measured, it collapses into a classical bit, either 0 or 1.\n\nQubits are used in quantum computers, which are much more powerful than classical computers. Quantum computers can solve some problems that are impossible for classical computers to solve.";

  res: any = "// C# program for Fibonacci Series \n\n// using Space Optimized Method\n\nusing System;\nn\nnamespace Fib {\n\n  public class GFG {\n\n    static int Fib(int n) {\n\n		int a = 0, b = 1, c = 0;\n\n\      // To return the first Fibonacci number\n\n      if (n == 0)\n\n        return a;\n\n\      for (int i = 2; i <= n; i++) {\n\n        c = a + b;\n\n        a = b;\n\n        b = c;\n\n      }\n\n\      return b;\n\n    }\\n\n    // Driver function\n\n    public static void Main(string[] args) {\n\n\		int n ";//= 9;\n\n      Console.Write("\\0} ", Fib(n));\n\n    }\n\n  }\n\n}\n\n\n\n// This code is contributed by Sam007.";

  res2: any = "sin(a+b)=sin(a)cos(b)+sin(b)cos(a)and\n\ncos(a+b)=cos(a)cos(b)–sin(a)sin(b)\n\n= cos(kΘ + Θ) + isin(kΘ + Θ)\n\n=cos((k + 1)Θ) + isin((k + 1)Θ)";

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

    console.log("New form data ==> ", formData);

    this.service.askQuestion(formData).subscribe(
      {
        next: res => {
          console.log("From text compo ---> ", res.responseMessage);

          // this.loader?.showLoader(true);
          this.response = res.responseMessage;
          // this.loader = false;
          // this.loader?.showLoader(false);
        },
        error: err => {
          console.log("This is error --> ", err.error);
          // this.loader = false;
          this.toaster.show(err.error);
        }
      }
    );
  }
}
