export interface QuizResultResponse {
    totalQs: number;
    totalCorrect: number;
    totalScore: number;
    totalTime: string;
    percentage: any;
    hasPassed: boolean;
    results: CorrectAnswer[];
}

export interface CorrectAnswer {
    questionId: string;
    answerExplanation: string[];
}