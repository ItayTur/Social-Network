import { Component, OnInit, Input } from "@angular/core";
import { Post } from "../post-adding/post.model";
import { PostsService } from "../core/posts.service";
import { SnackBarService } from "../core/snack-bar.service";
import { CommentWithTags } from "../comment/comment-with-tags.model";
import { IdentityService } from "../core/identity.service";
import { UserModel } from "../core/user.model";
import { TagsComponent } from "../tags/tags.component";

@Component({
  selector: "app-post",
  templateUrl: "./post.component.html",
  styleUrls: ["./post.component.scss"]
})
export class PostComponent implements OnInit {
  constructor(
    private postsService: PostsService,
    private snackBarService: SnackBarService,
    private identityService: IdentityService
  ) {}

  @Input()
  post: Post;
  @Input()
  tags = [];

  isLikeClicked = false;
  comments: CommentWithTags[] = [];
  isCommentClicked = false;
  isCommentsShow = false;
  isCommentsLoad = false;

  addComment() {
    this.isCommentClicked = !this.isCommentClicked;
  }

  onCommentAdded(commentDto) {
    const commentWithTags = new CommentWithTags();

    this.identityService.getDetails().subscribe(
      user => {
        commentWithTags.Writer.Id = user.Id;
        commentWithTags.Writer.Name = user.FirstName + user.LastName;
        commentWithTags.Writer.Email = user.Email;
        commentWithTags.Comment = commentDto.commentAdded;
        commentWithTags.TaggedUsers = commentDto.tags;
        this.comments.unshift(commentWithTags);
        this.isCommentsShow = true;
        this.isCommentClicked = false;
      },
      err => {
        this.snackBarService.openSnackBar(err, "", 10000);
      }
    );
  }

  getComments() {
    if (!this.isCommentsLoad) {
      this.postsService.GetComments(this.post.Id).subscribe(
        success => {
          if (success.length > 0) {
            console.log(success);
            this.comments = success;
            this.isCommentsLoad = true;
          } else {
            this.snackBarService.openSnackBar(
              "no comments has been send yet :`(",
              "",
              5000
            );
          }
        },
        err => this.snackBarService.openSnackBar(err, "", 10000)
      );
    }
    this.isCommentsShow = !this.isCommentsShow;
  }

  Like() {
    const formData = new FormData();
    formData.append("PostId", this.post.Id);
    this.postsService.LikePost(formData).subscribe(
      success => {
        this.post.Likes = this.post.Likes + 1;
        this.isLikeClicked = true;
      },
      err => this.snackBarService.openSnackBar(err, "", 10000)
    );
  }

  UnLike() {
    const formData = new FormData();
    formData.append("PostId", this.post.Id);
    this.postsService.UnLikePost(formData).subscribe(
      success => {
        console.log(success);
        this.isLikeClicked = false;
        this.post.Likes -= 1;
      },
      err => {
        this.snackBarService.openSnackBar(err, "", 10000);
      }
    );
  }

  ngOnInit() {
    this.isPostliked();
  }

  private isPostliked() {
    const formData = new FormData();
    formData.append("PostId", this.post.Id);
    const response = this.postsService.IsPostLikedBy(formData).subscribe(
      success => {
        console.log(success);
        if (success) {
          this.isLikeClicked = true;
        }
      },
      err => {
        this.snackBarService.openSnackBar(err, "", 10000);
      }
    );
  }
}
