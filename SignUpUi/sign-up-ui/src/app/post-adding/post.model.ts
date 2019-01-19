

export class Post {
    constructor(
        public Content: string,
        public IsPublic: boolean,
    ) {}
    pic: File;
    WriterName: string;
    DateTime;
    Likes: number;
    ImgUrl: string;
}

