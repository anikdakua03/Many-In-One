import { Component } from '@angular/core';
import { PaymentService } from '../../shared/services/payment.service';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
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
export class PaymentDetailsComponent {

  // filterForm: FormGroup = new FormGroup({
  //   searchText: new FormControl<string>('')
  // });
  // filterFormSubsription: Subscription;
  searchText: string = '';

  constructor(public service: PaymentService, private router: Router, private toaster: ToastrService) {

  }

  ngOnInit(): void {
    // this.loader?.showLoader(true);
    this.service.refreshList();
    // this.loader?.showLoader(false);
  }


  populateForm(selectedRecord: any) {

    this.service.paymentFormToUpdate = selectedRecord;
    console.log("Still on payment -->", this.service.paymentFormToUpdate);
    // to update form component
    this.router.navigateByUrl("/update-paymentdetails");

    // const obj = this.service.formData;
    // console.log(obj);
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
