import { Component, OnInit, Input } from '@angular/core';
import { PostsService } from '../core/posts.service';
import { SnackBarService } from '../core/snack-bar.service';
import { PostWithTags } from './postWithTags.model';

@Component({
  selector: 'app-posts',
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.scss']
})
export class PostsComponent implements OnInit {
  posts: PostWithTags[] = [];
  constructor(private postService: PostsService, private snackBarService: SnackBarService) { }

  ngOnInit() {
    this.postService.GetPosts()
    .subscribe((servicePosts)=> {
      console.log(servicePosts);
      this.posts = servicePosts;
    } ,(err)=> this.snackBarService.openSnackBar(err,"",10000));

  }

  onPostAdded(addedPost) {
    debugger;
    this.posts.unshift(addedPost);
  }


}
