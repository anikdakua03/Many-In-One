import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ToastrService } from 'ngx-toastr';
import { FAIcons } from '../../shared/constants/font-awesome-icons';
import { IAllCategory } from '../../shared/interfaces/Quizz/all-category';
import { IQuizMakerRequest } from '../../shared/interfaces/Quizz/quiz-maker-request';
import { QuizzService } from '../../shared/services/quizz.service';

@Component({
  selector: 'app-quizzer',
  standalone: true,
  imports: [ReactiveFormsModule, FontAwesomeModule],
  templateUrl: './quizzer.component.html',
  styles: ``
})
export class QuizzerComponent implements OnInit {
  dots = FAIcons.ELLIPSES;

  quizForm!: FormGroup;
  isLoading: boolean = false;

  customCategories: IAllCategory = {
    categories: []
  };

  resFromAPI: { id: number, name: string }[] = [];

  allCategories: IAllCategory = {
    categories: []
  };

  allQsLevels: { id: string, value: string }[] = [
    { id: "any", value: "Any Level" },
    { id: "easy", value: "Easy" },
    { id: "medium", value: "Medium" },
    { id: "hard", value: "Hard" }
  ];

  allQsTypes: { id: string, value: string }[] = [
    { id: "any", value: "Any type" },
    { id: "multiple_correct", value: "Multi Correct" },
    { id: "multiple", value: "Multiple Choice" },
    { id: "boolean", value: "True / False" }
  ];

  allQsNumberCounts: number[] = [5, 10, 15, 20, 25, 30, 35, 40, 45, 50]

  constructor(protected quizService: QuizzService, private toaster: ToastrService, private router: Router) {
    // get categories
    this.quizService.getOwnCategories();

    this.quizService.allCategory$.subscribe((res) => {
      if (res !== null) {
        this.customCategories = res;
        // mix up with custom and external 
        // clear first that default
        this.customCategories.categories.forEach(a => {
          this.allCategories.categories.push(a);
        });
      }
    });

    // get external categories
    this.quizService.getOtherCategories();

    this.quizService.allExtCategory$.subscribe({
      next: res => {
        this.resFromAPI = res;
        // also from apis , add to all category
        if (res === null) {
          const currCategory = {
            categoryId: "Not available",
            categoryName: "Choose a category"
          };
          this.allCategories.categories.push(currCategory);
        }
        else {
          this.resFromAPI.forEach(a => {
            const currCategory = {
              categoryId: a.id.toString(),
              categoryName: a.name
            };
            this.allCategories.categories.push(currCategory);
          });
        }
      }
    });
  }

  ngOnInit(): void {

    // then initialize the form also
    this.quizForm = new FormGroup({
      questionCount: new FormControl(5, [Validators.required, Validators.min(5)]),
      category: new FormControl(this.allCategories.categories[0].categoryId, [Validators.required]),
      questionType: new FormControl(this.allQsTypes[0].id, [Validators.required]),
      questionLevel: new FormControl(this.allQsLevels[0].id, [Validators.required]),
    });
  }


  onQuizMaking() {
    if (this.quizForm.valid) {
      this.isLoading = true;
      if (this.quizForm.value["category"].length == 36) {
        const reqBody: IQuizMakerRequest = {
          questionCount: Number(this.quizForm.value["questionCount"]),
          categoryId: this.quizForm.value["category"],
          questionType: this.quizForm.value["questionType"],
          questionLevel: this.quizForm.value["questionLevel"]
        }

        this.quizService.makeQuizFromOwn(reqBody).subscribe({
          next: res => {
            if (res.isSuccess) {
              res.data.forEach(a => {
                a.options.forEach(b => b.optionSelected = false);
              });

              res.data.forEach((a) => {
                a.options = this.shuffle(a.options);
              });

              this.quizService.quizQss$.next(this.shuffle(res.data));
              this.router.navigateByUrl("quizzer/play");
              this.toaster.success("Quiz is starting...", "Quiz Start");
              this.isLoading = false;
            }
            else {
              // show error
              this.toaster.error(res.error.description, "Quiz Starting error");
              this.isLoading = false;
            }
          },
          error : err => {
            this.isLoading = false;
            this.toaster.error("Invalid options selected, please try again.", "Validation error.")
          }
        });
      }
      else {
        const reqBody: IQuizMakerRequest = {
          questionCount: Number(this.quizForm.value["questionCount"]),
          categoryId: Number(this.quizForm.value["category"]),
          questionType: this.quizForm.value["questionType"],
          questionLevel: this.quizForm.value["questionLevel"]
        }

        this.quizService.makeQuizFromPublic(reqBody).subscribe({
          next: res => {
            if (res.isSuccess) {
              res.data.forEach(a => {
                a.options.forEach(b => b.optionSelected = false);
              });
              res.data.forEach((a) => {
                a.options = this.shuffle(a.options);
              });

              this.toaster.success("Quiz is starting...", "Quiz Start");
              this.quizService.quizQss$.next(this.shuffle(res.data));
              // route to play area
              this.router.navigateByUrl("quizzer/play");
              this.isLoading = false;
            }
            else {
              // show error
              this.isLoading = false;
              this.toaster.error(res.error.description, "Quiz Starting error");
              // may be public service is not available right now , try sometimes later or try making your own.
            }
          },
          error: err => {
            this.isLoading = false;
            this.toaster.error("Invalid options selected, please try again.", "Validation error.")
          }
        });
      }
    }
    else {
      this.quizForm.markAllAsTouched();
    }
  }

  // shuffle array elements
  shuffle(array: any[]) {
    for (let i = array.length - 1; i >= 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
  }
}
