import { Component, OnInit } from '@angular/core';
import { Post } from "./post.model";




@Component({
  selector: 'app-post-adding',
  templateUrl: './post-adding.component.html',
  styleUrls: ['./post-adding.component.scss']
})
export class PostAddingComponent implements OnInit {

  post: Post = new Post("","","",new Date());
  pics: any;
  constructor() { }

  onFileChanged(event) {
    const file = event.target.files[0];
  }

  onSubmit() {

  }

  ngOnInit() {

    
  }



}
