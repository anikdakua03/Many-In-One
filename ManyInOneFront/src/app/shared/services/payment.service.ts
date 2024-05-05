import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PaymentDetail } from '../models/payment-detail.model';
import { ApiResponse } from '../interfaces/api-response';

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
  refreshList()  {
    this.http.get<ApiResponse<PaymentDetail[]>>(this.url, {withCredentials : true}).subscribe({
      next : res => {
        this.list = res.data as PaymentDetail[];
      }
    });
  }

  // add payment details
  addPaymentDetails(paymentForm: object): Observable<ApiResponse<PaymentDetail>> {
    return this.http.post<ApiResponse<PaymentDetail>>(this.url, paymentForm, { withCredentials: true });
  }


  // update payment details
  updatePaymentDetails(paymentForm: FormGroup): Observable<ApiResponse<PaymentDetail>> {
    return this.http.put<ApiResponse<PaymentDetail>>(this.url + '/' + paymentForm.value.paymentDetailId, paymentForm.value, {withCredentials : true});
  }

  // delete payment details
  deletePaymentDetails(id: number): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(this.url + '/' + id, { withCredentials : true }); 
  }

  // resetting the form after successful addition
  resetForm(form: FormGroup) {
    form.reset();
    this.formData = new PaymentDetail(); // also resetting the PaymentDetail object
    this.formSubmitted = false;
  }
}
