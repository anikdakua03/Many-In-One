import { NgClass } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { KatexOptions, MarkdownModule } from 'ngx-markdown';
import { ToastrService } from 'ngx-toastr';
import { QuizConfig } from '../../../shared/constants/QuizConfigs';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';
import { TableSearchFilterPipe } from '../../../shared/constants/table-search-filter.pipe';
import { AddQuestionRequest } from '../../../shared/interfaces/Quizz/add-question-request';
import { IAllCategory, ICategory } from '../../../shared/interfaces/Quizz/all-category';
import { IAllQuestion, IOptionRes } from '../../../shared/interfaces/Quizz/all-question';
import { EditQuestionRequest } from '../../../shared/interfaces/Quizz/edit-question-request';
import { QuizzService } from '../../../shared/services/quizz.service';
import { PaginationComponent } from "../../pagination/pagination.component";

@Component({
    selector: 'app-quizzer-manage',
    standalone: true,
    templateUrl: './quizzer-manage.component.html',
    styles: ``,
    imports: [RouterLink, ReactiveFormsModule, NgClass, FontAwesomeModule, MarkdownModule, PaginationComponent, TableSearchFilterPipe]
})
export class QuizzerManageComponent implements OnInit {

    public options: KatexOptions = {
        displayMode: true,
        throwOnError: false,
        errorColor: 'red',
    };

    @ViewChild('myTextArea') myTextArea!: ElementRef;
    @ViewChild('fileInput') uploadedFile!: ElementRef;

    rightArrow = FAIcons.RIGHT_ANGLED_ARROW;
    close = FAIcons.CLOSE;
    add = FAIcons.PLUS;
    upload = FAIcons.UPLOAD;
    download = FAIcons.DOWNLOAD;
    search = FAIcons.SEARCH;
    dots = FAIcons.ELLIPSES;

    isCategoryOpen: boolean = true;
    isQssOpen: boolean = false;
    addCategoryModal: boolean = false;
    editCategoryModal: boolean = false;
    deleteCategoryModal: boolean = false;
    addQsModal: boolean = false;
    editQsModal: boolean = false;
    deleteQsModal: boolean = false;
    isLoading: boolean = false;
    isUploading: boolean = false;

    questionForm!: FormGroup;
    updateQuestionForm!: FormGroup;
    categoryForm!: FormGroup;
    updateCategoryForm!: FormGroup;

    oldCate !: ICategory;

    categoryToDelete: string = "";

    qsToUpdate: string = "";
    qsToDelete: string = "";

    searchText: string = "";

    allQsLevels: { id: string, value: string }[] = QuizConfig.allQsLevels;

    allQsTypes: { id: string, value: string }[] = QuizConfig.allQsTypes;

    allCategories: IAllCategory = {
        categories: []
    };

    allQuestions: IAllQuestion[] = [];

    tags = new Set<string>();
    oldQsTags = new Set<string>();

    qsPerPage: number = 4;
    currentPage: number = 1;

    constructor(private fb: FormBuilder, private quizService: QuizzService, private toaster: ToastrService, private router: Router) {
    }

    ngOnInit(): void {
        // get categories from db
        this.quizService.getOwnCategories();

        this.quizService.allCategory$.subscribe(
            (res) => {
                this.allCategories = res;
            }
        );

        // get questions from db
        this.quizService.getAllQuestions();

        this.quizService.allQss$.subscribe(
            (res) => {
                this.allQuestions = res;
            }
        );


        this.questionForm = this.fb.group({
            questionText: new FormControl('', [Validators.required, Validators.minLength(5)]),
            questionImageLink: new FormControl(''),
            questionTags: new FormControl([]),
            questionType: new FormControl(this.allQsTypes[0].id, [Validators.required]),
            questionLevel: new FormControl(this.allQsLevels[0].id, [Validators.required]),
            category: new FormControl(this.allCategories?.categories[0].categoryId, [Validators.required]),
            options: new FormArray([
                this.fb.group({
                    optionValue: new FormControl('', [Validators.required, Validators.minLength(1)]),
                    answerExplanation: new FormControl('No explanation available.', [Validators.required, Validators.minLength(3)]),
                    isAnswer: new FormControl(false)
                }),
                this.fb.group({
                    optionValue: new FormControl('', [Validators.required, Validators.minLength(1)]),
                    answerExplanation: new FormControl('No explanation available.', [Validators.required, Validators.minLength(3)]),
                    isAnswer: new FormControl(false)
                })
            ])
        });

        this.categoryForm = new FormGroup({
            categoryName: new FormControl('', [Validators.required, Validators.minLength(3)]),
            description: new FormControl()
        });

    }

