export interface ApiResponse<T> {
    isSuccess: boolean,
    data: T,
    error: IError
}

export interface IError {
    code: string
    description: string
    type: ErrorType 
}

enum ErrorType {
    Failure = 0, // General fail
    Validation = 1, // Status Code 400 Bad request
    NotFound = 2, // Status Code 404 Not Found
    Conflict = 3 // Status Code 409
}