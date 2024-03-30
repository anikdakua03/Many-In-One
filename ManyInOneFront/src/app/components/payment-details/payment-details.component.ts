import { Component, OnInit } from '@angular/core';
import { PaymentService } from '../../shared/services/payment.service';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PaymentDetailsFormComponent } from './payment-details-form/payment-details-form.component';

@Component({
    selector: 'app-payment-details',
    standalone: true,
    templateUrl: './payment-details.component.html',
    styles: ``,
  imports: [RouterLink, ReactiveFormsModule, PaymentDetailsFormComponent]
})
export class PaymentDetailsComponent implements OnInit {

  isOpen: boolean = false;
  isLoading : boolean = false;

  updateForm: FormGroup = new FormGroup({
    paymentDetailId: new FormControl(0),
    cardOwnerName: new FormControl("", [Validators.required, Validators.minLength(4), Validators.maxLength(50)]),
    cardNumber: new FormControl("", [Validators.required, Validators.minLength(16), Validators.maxLength(16)]),
    securityCode: new FormControl("", [Validators.required, Validators.minLength(3), Validators.maxLength(3)]),
    expirationDate: new FormControl("", [Validators.required, Validators.pattern(/^(0[1-9]|1[0-2])\/\d{2}$/)]) // for like MM/YY [valid month]
  });

  constructor(public service: PaymentService, private router: Router, private toaster: ToastrService) {

  }

  ngOnInit(): void {
    this.service.refreshList();
  }


  populateForm(selectedRecord: any) {
    this.updateForm.patchValue(selectedRecord);
  }


  openPop(oldDetails: any) {
    this.isOpen = true;
    this.populateForm(oldDetails);
  }

  closePop() {
    this.isOpen = false;
  }

  onSubmit() {
    if (this.updateForm.valid) {
      this.isLoading = true;
      this.service.updatePaymentDetails(this.updateForm).subscribe({
        next: res => {
          this.service.refreshList();
          this.isLoading = false;
          this.toaster.info("Payment method updated successfully !", "Payment Detail updated");
        },
        error: err => {
        }
      });
      this.updateForm.reset();
    }
    else {
      this.isLoading = false;
      this.updateForm.markAllAsTouched();
    }
  }

  onDelete(id: number) {
    // before deleting need to confirm from user
    if (confirm("Do you want to remove this payment detail ??")) {
      this.isLoading = true;
      this.service.deletePaymentDetails(id)
        .subscribe(
          {
            next: res => {
              this.isLoading = false;
              this.toaster.error("Payment detail deleted successfully !", "Payment Detail Delete");
              this.service.refreshList();
            },
          });
    }
  }
}