    // for auto adjusting textarea heights
    onInput(event: Event) {
        const textArea = event.target as HTMLTextAreaElement;
        const borderBoxHeight = (textArea.offsetHeight - textArea.clientHeight) + 'px';
        textArea.style.height = 'auto';
        textArea.style.height = (textArea.scrollHeight + parseInt(borderBoxHeight)) + 'px';
    }

    get getOptions(): FormArray {
        return this.questionForm.get('options') as FormArray;
    }

    get paginatedData() {
        if (this.allQuestions === null || this.allQuestions.length === 0) {
            return;
        }
        const start = (this.currentPage - 1) * (this.qsPerPage);

        const end = start + this.qsPerPage;

        return this.allQuestions.slice(start, end);
    }

    changePage(page: number) {
        this.currentPage = page;
    }

    addOption() {
        const newOptionGroup = this.fb.group({
            optionValue: new FormControl('', [Validators.required, Validators.minLength(1)]),
            answerExplanation: new FormControl('No explanation available.'),
            isAnswer: new FormControl(false)
        });
        // will allow only max 6 options
        if (this.getOptions.length == 6) {
            this.toaster.warning("Cannot add more options !", "Option Addition warning.");
            return;
        }
        this.toaster.info("Option added !", "Option Action");
        this.getOptions.push(newOptionGroup);
    }

    removeOption(index: number) {
        const optionsFormArray = this.getOptions;
        optionsFormArray.removeAt(index);

        this.toaster.info("Option removed !", "Option Action");

        // mark ot as touched or dirty because we manually changed option, so to reflect this is our form
        optionsFormArray.markAsDirty();
        optionsFormArray.markAsTouched();
    }

    addOptionsOnQsEdit() {
        const newOptionGroup = this.fb.group({
            optionValue: new FormControl(""),
            answerExplanation: new FormControl(""),
            isAnswer: new FormControl(false),
            optionId: new FormControl()
        });
        // will allow only max 6 options
        if (this.getOptions.length == 6) {
            return;
        }

        const old = this.updateQuestionForm.get('options') as FormArray;
        old.push(newOptionGroup);

    }

    addTag(event: any) {
        if ((event.key === 'Enter' || event.key === ',') && (event.target.value !== ',') && (event.target.value !== '')) {
            this.tags.add((event.target.value).toString());
            this.questionForm.get('questionTags')?.reset();
        }
    }

    addOldAndCurrentTag(event: any) {
        if ((event.key === 'Enter' || event.key === ',') && (event.target.value !== ',') && (event.target.value !== '')) {
            this.oldQsTags.add((event.target.value).toString());
            this.questionForm.get('questionTags')?.reset();
        }
    }

    removeTag(tag: string) {
        this.tags.delete(tag);
    }

    removeTagOnEdit(tag: string) {
        this.oldQsTags.delete(tag);
    }

    onInputChange(event: any) {
        this.searchText = event.target.value;
    }

    getQsTypeValue(typeId: string): string {
        const matchingType = this.allQsTypes.find(type => type.id === typeId.toLowerCase());
        return matchingType ? matchingType.value : 'Any';
    }

    getQsLevelValue(lvlId: string): string {
        const matchingType = this.allQsLevels.find(level => level.id === lvlId.toLowerCase());
        return matchingType ? matchingType.value : 'Any';
    }

    // region : Add / update / remove question / category
    onAddQuestion() {
        if (this.questionForm.valid) {
            this.isLoading = true;

            const tagsList: string[] = [];
            this.tags.forEach(a => tagsList.push(a));

            let qsReqBody: AddQuestionRequest = {
                questionText: this.questionForm.get('questionText')?.value,
                questionImageLink: this.questionForm.get('questionImageLink')?.value,
                questionType: this.questionForm.get('questionType')?.value,
                questionLevel: this.questionForm.get('questionLevel')?.value,
                questionTags: tagsList,
                categoryName: this.questionForm.get('category')?.value,
                options: this.questionForm.get('options')?.value,
            };

            this.quizService.addQuestion(qsReqBody).subscribe({
                next: res => {
                    if (res.isSuccess) {
                        this.quizService.getAllQuestions(); // refreshes
                        this.allQuestions = this.quizService.allQss$.getValue();
                        this.toaster.success(res.data, "Question Added.")
                    }
                    else {
                        this.toaster.error(res.data, "Question Addition Failed.")
                    }
                    this.closeModal();
                    this.questionForm.reset();
                    this.isLoading = false;
                },
                error: err => {
                    this.isLoading = false;
                    this.toaster.error("May be service is not available right now !!", "Service Error");
                }
            });
        }
        else {
            if (this.questionForm.controls['options'].valid === false) {
                this.toaster.error("Please do not keep empty explanation in options . !!", "Invalid Question Form Error");
            }
            this.questionForm.markAllAsTouched();
            this.toaster.error("Please fill the question addition form properly. !!", "Invalid Question Form Error");
        }
    }

