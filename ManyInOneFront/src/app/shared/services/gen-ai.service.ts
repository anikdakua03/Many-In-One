import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GenAIResponse } from '../interfaces/gen-ai-response';
import { ApiResponse } from '../interfaces/api-response';

@Injectable({
  providedIn: 'root'
})
export class GenAIService {

  baseUrl: string = environment.apiBaseUrl;
  formSubmitted: boolean = false;

  constructor(private http: HttpClient) { }


  // asks query and get some response text
  askQuery(askQuery: object): Observable<ApiResponse<GenAIResponse>> {
    this.formSubmitted = true;
    return this.http.post<ApiResponse<GenAIResponse>>(this.baseUrl + 'GenAI/TextOnly', askQuery, { withCredentials: true });
  }

  // asks query related to the uploaded image and get some response text
  askQuestion(askQuery: any): Observable<ApiResponse<GenAIResponse>> {
    this.formSubmitted = true;
    return this.http.post<ApiResponse<GenAIResponse>>(this.baseUrl + 'GenAI/TextAndImage', askQuery, { withCredentials: true }); 
  }

  // asks long query and get summarized response text
  askToSummarize(askQuery: object): Observable<ApiResponse<GenAIResponse>> {
    this.formSubmitted = true;
    return this.http.post<ApiResponse<GenAIResponse>>(this.baseUrl + 'GenAI/TextSummarize', askQuery, { withCredentials: true });
  }

  // gives description and gets generated image
  generateImage(askQuery: object): Observable<ApiResponse<GenAIResponse>> {
    this.formSubmitted = true;
    return this.http.post<ApiResponse<GenAIResponse>>(this.baseUrl + 'GenAI/TextToImage', askQuery, { withCredentials: true });
  }

  // conversation and answers based on also previous history of the conversations
  conversationAndAnswer(askQuery: object): Observable<ApiResponse<GenAIResponse>> {
    this.formSubmitted = true;
    return this.http.post<ApiResponse<GenAIResponse>>(this.baseUrl + 'GenAI/MultiConversation', askQuery, { withCredentials: true });
  }
  
  // conversation and answers based on also previous history of the conversations
  transformTextToSpeech(askQuery: object): Observable<ApiResponse<GenAIResponse>> {
    this.formSubmitted = true;
    return this.http.post<ApiResponse<GenAIResponse>>(this.baseUrl + 'GenAI/TextToSpeech', askQuery, { withCredentials: true });
  }
}
