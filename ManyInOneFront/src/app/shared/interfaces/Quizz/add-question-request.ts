export interface AddQuestionRequest {
    questionText: string;
    questionImageLink: string;
    questionType: string;
    questionLevel: string;
    questionTags: string[];
    categoryName: string;
    options: Option[]
}

export interface Option {
    optionValue: string;
    answerExplanation: string;
    isAnswer: boolean
}