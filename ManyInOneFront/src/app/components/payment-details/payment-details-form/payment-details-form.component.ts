import { Component } from '@angular/core';
import { PaymentService } from '../../../shared/services/payment.service';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-payment-details-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './payment-details-form.component.html',
  styles: ``
})
export class PaymentDetailsFormComponent {

  paymentForm: FormGroup = new FormGroup({
    paymentDetailId: new FormControl(0),
    cardOwnerName: new FormControl("", [Validators.required, Validators.minLength(4), Validators.maxLength(50)]),
    cardNumber: new FormControl("", [Validators.required, Validators.minLength(16), Validators.maxLength(16)]),
    securityCode: new FormControl("", [Validators.required, Validators.minLength(3), Validators.maxLength(3)]),
    expirationDate: new FormControl("", [Validators.required, Validators.minLength(5), Validators.maxLength(5)])
  });

  constructor(public service: PaymentService, private toaster: ToastrService) //
  {

  }

  // resetting the form after successful addition
  resetForm() {
    this.paymentForm.reset();
    // console.log("Form -->", this.paymentForm);
    this.service.formSubmitted = false;
  }

  onSubmit() {
    if (this.paymentForm.valid) {
      this.service.formSubmitted = true; // making flag true
      // we are inserting if details id == 0 , mneans fresh
      if (this.service.formData.paymentDetailId === 0) {
        this.insertRecord();
      }
      // otherwise it is a exisiting so , updating that
      else {
        this.updateRecord();
      }

    }
    else {
      console.log("Fill payment details correctly !");
      this.toaster.error("Fill details correctly !", "Payment Detail Register", { tapToDismiss: true });
    }
  }

  // when adding  records
  insertRecord() {
    const obj = this.paymentForm.value;
    delete this.paymentForm.value.paymentDetailId; // Remove the property
    console.log(obj);
    debugger
    this.service.addPaymentDetails(obj)
      .subscribe(
        {
          next: res => {
            console.log("Lets check --->",res);
            // resetting the form also
            this.resetForm();
            // toaster success message
            this.toaster.success("Payment method added successfully !", "Payment Detail Register");

            // for updating the lsit
            this.service.refreshList();

          },
          error: err => {
            console.log(err.message);
            this.toaster.error(err.message, "Payment Detail Registeration Faild !!");
          }
        });
  }

  // updating records
  updateRecord() {
    const obj = this.paymentForm.value;
    // console.log("Before patching --> ", obj);

    // const obj1 = this.paymentForm.patchValue(obj);
    // console.log("After patching --> ", obj1);

    this.service.updatePaymentDetails(obj)
      .subscribe(
        {
          next: res => {
            // resetting the form also
            this.resetForm();
            // toaster success message
            this.toaster.info("Payment method updated successfully !", "Payment Detail Update");

            // for updating the lsit
            this.service.refreshList();
          },
          error: err => { console.log(err); }
        });
  }
}
