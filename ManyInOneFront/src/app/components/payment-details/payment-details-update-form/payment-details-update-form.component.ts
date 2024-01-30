import { Component } from '@angular/core';
import { PaymentService } from '../../../shared/services/payment.service';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-payment-details-update-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './payment-details-update-form.component.html',
  styles: ``
})
export class PaymentDetailsUpdateFormComponent {

  updateForm: FormGroup = new FormGroup({
    paymentDetailId: new FormControl(0),
    cardOwnerName: new FormControl("", [Validators.required, Validators.minLength(4), Validators.maxLength(50)]),
    cardNumber: new FormControl("", [Validators.required, Validators.minLength(16), Validators.maxLength(16)]),
    securityCode: new FormControl("", [Validators.required, Validators.minLength(3), Validators.maxLength(3)]),
    expirationDate: new FormControl("", [Validators.required, Validators.minLength(5), Validators.maxLength(5)])
  });

  constructor(public service: PaymentService, private router: Router, private toaster: ToastrService) {

  }
  ngOnInit(): void {
    // wherever this page is called it will get all the details first
    this.getAllPaymentDetails();
  }
  
  onSubmit() {
    console.log("After clicking update details", this.updateForm.value);

    // call update from service
    this.service.updatePaymentDetails(this.updateForm).subscribe({
      next: res => {
        // console.log("Response --> ", res);
        // for updating the list
        this.service.refreshList();
        this.toaster.info("Payment method updated successfully !", "Payment Detail updated");
      },
      error: err => {
        console.log(err);
      }
    });

    // route to details page of pament
    this.router.navigateByUrl("/paymentdetails");
  }

  getAllPaymentDetails() {
    this.updateForm.patchValue(this.service.paymentFormToUpdate);
  }
}
