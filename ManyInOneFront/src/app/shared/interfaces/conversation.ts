export interface Conversation {
    contents: Content[];
}

export interface Content {
    role: string;
    parts: Part[];
}

export interface Part {
    text: string;
}