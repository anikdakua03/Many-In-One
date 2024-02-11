import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TextAndImageOnlyService {

  constructor(private http: HttpClient) { }

  url: string = environment.apiBaseUrl + 'GenAI/TextAndImage';

  formSubmitted: boolean = false;

  response: any = "";


  // askes and get some response
  askQuestion(askQuery: any): Observable<any> {
    this.formSubmitted = true;

    return this.http.post<any>(this.url, askQuery, {  withCredentials: true }); //, { headers } automatically sets its appropiate headers
  }
}
