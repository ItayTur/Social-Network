import { Component, OnInit, Input } from '@angular/core';
import { Comment } from "./comment.model";
import { TagsService } from '../core/tags.service';
import { PostsService } from '../core/posts.service';
import { SnackBarService } from '../core/snack-bar.service';
import { Observable } from 'rxjs';
import { User } from './user.model';

@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.scss']
})
export class CommentComponent implements OnInit {
  @Input()
  comment: Comment = new Comment();
  @Input()
  taggedUsers: User[] = [];
  @Input()
  writer: User = new User();


  constructor() { }

  ngOnInit() {
  }

}
