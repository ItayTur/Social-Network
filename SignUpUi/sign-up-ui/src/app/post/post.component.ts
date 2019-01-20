import { Component, OnInit, Input } from "@angular/core";
import { Post } from "../post-adding/post.model";
import { PostsService } from "../core/posts.service";
import { SnackBarService } from "../core/snack-bar.service";
import { post } from "selenium-webdriver/http";

@Component({
  selector: "app-post",
  templateUrl: "./post.component.html",
  styleUrls: ["./post.component.scss"]
})
export class PostComponent implements OnInit {
  constructor(
    private postsService: PostsService,
    private snackBarService: SnackBarService
  ) {}
  @Input()
  post: Post;
  isLikeClicked = false;

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
    this.postsService.UnLikePost(formData).subscribe(success => {
      console.log(success);
      this.isLikeClicked = false;
      this.post.Likes -= 1;
    }, err => {
      this.snackBarService.openSnackBar(err,"",10000);
    });
  }

  AddComment() {

  }

  ngOnInit() {
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
        this.snackBarService.openSnackBar(err,"",10000);
      }
    );
  }


}
