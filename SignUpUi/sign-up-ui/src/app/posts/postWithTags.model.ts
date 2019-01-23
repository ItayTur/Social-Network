import { Post } from "../post-adding/post.model";
import { User } from "../comment/user.model";

export class PostWithTags {
  Post:Post ;
  Tags: User[];
}