    onQsExcelUpload() {
        const formData = new FormData();
        const file = this.uploadedFile.nativeElement.files[0];

        if ((file.type === "application/vnd.ms-excel" ||
            file.type === "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") &&
            file.size < 1153434) {
            formData.append(file.name, file);
            this.isLoading = true;
            this.isUploading = true;
            this.quizService.addMultipleQuestion(formData).subscribe({
                next: res => {
                    if (res.isSuccess) {
                        // toaster success
                        this.quizService.getOwnCategories(); // refreshes
                        this.allCategories = this.quizService.allCategory$.getValue();
                        this.quizService.getAllQuestions();
                        this.allQuestions = this.quizService.allQss$.getValue();
                        // with toaster
                        this.toaster.success(res.data, "Multiple Question Addition.")
                    }
                    else {
                        // toaster error
                        this.toaster.error(res.error.description, "File Validation Error", {timeOut : 5000})
                    }
                    this.closeModal();
                    this.isUploading = false;
                    this.isLoading = false;
                },
                error : err => {
                    this.isUploading = false;
                    this.isLoading = false;
                    this.toaster.error("Maybe service is not available right now, please try after some.", "Serice error Error");
                }
            });
        }
        else {
            this.toaster.error("Not a valid type of file or exceeds 1 MB. ", "File Validation Error");
        }
    }

    openEditQsModal(qs: IAllQuestion) {
        this.qsToUpdate = qs.questionId;
        const oldTagList: string[] = [];

        qs.questionTags.forEach(a => this.oldQsTags.add(a));
        this.oldQsTags.forEach(a => oldTagList.push(a));

        this.questionForm.patchValue({
            questionText: qs.questionText,
            questionImageLink: qs.questionImageLink,
            questionTags: [],
            questionType: this.allQsTypes.find(a => a.id == qs.questionType.toLowerCase())?.id,
            questionLevel: this.allQsLevels.find(a => a.id == qs.questionLevel.toLowerCase())?.id,
            category: qs.categoryName,
        });

        // now for those options form array need to use set control to bind all options
        this.questionForm.setControl('options', this.setExistingOptions(qs.options));

        this.editQsModal = true;
    }

    setExistingOptions(options: IOptionRes[]): FormArray<any> {
        const formArr = new FormArray<any>([]);

        options.forEach(op => {
            formArr.push(this.fb.group({
                optionValue: op.optionValue,
                answerExplanation: op.answerExplanation,
                isAnswer: op.isAnswer
            }) as FormGroup);
        });

        return formArr;
    }

    onUpdateQuestion() {
        if (this.questionForm.valid) {
            this.isLoading = true;
            const updatedTagList: string[] = [];
            this.oldQsTags.forEach(a => updatedTagList.push(a));

            let qsReqBody: EditQuestionRequest = {
                questionId: this.qsToUpdate,
                questionText: this.questionForm.get('questionText')?.value,
                questionImageLink: this.questionForm.get('questionImageLink')?.value,
                questionType: this.questionForm.get('questionType')?.value,
                questionLevel: this.questionForm.get('questionLevel')?.value,
                questionTags: updatedTagList,
                categoryName: this.questionForm.get('category')?.value,
                options: this.questionForm.get('options')?.value,
            };

            this.quizService.updateQuestion(qsReqBody).subscribe({
                next: res => {
                    if (res.isSuccess) {
                        this.quizService.getAllQuestions(); // refreshes
                        this.allQuestions = this.quizService.allQss$.getValue();
                        // with toaster
                        this.toaster.success(res.data, "Question Updation.")
                    }
                    else {
                        this.toaster.error(res.data, "Question Updation Failed.")
                    }
                    this.closeModal();
                    this.questionForm.reset();
                    this.isLoading = false;
                },
                error: err => {
                    this.isLoading = false;
                    this.toaster.error("May be service is not available right now !!", "Service Error");
                }
            });
        }
        else {
            this.questionForm.markAllAsTouched();
        }
    }

    deleteQuestion() {
        if (this.qsToDelete !== undefined || this.qsToDelete !== "") {
            this.isLoading = true;
            this.quizService.deleteQuestion(this.qsToDelete).subscribe({
                next: res => {
                    if (res.isSuccess) {
                        this.quizService.getAllQuestions(); // refreshes
                        this.allQuestions = this.quizService.allQss$.getValue();
                        // with toaster
                        this.toaster.success(res.data, "Question Deletion.")
                    }
                    else {
                        this.toaster.error(res.data, "Question Deletion Failed.")
                    }
                    this.closeModal();
                    this.isLoading = false;
                },
                error: err => {
                    this.isLoading = false;
                    this.toaster.error("May be service is not available right now !!", "Service Error");
                }
            });
        }
        else {
            // toaster error
        }
    }

