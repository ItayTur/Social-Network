import { Comment } from "./comment.model";
import { User } from "./user.model";

export class CommentWithTags {
Comment: Comment ;
TaggedUsers: User[];
Writer: User;

constructor () {
  this.Comment = new Comment();
  this.Writer = new User();
  this.TaggedUsers = [];
}
}
