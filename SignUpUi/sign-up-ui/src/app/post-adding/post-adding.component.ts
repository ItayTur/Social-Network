import { Component, OnInit } from '@angular/core';
import { Post } from "./post.model";
import { PostAddingService } from "../core/post-adding.service";
import { SnackBarService } from '../core/snack-bar.service';



@Component({
  selector: 'app-post-adding',
  templateUrl: './post-adding.component.html',
  styleUrls: ['./post-adding.component.scss']
})
export class PostAddingComponent implements OnInit {

  post: Post = new Post("","");
  imgSrc: string | ArrayBuffer;
  constructor(private postAddingService: PostAddingService, private snackBarService: SnackBarService) { }

  onFileChanged(event) {

    const pic = event.target.files[0];
    this.post.pic = pic;

    const reader = new FileReader();
    reader.onload = e => this.imgSrc = reader.result;
    reader.readAsDataURL(pic);
  }

  onSubmit() {
    this.postAddingService.AddPost(this.post).subscribe(success=> { console.log(success); },
    (err)=> { this.snackBarService.openSnackBar(err, "", 10000); });
  }

  ngOnInit() {


  }



}
