import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { PaymentDetail } from '../models/payment-detail.model';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {

  constructor(private http: HttpClient) { }

  url: string = environment.apiBaseUrl + 'Payment';
  list: PaymentDetail[] = [];
  formData: any = new PaymentDetail();
  formSubmitted: boolean = false;
  paymentFormToUpdate: any = "";

  // refresh the details shown on page whenever any update occurs 
  refreshList() {
    this.http.get(this.url, {withCredentials : true}).subscribe(
      {
        next: res => {
          
          this.list = res as PaymentDetail[]; // storing the res in list as PaymentDetails arr
        },
        error: err => { console.log(err); }
      }
    );
  }

  // add payment details
  addPaymentDetails(paymentForm: object): Observable<any> {
    return this.http.post(this.url, paymentForm, { responseType: 'text', withCredentials: true });
  }


  // update payment details
  updatePaymentDetails(paymentForm: FormGroup): Observable<any> {

    return this.http.put(this.url + '/' + paymentForm.value.paymentDetailId, paymentForm.value, {withCredentials : true});
  }

  // delete payment details
  deletePaymentDetails(id: number): Observable<any> {

    return this.http.delete(this.url + '/' + id, { responseType: 'text', withCredentials : true }); 
  }

  // resetting the form after successful addition
  resetForm(form: FormGroup) {
    form.reset();
    this.formData = new PaymentDetail(); // also resetting the PaymnetDeatil object
    this.formSubmitted = false;
  }
}