    onAddCategory() {
        if (this.categoryForm.valid) {
            this.isLoading = true;
            this.quizService.addCategory(this.categoryForm.value).subscribe({
                next: res => {
                    if (res.isSuccess) {
                        this.quizService.getOwnCategories(); // refreshes
                        this.allCategories = this.quizService.allCategory$.getValue();
                        // with toaster
                        this.toaster.success(res.data, "Category Updation.")
                    }
                    else {
                        this.toaster.error(res.data, "Category Updation.")
                    }
                    this.closeModal();
                    this.categoryForm.reset();
                    this.isLoading = false;
                },
                error: err => {
                    this.isLoading = false;
                    this.toaster.error("May be service is not available right now !!", "Service Error");
                }
            });
        }
        else {
            this.questionForm.markAllAsTouched();
        }
    }

    updateCategory() {
        if (this.updateCategoryForm.valid) {
            this.isLoading = true;
            const updatedCategoryReqBody: { categoryName: string, newCategoryName: string, description: string } = {
                categoryName: this.oldCate.categoryName,
                newCategoryName: this.updateCategoryForm.get('newCategoryName')?.value,
                description: this.updateCategoryForm.get('description')?.value,
            };

            this.quizService.updateCategory(updatedCategoryReqBody).subscribe({
                next: res => {
                    if (res.isSuccess) {
                        this.quizService.getOwnCategories(); // refreshes
                        this.allCategories = this.quizService.allCategory$.getValue();
                        this.quizService.getAllQuestions(); // refreshes
                        this.allQuestions = this.quizService.allQss$.getValue();
                        // with toaster
                        this.toaster.success(res.data, "Category Updation.")
                    }
                    else {
                        this.toaster.error(res.data, "Category Updation Failed.")
                    }
                    this.closeModal();
                    this.categoryForm.reset();
                    this.isLoading = false;
                },
                error: err => {
                    this.isLoading = false;
                    this.toaster.error("May be service is not available right now !!", "Service Error");
                }
            });
        }
        else {
            this.updateCategoryForm.markAllAsTouched();
        }
    }

    deleteCategory() {
        if (this.categoryToDelete !== undefined || this.categoryToDelete !== "") {
            this.isLoading = true;
            this.quizService.deleteCategory(this.categoryToDelete).subscribe({
                next: res => {
                    if (res.isSuccess) {
                        this.quizService.getOwnCategories(); // refreshes
                        this.allCategories = this.quizService.allCategory$.getValue();
                        this.quizService.getAllQuestions(); // refreshes
                        this.allQuestions = this.quizService.allQss$.getValue();
                        // with toaster
                        this.toaster.success(res.data, "Category Deletion.")
                    }
                    else {
                        this.toaster.error(res.data, "Category Deletion Failed")
                    }
                    this.closeModal();
                    this.isLoading = false;
                },
                error: err => {
                    this.isLoading = false;
                    this.toaster.error("May be service is not available right now !!", "Service Error");
                }
            });
        }
        else {
            // toaster error
            this.toaster.error("Invalid", "Category Deletion Failed")
        }
    }

    // region : Modal pop up open close
    openCategories() {
        this.isCategoryOpen = true;
        this.isQssOpen = false;
    }

    openQss() {
        this.isCategoryOpen = false;
        this.isQssOpen = true;
    }

    openAddQsModal() {
        this.addQsModal = true;
    }

    openCategoryModal() {
        this.addCategoryModal = true;
    }

    openDeleteQsModal(qsId: string) {
        this.qsToDelete = qsId;
        this.deleteQsModal = true;
    }

    openDeleteCategoryModal(categoryName: string) {
        this.categoryToDelete = categoryName;
        this.deleteCategoryModal = true;
    }

    openEditCategoryModal(category: ICategory) {
        this.oldCate = category;
        this.updateCategoryForm = this.fb.group({
            categoryName: [{ value: this.oldCate.categoryName, disabled: true }],
            newCategoryName: new FormControl('', [Validators.required, Validators.minLength(3)]),
            description: [{ value: this.oldCate.description, disabled: false }]
        });
        this.editCategoryModal = true;
    }

    closeModal() {
        if (confirm("Your may have unsaved changes, Do you really want to exit ?")) {
            this.editCategoryModal = false;
            this.addCategoryModal = false;
            this.deleteCategoryModal = false;
            this.addQsModal = false;
            this.editQsModal = false;
            this.deleteQsModal = false;
        }
    }
}
