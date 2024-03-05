import { Component, OnInit } from '@angular/core';
import { PaymentService } from '../../shared/services/payment.service';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PaymentDetailsFormComponent } from './payment-details-form/payment-details-form.component';
import { PaymentDetailsUpdateFormComponent } from './payment-details-update-form/payment-details-update-form.component';
import { TableSearchFilterPipe } from "../../shared/constants/table-search-filter.pipe";

@Component({
    selector: 'app-payment-details',
    standalone: true,
    templateUrl: './payment-details.component.html',
    styles: ``,
    imports: [RouterLink, ReactiveFormsModule, PaymentDetailsFormComponent, PaymentDetailsUpdateFormComponent, TableSearchFilterPipe]
})
export class PaymentDetailsComponent implements OnInit {

  // filterForm: FormGroup = new FormGroup({
  //   searchText: new FormControl<string>('')
  // });
  // filterFormSubsription: Subscription;
  searchText: string = '';
  oldForm: any = "";
  // pop up testing
  isOpen: boolean = false;

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
    // this.loader?.showLoader(true);
    this.service.refreshList();
    // this.loader?.showLoader(false);
  }


  populateForm(selectedRecord: any) {

    this.updateForm.patchValue(selectedRecord);
  }

  // pop up testing
  openPop(oldDetails: any) {
    this.isOpen = true;
    this.populateForm(oldDetails);
  }

  closePop() {
    this.isOpen = false;
  }

  onSubmit() {
    this.service.updatePaymentDetails(this.updateForm).subscribe({
      next: res => {
        this.service.refreshList();
        this.toaster.info("Payment method updated successfully !", "Payment Detail updated");
      },
      error: err => {
        console.log(err);
      }
    });
  }

  onDelete(id: any) {
    // before deleting need to confirm from user
    if (confirm("Do you want to remove this payment detail method ??")) {
      // console.log("For deleteing from compo --> ",id);
      this.service.deletePaymentDetails(id)
        .subscribe(
          {
            next: res => {
              // alert(res);
              // toaster success message
              this.toaster.error("Payment method deleted successfully !", "Payment Detail Delete");

              // for updating the lsit
              this.service.refreshList();
            },
            error: err => { console.log(err); }
          });
    }
  }
}
