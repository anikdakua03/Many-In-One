import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class TextOnlyService {

  constructor(private http: HttpClient) { }

  url: string = environment.apiBaseUrl + 'GenAI/TextOnly';

  formSubmitted: boolean = false;

  response: any = "";


  // askes and get some response
  askQuestion(askQuery: object): Observable<any> {
    this.formSubmitted = true;

    return this.http.post<any>(this.url, askQuery);
  }
}
