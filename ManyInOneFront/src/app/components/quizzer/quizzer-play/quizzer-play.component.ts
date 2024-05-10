import { NgClass } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ToastrService } from 'ngx-toastr';
import { Subject, timer } from 'rxjs';
import { QuizConfig } from '../../../shared/constants/QuizConfigs';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';
import { CanDeactivateType } from '../../../shared/guards/quiz-leaving.guard';
import { IOptionResponse, QuizQuestionResponse } from '../../../shared/interfaces/Quizz/quiz-question-response';
import { QuizResultResponse } from '../../../shared/interfaces/Quizz/quiz-score';
import { IUserAnswer, IUserQuizAnswer } from '../../../shared/interfaces/Quizz/user-quiz-answer';
import { CryptingService } from '../../../shared/services/crypting.service';
import { QuizzService } from '../../../shared/services/quizz.service';
import { KatexOptions, MarkdownModule } from 'ngx-markdown';

@Component({
  selector: 'app-quizzer-play',
  standalone: true,
  imports: [FontAwesomeModule, NgClass, MarkdownModule],
  templateUrl: './quizzer-play.component.html',
  styles: ``
})
export class QuizzerPlayComponent implements OnInit {

  public options: KatexOptions = {
    displayMode: true,
    throwOnError: false,
    errorColor: 'red',
  };
  
  q = FAIcons.Q;
  stopwatch = FAIcons.STW;
  leftArr = FAIcons.LONG_LEFT_ARROW;
  rightArr = FAIcons.LONG_RIGHT_ARROW;
  close = FAIcons.CLOSE;
  tick = FAIcons.TICK;
  dots = FAIcons.ELLIPSES;

  isQuizStarted: boolean = false;
  openQuizScore: boolean = false;
  isLoading: boolean = false;

  allQsLevels: { id: string, value: string }[] = QuizConfig.allQsLevels;

  allQsTypes: { id: string, value: string }[] = QuizConfig.allQsTypes;

  questionList: QuizQuestionResponse[] = [];

  currentQuestion: number = 0;

  userQuizScore: number = 0;

  totalTimeTaken: number = 0;

  quizScore: QuizResultResponse = {
    totalQs: 0,
    totalCorrect: 0,
    totalScore: 0,
    totalTime: "",
    percentage: 0,
    hasPassed: false,
    results: []
  };

  seconds: number = 0;
  minutes: number = 0;
  hours: number = 0;
  // intervalId: any = null;
  timer$: any;

  userAnswers = new Map<string, { answer: Set<string>, opId: Set<string> }>();

  constructor(private cryptingService: CryptingService, private quizService: QuizzService, private router: Router, private toaster: ToastrService) {

  }

  ngOnInit(): void {
    this.isQuizStarted = true;
    this.startTimer();

    // get those quiz question from service
    const qss = this.quizService.quizQss$.getValue();

    if (qss.length < 5) {
      // return to create quiz with more qs or other category
      this.router.navigateByUrl("quizzer");
      this.toaster.info("Please try other category or changing some options.", "Insufficient no. of questions.");
    }
    else {
      this.questionList = qss;
    }
  }


  ngOnDestroy() {
    this.stopTimer();
  }

  startTimer() {
    if (this.isQuizStarted) {
      // this.intervalId = setInterval(() => {
      //   this.seconds++;
      //   if (this.seconds >= 60) {
      //     this.seconds = 0;
      //     this.minutes++;
      //   }
      //   if (this.minutes >= 60) {
      //     this.minutes = 0;
      //     this.hours++;
      //   }
      // }, 1000);

      // using RxJs timer
      // will wait for 2 seconds then each 1 sec after will do operation
      this.timer$ = timer(2000, 1000).subscribe((res) => {
        // now need to break this count into hour minutes seconds
        this.seconds = Math.floor(res % 60);
        this.minutes = Math.floor(res / 60);
        this.hours = Math.floor(res / 3600);
      });

    }
  }

  stopTimer() {
    this.timer$.unsubscribe();

    // for setInterval()
    // if (this.intervalId) {
    //   clearInterval(this.intervalId);
    //   this.intervalId = null;
    // }
  }

  resetTimer() {
    this.stopTimer();
    this.seconds = 0;
    this.minutes = 0;
    this.hours = 0;
  }

  formatTime(value: number) {
    return value.toString().padStart(2, '0'); // Add leading zero for single digits
  }

  formatTotalTime(hr: number, min: number, seconds: number): string {
    if (hr <= 0) {
      return `${this.formatTime(min)} minutes ${this.formatTime(seconds)} seconds`;
    }
    return `${this.formatTime(hr)} hours ${this.formatTime(min)} minutes ${this.formatTime(seconds)} seconds`;
  }


  canDeactivate(): CanDeactivateType {
    const deactivateSubject = new Subject<boolean>();
    if (confirm('Are you sure you want to leave? Progress will be lost.')) {
      deactivateSubject.next(true);
    }
    else {
      deactivateSubject.next(false);
    }
    return deactivateSubject;
  }

  getQsTypeValue(typeId: string): string {
    const matchingType = this.allQsTypes.find(type => type.id === typeId.toLowerCase());
    return matchingType ? matchingType.value : 'Any';
  }

