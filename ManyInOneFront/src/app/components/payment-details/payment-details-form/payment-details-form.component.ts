import { Component } from '@angular/core';
import { PaymentService } from '../../../shared/services/payment.service';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';

@Component({
  selector: 'app-payment-details-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, FontAwesomeModule],
  templateUrl: './payment-details-form.component.html',
  styles: ``
})
export class PaymentDetailsFormComponent {

  isLoading: boolean = false;
  dots = FAIcons.ELLIPSES;

  paymentForm: FormGroup = new FormGroup({
    paymentDetailId: new FormControl(0),
    cardOwnerName: new FormControl("", [Validators.required, Validators.minLength(4), Validators.maxLength(50)]),
    cardNumber: new FormControl("", [Validators.required, Validators.minLength(16), Validators.maxLength(16)]),
    securityCode: new FormControl("", [Validators.required, Validators.minLength(3), Validators.maxLength(3)]),
    expirationDate: new FormControl("", [Validators.required, Validators.pattern(/^(0[1-9]|1[0-2])\/\d{2}$/)]) // for like MM/YY [valid month]
  });

  constructor(public service: PaymentService, private toaster: ToastrService) {

  }

  // resetting the form after successful addition
  resetForm() {
    this.paymentForm.reset();
    this.service.formSubmitted = false;
  }

  onSubmit() {
    if (this.paymentForm.valid) {
      this.service.formSubmitted = true; // making flag true
      // we are inserting if details id == 0 , means fresh
      if (this.service.formData.paymentDetailId === 0) {
        this.insertRecord();
      }
      // otherwise it is a existing so , updating that
      else {
        this.updateRecord();
      }

    }
    else {
      this.toaster.error("Fill details correctly !", "Payment Detail Register", { tapToDismiss: true });
    }
  }

  // when adding  records
  insertRecord() {
    this.isLoading = true;
    const obj = this.paymentForm.value;
    delete this.paymentForm.value.paymentDetailId; // Remove the property
    this.service.addPaymentDetails(obj)
      .subscribe(
        {
          next: res => {
            this.isLoading = false;
            // resetting the form also
            this.resetForm();
            // toaster success message
            this.toaster.success("Payment method added successfully !", "Payment Detail Register");

            // for updating the lsit
            this.service.refreshList();

          },
          error: err => {
            this.isLoading = false;
            this.toaster.error("Payment method failed to add!", "Payment Detail Registration Failed !!");
          }
        });
  }

  // updating records
  updateRecord() {
    const obj = this.paymentForm.value;
    this.isLoading = true;
    this.service.updatePaymentDetails(obj)
      .subscribe(
        {
          next: res => {
            this.isLoading = false;
            if (res.isSuccess) {
              // resetting the form also
              this.resetForm();
              // toaster success message
              this.toaster.info("Payment method updated successfully !", "Payment Detail Update");

              // for updating the list
              this.service.refreshList();
            }
            else {
              // resetting the form also
              this.resetForm();
              // toaster success message
              this.toaster.error(res.error.description, "Payment Detail Update");
            }
          },
        });
  }
}
