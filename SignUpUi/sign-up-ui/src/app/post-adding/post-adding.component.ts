import { Component, OnInit } from '@angular/core';
import { Post } from "./post.model";




@Component({
  selector: 'app-post-adding',
  templateUrl: './post-adding.component.html',
  styleUrls: ['./post-adding.component.scss']
})
export class PostAddingComponent implements OnInit {

  post: Post = new Post("","",new Date());
  imgSrc: string | ArrayBuffer;
  constructor() { }

  onFileChanged(event) {

    const pic = event.target.files[0];
    this.post.pic = pic;

    const reader = new FileReader();
    reader.onload = e => this.imgSrc = reader.result;
    reader.readAsDataURL(pic);
  }

  onSubmit() {

  }

  ngOnInit() {


  }



}
