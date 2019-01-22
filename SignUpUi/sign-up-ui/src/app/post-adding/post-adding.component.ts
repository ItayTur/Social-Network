import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { Post } from "./post.model";
import { PostAddingService } from "../core/post-adding.service";
import { SnackBarService } from '../core/snack-bar.service';
import { Observable } from 'rxjs';
import { TagsService } from '../core/tags.service';



@Component({
  selector: 'app-post-adding',
  templateUrl: './post-adding.component.html',
  styleUrls: ['./post-adding.component.scss']
})
export class PostAddingComponent implements OnInit {
  constructor(private postAddingService: PostAddingService,
    private tagsService: TagsService, private snackBarService: SnackBarService) { }
  @Output() postAdded:EventEmitter<any> = new EventEmitter();
  post: Post = new Post("",true);
  tags: any[];
  imgSrc: string | ArrayBuffer;
  public requestedTags = (text: string): Observable<any> => this.tagsService.GetTags(text);



  onFileChanged(event) {

    const pic = event.target.files[0];
    this.post.pic = pic;

    const reader = new FileReader();
    reader.onload = e => this.imgSrc = reader.result;
    reader.readAsDataURL(pic);
  }

  onSubmit() {
    this.postAddingService.AddPost(this.post, this.tags).subscribe(
      postAdded=> {
        console.log('done');
        this.postAdded.emit({post: postAdded, tags: this.tags});
        this.cleanForm();
      },
    (err)=> { this.snackBarService.openSnackBar(err, "", 10000); });
  }

  private cleanForm() {
    this.post.Content=null;
    this.tags = null;
    this.post.pic = null;
    this.imgSrc = null;
  }

  ngOnInit() {


  }



}
