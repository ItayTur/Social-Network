import { Injectable } from '@angular/core';
import { Post } from '../post-adding/post.model';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class PostAddingService {

  constructor(private httpClient: HttpClient, private cookieService: CookieService) { }

  AddPost (post: Post) {
    const token = this.cookieService.get("authToken");

  }
}
