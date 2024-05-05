export interface IUserQuizAnswer {
    userQsAnswers : IUserAnswer[]
}


export interface IUserAnswer{
    questionId : string;
    selectedAnswer : string[];
}