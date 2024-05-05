export interface IAllQuestion {
    questionId : string;
    questionText: string;
    questionImageLink: string;
    questionType: string;
    questionLevel: string;
    questionTags: string[];
    categoryName: string;
    options: IOptionRes[]
}

export interface IOptionRes {
    optionId : string;
    optionValue: string;
    answerExplanation: string;
    isAnswer: boolean
    // questionId: string; // not needed
}


