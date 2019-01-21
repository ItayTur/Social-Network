import { Component, OnInit, Input } from '@angular/core';
import { Comment } from "../comment/comment.model";
import { TagsService } from '../core/tags.service';
import { PostsService } from '../core/posts.service';
import { SnackBarService } from '../core/snack-bar.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-add-comment',
  templateUrl: './add-comment.component.html',
  styleUrls: ['./add-comment.component.scss']
})
export class AddCommentComponent implements OnInit {

  @Input() postId: string;
  tags: any[];
  comment: Comment = new Comment();

  imgSrc: string | ArrayBuffer;

  public requestedTags = (text: string): Observable<any> => this.tagsService.GetTags(text);

  onFileChanged(event) {

    const pic = event.target.files[0];
    this.comment.Pic = pic;

    const reader = new FileReader();
    reader.onload = e => this.imgSrc = reader.result;
    reader.readAsDataURL(pic);
  }


  onSubmit() {
    const formData = new FormData();
    formData.append("Pic",this.comment.Pic);
    formData.append("Content",this.comment.Content);
    formData.append("PostId",this.postId);
    formData.append("Tags",JSON.stringify(this.tags));
    this.postsService.AddComment(formData).subscribe(
      success=> {
        console.log(success);
        this.cleanForm();
      },
    (err)=> { this.snackBarService.openSnackBar(err, "", 10000); });
  }

  private cleanForm() {
    this.comment.Content=null;
    this.tags = null;
    this.comment.Pic = null;
    this.imgSrc = null;
  }


  constructor(private tagsService: TagsService,
     private postsService: PostsService, private snackBarService: SnackBarService) { }

  ngOnInit() {
  }

}
