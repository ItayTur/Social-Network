

export class Post {
    constructor(
        public Content: string,
        public IsPublic: boolean,
    ) {}
    pic: File;
    WriterEmail: string;
    DateTime;
    Likes: number;
    ImgUrl: string;
}

