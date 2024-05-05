export interface QuizQuestionResponse {
    questionId: string;
    questionText: string;
    questionImageLink: string;
    questionType: string;
    questionLevel: string;
    questionTags: string[];
    categoryName: string;
    options: IOptionResponse[]
    answer?: string;
}

export interface IOptionResponse {
    optionId: string;
    optionValue: string;
    optionSelected?: boolean;
}

// export enum QuestionType {
//     ANY = 0,
//     MULTIPLE = 1, // MCQ
//     MULTIPLE_CORRECT = 2,
//     BOOLEAN = 3 // True / False
// }
// export enum QuestionLevel {
//     ANY = 0,
//     EASY = 1,
//     MEDIUM = 2,
//     HARD = 3
// }