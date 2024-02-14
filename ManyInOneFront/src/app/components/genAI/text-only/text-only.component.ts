import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { TextOnlyService } from '../../../shared/services/text-only.service';
import { RouterLink } from '@angular/router';
import { KatexOptions, MarkdownModule } from 'ngx-markdown';
import "prismjs"
import "prismjs/plugins/line-numbers/prism-line-numbers.js"
import "prismjs/plugins/line-highlight/prism-line-highlight.js"
import "prismjs/components/prism-csharp.min.js"
import "prismjs/components/prism-typescript.min.js"
import "prismjs/components/prism-javascript.min.js"
// import "prismjs/components/prism-cpp.js" // has problem
import "prismjs/components/prism-c.min.js"
import "prismjs/components/prism-python.min.js"
import "prismjs/components/prism-java.min.js"
import "prismjs/components/prism-sql.min.js"
import "prismjs/components/prism-yaml.min.js"
import "prismjs/components/prism-v.min.js"
import "prismjs/components/prism-jsx.min.js"
import "prismjs/components/prism-css.min.js"
import { NgxLoadingModule } from 'ngx-loading';


@Component({
  selector: 'app-text-only',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, MarkdownModule, NgxLoadingModule],
  templateUrl: './text-only.component.html',
  styles: ``
})
export class TextOnlyComponent {

  public options: KatexOptions = {
    displayMode: true,
    throwOnError: false,
    errorColor: 'red',
    // delimiters: [...],
  };

  response: any = "";
  // resposne2: string = "```csharp\n// Function to check if a string is a palindrome\nbool IsPalindrome(string str)\n{\n    // Convert the string to lowercase and remove all non-alphanumeric characters\n    string cleanStr = new string(str.ToLower().Where(c => Char.IsLetterOrDigit(c)).ToArray());\n\n    // Check if the cleaned string is equal to its reverse\n    return cleanStr == ReverseString(cleanStr);\n}\n\n// Function to reverse a string\nstring ReverseString(string str)\n{\n    // Create a new character array to hold the reversed string\n    char[] reversed = new char[str.Length];\n\n    // Iterate through the string in reverse order and copy each character to the reversed array\n    for (int i = str.Length - 1, j = 0; i >= 0; i--, j++)\n    {\n        reversed[j] = str[i];\n    }\n\n    // Return the reversed string\n    return new string(reversed);\n}\n```\n\nUsage:\n\n```c#\nstring input = \"racecar\";\nbool isPalindrome = IsPalindrome(input);\n\nif (isPalindrome)\n{\n    Console.WriteLine($\"{input} is a palindrome.\");\n}\nelse\n{\n    Console.WriteLine($\"{input} is not a palindrome.\");\n}\n```";
  // re : string = "The derivative of $1 + \\tan \\theta$ with respect to $\\theta$ can be calculated using the sum rule and the derivative of $\\tan \\theta$.\n\nThe sum rule states that the derivative of a sum of functions is equal to the sum of the derivatives of the individual functions. Therefore,\n\n$$\\frac{d}{d\\theta}(1 + \\tan \\theta) = \\frac{d}{d\\theta}(1) + \\frac{d}{d\\theta}(\\tan \\theta)$$\n\nThe derivative of a constant function is always zero, so $\\frac{d}{d\\theta}(1) = 0$.\n\nThe derivative of $\\tan \\theta$ is given by:\n\n$$\\frac{d}{d\\theta}(\\tan \\theta) = \\sec^2 \\theta$$\n\nTherefore,\n\n$$\\frac{d}{d\\theta}(1 + \\tan \\theta) = 0 + \\sec^2 \\theta = \\sec^2 \\theta$$\n\nHence, the derivative of $1 + \\tan \\theta$ with respect to $\\theta$ is $\\sec^2 \\theta$.";
  // mathEq: string = `
  // 1. **Gaussian Integral:**\n$$\\int_{-\\infty}^{\\infty}e^{-x^2}dx = \\sqrt{\\pi}$$\n2. **Stirling's Formula:**\n$$n! \\approx \\sqrt{2\\pi n}\\left(\\frac{n}{e}\\right)^n$$\n3. **Euler's Formula:**\n$$e^{ix} = \\cos x + i\\sin x$$\n4. **Black-Scholes Equation:**\n$$\\frac{\\partial V}{\\partial t} + \\frac{1}{2}\\sigma^2 S^2\\frac{\\partial^2 V}{\\partial S^2} + rS\\frac{\\partial V}{\\partial S} - rV = 0$$\n5. **Navier-Stokes Equations:**\n$$\\rho\\left(\\frac{\\partial \\mathbf{u}}{\\partial t} + \\mathbf{u}\\cdot\\nabla\\mathbf{u}\\right) = -\\nabla p + \\mu\\nabla^2\\mathbf{u}+\\mathbf{f}$$\n6. **Maxwell's Equations:**\n$$\\nabla\\times\\mathbf{E} = -\\frac{\\partial \\mathbf{B}}{\\partial t}$$\n$$\\nabla\\times\\mathbf{B} = \\mu_0\\mathbf{J} + \\mu_0\\epsilon_0\\frac{\\partial \\mathbf{E}}{\\partial t}$$\n$$\\nabla\\cdot\\mathbf{E} = \\frac{\\rho}{\\epsilon_0}$$\n$$\\nabla\\cdot\\mathbf{B} = 0$$\n7. **Einstein's Field Equations:**\n$$G_{\\mu\\nu} = 8\\pi G T_{\\mu\\nu}$$\n8. **Schrödinger Equation:**\n$$i\\hbar\\frac{\\partial \\psi}{\\partial t} = -\\frac{\\hbar^2}{2m}\\nabla^2\\psi + V(x, y, z)\\psi$$\n9. **Heat Equation:**\n$$\\frac{\\partial u}{\\partial t} = \\alpha \\nabla^2 u$$\n10. **Laplace Equation:**\n$$\\nabla^2 \\phi = 0$$`;

  inputForm: FormGroup = new FormGroup({
    inputText: new FormControl("", [Validators.required, Validators.minLength(4)]),
  });

  isLoading : boolean = false;

  constructor(public service: TextOnlyService, private toaster: ToastrService)
  {
  }

  getAnswer() {
    const obj = this.inputForm.value;
    console.log(" Your query is componeent --> " + this.inputForm.value.inputText);
    debugger
    if (this.inputForm.valid) {
      this.service.formSubmitted = true; // making flag true
      this.isLoading = true;
      this.service.askQuestion(obj)
        .subscribe(
          {
            next: res => {
              // console.log("From text compo ---> " , res.responseMessage);
              // this.loader?.showLoader(true);
              this.response = res.responseMessage;
              this.isLoading = false;
              this.toaster.success("Here is your response", "Response")
              // this.loader = false;
              // this.loader?.showLoader(false);
            },
            error: err => {
              this.isLoading = false;
              console.log("This is an error --> ", err.error.title);
              // this.loader = false;
              // this.toaster.show(err.error.title);
            }
          }
        );
    }
    else {
      this.toaster.error("Please ask your query !!", " Error !!");
    }
  }

  clearAll()
  {
    this.inputForm.reset();
    this.response = "";
  }
}
