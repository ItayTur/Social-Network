

export class Post {
    constructor(
        public Content: string,
        public Tags: string,
        public DateTime: Date,
    ) {}
    token: string;
    pic: File;
}

