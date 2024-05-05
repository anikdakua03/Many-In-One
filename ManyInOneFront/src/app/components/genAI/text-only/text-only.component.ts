import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { RouterLink } from '@angular/router';
import { KatexOptions, MarkdownModule } from 'ngx-markdown';
import "prismjs"
import "prismjs/plugins/line-numbers/prism-line-numbers.js"
import "prismjs/plugins/line-highlight/prism-line-highlight.js"
import "prismjs/components/prism-csharp.min.js"
import "prismjs/components/prism-typescript.min.js"
import "prismjs/components/prism-javascript.min.js"
// import "prismjs/components/prism-cpp.js" // has problem
import "prismjs/components/prism-docker.min.js"
import "prismjs/components/prism-c.min.js"
import "prismjs/components/prism-python.min.js"
import "prismjs/components/prism-java.min.js"
import "prismjs/components/prism-sql.min.js"
import "prismjs/components/prism-yaml.min.js"
import "prismjs/components/prism-v.min.js"
import "prismjs/components/prism-jsx.min.js"
import "prismjs/components/prism-css.min.js"
import { CopyToClipboardComponent } from '../../copy-to-clipboard/copy-to-clipboard.component';
import { GenAIService } from '../../../shared/services/gen-ai.service';


@Component({
  selector: 'app-text-only',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, MarkdownModule],
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

  readonly clipBoardButton = CopyToClipboardComponent;

  response: string = "";
  // response2: string = "**Step 1: Convert Each Number to Decimal**\n\n* Convert each binary number to decimal using the following formula:\n```\nDecimal = a * 2^n + b * 2^(n-1) + ... + z * 2^0\n```\nwhere:\n    * a, b, ..., z are the binary digits\n    * n is the number of digits in the binary number\n\n**Step 2: Add the Decimal Numbers**\n\n* Add the decimal numbers obtained in Step 1.\n\n**Step 3: Convert the Sum Back to Binary**\n\n* Convert the sum obtained in Step 2 back to binary using the following steps:\n    1. Divide the sum by 2 and record the remainder (0 or 1).\n    2. Divide the quotient by 2 and record the remainder.\n    3. Repeat Step 2.2 until the quotient becomes 0.\n    4. The remainders, in reverse order, represent the binary digits of the sum.\n\n**Example:**\n\n**Problem:** Find the sum of 10110 (binary) and 11001 (binary).\n\n**Solution:**\n\n* **Step 1: Convert to Decimal**\n    * 10110 (binary) = 1 * 2^4 + 0 * 2^3 + 1 * 2^2 + 1 * 2^1 + 0 * 2^0 = 22\n    * 11001 (binary) = 1 * 2^4 + 1 * 2^3 + 0 * 2^2 + 0 * 2^1 + 1 * 2^0 = 25\n\n* **Step 2: Add Decimal Numbers**\n    * 22 + 25 = 47\n\n* **Step 3: Convert Sum Back to Binary**\n    * 47 / 2 = 23 (Remainder: 1)\n    * 23 / 2 = 11 (Remainder: 0)\n    * 11 / 2 = 5 (Remainder: 1)\n    * 5 / 2 = 2 (Remainder: 0)\n    * 2 / 2 = 1 (Remainder: 0)\n    * 1 / 2 = 0 (Remainder: 1)\n    * Binary sum: 10111 (reversed: 11101)\n\nTherefore, the sum of 10110 (binary) and 11001 (binary) is **11101 (binary)**.";
  // mathEq: string = `
  // 1. **Gaussian Integral:**\n$$\\int_{-\\infty}^{\\infty}e^{-x^2}dx = \\sqrt{\\pi}$$\n2. **Stirling's Formula:**\n$$n! \\approx \\sqrt{2\\pi n}\\left(\\frac{n}{e}\\right)^n$$\n3. **Euler's Formula:**\n$$e^{ix} = \\cos x + i\\sin x$$\n4. **Black-Scholes Equation:**\n$$\\frac{\\partial V}{\\partial t} + \\frac{1}{2}\\sigma^2 S^2\\frac{\\partial^2 V}{\\partial S^2} + rS\\frac{\\partial V}{\\partial S} - rV = 0$$\n5. **Navier-Stokes Equations:**\n$$\\rho\\left(\\frac{\\partial \\mathbf{u}}{\\partial t} + \\mathbf{u}\\cdot\\nabla\\mathbf{u}\\right) = -\\nabla p + \\mu\\nabla^2\\mathbf{u}+\\mathbf{f}$$\n6. **Maxwell's Equations:**\n$$\\nabla\\times\\mathbf{E} = -\\frac{\\partial \\mathbf{B}}{\\partial t}$$\n$$\\nabla\\times\\mathbf{B} = \\mu_0\\mathbf{J} + \\mu_0\\epsilon_0\\frac{\\partial \\mathbf{E}}{\\partial t}$$\n$$\\nabla\\cdot\\mathbf{E} = \\frac{\\rho}{\\epsilon_0}$$\n$$\\nabla\\cdot\\mathbf{B} = 0$$\n7. **Einstein's Field Equations:**\n$$G_{\\mu\\nu} = 8\\pi G T_{\\mu\\nu}$$\n8. **Schrödinger Equation:**\n$$i\\hbar\\frac{\\partial \\psi}{\\partial t} = -\\frac{\\hbar^2}{2m}\\nabla^2\\psi + V(x, y, z)\\psi$$\n9. **Heat Equation:**\n$$\\frac{\\partial u}{\\partial t} = \\alpha \\nabla^2 u$$\n10. **Laplace Equation:**\n$$\\nabla^2 \\phi = 0$$`;

  inputForm: FormGroup = new FormGroup({
    inputText: new FormControl("", [Validators.required, Validators.minLength(4)]),
  });

  isLoading : boolean = false;

  constructor(public service: GenAIService, private toaster: ToastrService)
  {
  }

  getAnswer() {
    const obj = this.inputForm.value;
    if (this.inputForm.valid) {
      this.service.formSubmitted = true; // making flag true
      this.isLoading = true;
      this.service.askQuery(obj)
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

  clearAll()
  {
    this.inputForm.reset();
    this.response = "";
  }
}
