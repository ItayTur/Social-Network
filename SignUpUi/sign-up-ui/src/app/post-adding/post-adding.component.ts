import { Component, OnInit } from '@angular/core';
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
  post: Post = new Post("");
  tags: string[];
  imgSrc: string | ArrayBuffer;
  public requestedTags = (text: string): Observable<any> => {
    let l =this.tagsService.GetTags(text);
    console.log(l);
    return l;
  }



  onFileChanged(event) {

    const pic = event.target.files[0];
    this.post.pic = pic;

    const reader = new FileReader();
    reader.onload = e => this.imgSrc = reader.result;
    reader.readAsDataURL(pic);
  }

  onSubmit() {
    this.postAddingService.AddPost(this.post, this.tags).subscribe(
      success=> {
        console.log('done');
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
