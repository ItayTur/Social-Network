import { Component, OnInit, Input } from '@angular/core';
import { Post } from '../post-adding/post.model';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.scss']
})
export class PostComponent implements OnInit {

  constructor() { }
  @Input() post: Post;
  ngOnInit() {
  }

}
