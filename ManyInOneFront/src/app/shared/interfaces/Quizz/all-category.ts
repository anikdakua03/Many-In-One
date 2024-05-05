export interface IAllCategory {
    categories: ICategory[];
}

export interface ICategory {
    categoryId: string; // but from api one is integer, keep in mind
    categoryName: string;
    description? : string;
    questionCount? : number;
}