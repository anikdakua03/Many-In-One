import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResponse } from '../interfaces/api-response';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { QuizQuestionResponse } from '../interfaces/Quizz/quiz-question-response';
import { IAllCategory } from '../interfaces/Quizz/all-category';
import { IAllQuestion } from '../interfaces/Quizz/all-question';
import { QuizResultResponse } from '../interfaces/Quizz/quiz-score';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class QuizzService {
  baseUrl: string = environment.apiBaseUrl + 'Quiz';

  
  allCategory$ : BehaviorSubject<IAllCategory> = new  BehaviorSubject<IAllCategory>(null!);
  allExtCategory$ : BehaviorSubject<any> = new  BehaviorSubject<any>(null!);
  allQss$ : BehaviorSubject<IAllQuestion[]> = new  BehaviorSubject<IAllQuestion[]>([]);
  quizQss$ : BehaviorSubject<QuizQuestionResponse[]> = new  BehaviorSubject<QuizQuestionResponse[]>([]);
  
  
  constructor(private http: HttpClient, private toaster: ToastrService) {
  }

  // get custom categories
  getOwnCategories() {
    this.http.get<ApiResponse<IAllCategory>>(this.baseUrl +'/GetQuizCategories', { withCredentials: true }).subscribe({
      next : res =>  {
        this.allCategory$.next(res.data);
      },
      error : err => {
        this.toaster.error("Currently not available right now, please try again in few minutes.", " Some Error Occurred.");
      }
    });
  }

  // get all questions
  getAllQuestions() {
    this.http.get<ApiResponse<IAllQuestion[]>>(this.baseUrl +'/GetAllQuestions', { withCredentials: true }).subscribe({
      next : res =>  {
        this.allQss$.next(res.data);
      },
      error : err => {
        this.toaster.error("Currently not available right now, please try again in few minutes.", " Some Error Occurred.");
      }
    });
  }

  // add question
  addQuestion(qsObject : object): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(this.baseUrl +'/AddQuestion', qsObject, { withCredentials: true });
  }

  // add multiple  question
  addMultipleQuestion(qsFromData : object): Observable<ApiResponse<string>> {
    let header = new HttpHeaders();
    header.append('Content-Type', 'multipart/form-data');

    return this.http.post<ApiResponse<string>>(this.baseUrl +'/AddQuestionsFromExcel', qsFromData, { headers : header, withCredentials: true });
  }

  // update question
  updateQuestion(updatedQsObject : object): Observable<ApiResponse<string>> {
    return this.http.put<ApiResponse<string>>(this.baseUrl +'/UpdateQuestion', updatedQsObject, { withCredentials: true });
  }

  // delete  question
  deleteQuestion(categoryId : string): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(this.baseUrl +'/RemoveQuestion/' + categoryId, { withCredentials: true });
  }

  // add custom categories
  addCategory(categoryObject : object): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(this.baseUrl +'/AddCategory', categoryObject, { withCredentials: true });
  }

  // add custom categories
  updateCategory(categoryObject : object): Observable<ApiResponse<string>> {
    return this.http.put<ApiResponse<string>>(this.baseUrl +'/UpdateCategory', categoryObject, { withCredentials: true });
  }

  // add custom categories
  deleteCategory(categoryId : string): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(this.baseUrl +'/RemoveCategory/' + categoryId, { withCredentials: true });
  }

  // get other categories
  getOtherCategories() {
    this.http.get<ApiResponse<any>>(this.baseUrl + '/GetQuizCategoriesFromAPI', { withCredentials: true }).subscribe({
      next: res => {
        this.allExtCategory$.next(res.data.trivia_categories);
      },
      error: err => {
        this.toaster.error("Currently not available right now, please try again in few minutes.", " Some Error Occurred.");
      }
    });
  }
  
  // get quiz questions
  makeQuizFromOwn(reqBody: object): Observable<ApiResponse<QuizQuestionResponse[]>> {
    return this.http.post<ApiResponse<QuizQuestionResponse[]>>(this.baseUrl +'/QuizMaker', reqBody, { withCredentials: true });
  }

  // from external public api available
  makeQuizFromPublic(reqBody: object): Observable<ApiResponse<QuizQuestionResponse[]>> {
    return this.http.post<ApiResponse<QuizQuestionResponse[]>>(this.baseUrl +'/GetQuizQssFromAPI', reqBody, { withCredentials: true });
  }

  // get quiz score
  getQuizScore(reqBody: object): Observable<ApiResponse<QuizResultResponse>> {
    return this.http.post<ApiResponse<QuizResultResponse>>(this.baseUrl +'/GetQuizResult', reqBody, { withCredentials: true });
  }

}