  getQsLevelValue(lvlId: string): string {
    const matchingType = this.allQsLevels.find(level => level.id === lvlId.toLowerCase());
    return matchingType ? matchingType.value : 'Any';
  }

  onSelectOption(qsId: string, option: IOptionResponse) {
    // on click will change selected to not , not selected to selected
    option.optionSelected = !option.optionSelected;
    // also store the answer along with it
    this.storeAnswer(qsId, option);
  }

  // for public quiz it will take answer string , for private quiz will take option for validation
  storeAnswer(questionId: string, option: IOptionResponse) {
    const a = this.userAnswers.get(questionId);
    if (option !== undefined && option.optionSelected === true) {
      if (a === undefined) {
        const an = new Set<string>().add(option.optionValue);
        const op = new Set<string>().add(option.optionId);
        this.userAnswers.set(questionId, { answer: an, opId: op });
      }
      else {
        a.answer.add(option.optionValue);
        a.opId.add(option.optionId);
      }

    }
    else if (option !== undefined && a?.opId.has("00000000-0000-0000-0000-000000000000"))
    // other deselect or remove from set 
    {
      this.userAnswers.get(questionId)?.answer.delete(option.optionValue);
      // this.userAnswers.get(questionId)?.opId.delete(option.optionId); it will be same so not need to remove
    }
    else // other deselect or remove from set
    {
      this.userAnswers.get(questionId)?.answer.delete(option.optionValue);
      this.userAnswers.get(questionId)?.opId.delete(option.optionId);
    }
  }

  onQuizSubmit() {
    this.isQuizStarted = false;
    this.stopTimer();
    this.isLoading = true;
    let forDb: Boolean = false;

    for (const qs of this.questionList) {
      const userAns = this.userAnswers.get(qs.questionId);
      if (userAns === undefined) {
        // skipped that qs
        continue;
      }
      else if (userAns.opId.has("00000000-0000-0000-0000-000000000000")) {
        // means this for api quiz
        // api quiz will have only 1 answer because it has 2 types MCQ or True / False

        // trying other way to check correct ans
        if (userAns.answer.size > 1) { // for multiple selection of a only one option correct will cause -ve
          // this.userQuizScore--;
        }
        else if (userAns.answer.size == 1 && userAns.answer.has(this.cryptingService.decryptText(qs.answer!))) {
          this.userQuizScore++;
        }
      }
      else {
        // get answer from our server
        forDb = true;
        break;
      }
    }

    if (forDb) {
      let preparedReqBody: IUserQuizAnswer = { userQsAnswers: [] };

      for (let [k, v] of this.userAnswers) {
        var c = this.userAnswers.get(k);

        if (k === undefined) { return; }

        const item: IUserAnswer = { questionId: k, selectedAnswer: [...v?.answer!.values()!] }

        preparedReqBody.userQsAnswers.push(item);
      }

      this.getCustomQuizScore(preparedReqBody);
    }
    else {
      this.isLoading = false;
      this.quizScore.totalQs = this.questionList.length;
      this.quizScore.totalCorrect = this.userQuizScore;
      this.quizScore.totalScore = this.userQuizScore;
      this.quizScore.totalTime = this.formatTotalTime(this.hours, this.minutes, this.seconds);
      this.quizScore.percentage = (this.quizScore.totalCorrect * 100) / this.quizScore.totalQs;
      this.quizScore.hasPassed = this.quizScore.percentage >= 80 ? true : false;
      this.openQuizScore = true;
      this.toaster.success("Check your score.", "Quiz Score");
      // now also reset the timer
      this.resetTimer();
    }
  }

  getCustomQuizScore(userAnswersBody: IUserQuizAnswer) {
    this.quizService.getQuizScore(userAnswersBody).subscribe({
      next: res => {
        if (res.isSuccess) {
          this.quizScore.totalQs = res.data.totalQs;
          this.quizScore.totalCorrect = res.data.totalCorrect;
          this.quizScore.totalScore = res.data.totalScore;
          this.quizScore.totalTime = this.formatTotalTime(this.hours, this.minutes, this.seconds);
          this.quizScore.percentage = res.data.percentage;
          this.quizScore.hasPassed = res.data.hasPassed;
          this.quizScore.results = res.data.results;
          this.isLoading = false;
          this.openQuizScore = true;
          // now also reset the timer
          this.resetTimer();
          this.toaster.success("Check your score.", "Quiz Score");
        }
        else {
          this.isLoading = false;
          // now also reset the timer
          this.resetTimer();
          this.toaster.error(res.error.description, "Unable to get Quiz Score Now");
        }
      },
      error: err => {
        // error service not available right now , please try later
        this.isLoading = false;
        this.toaster.error("May be service is not available right now !!", "Service Error");
      }
    }
    );
  }

  showAnswers() {
    // not implemented, TODO :
  }

  closeModal() {
    this.openQuizScore = false;
    this.router.navigate(['quizzer']);
  }

  prevQuestion() {
    this.currentQuestion--;
  }

  nextQuestion() {
    this.currentQuestion++;
  }
} 
