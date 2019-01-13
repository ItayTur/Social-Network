import { Injectable } from '@angular/core';
import { Post } from '../post-adding/post.model';
import { HttpClient, HttpHeaders, HttpResponse } from "@angular/common/http";
import { CookieService } from 'ngx-cookie-service';
import { ErrorHandlingService } from './error-handling.service';
import { retry, catchError } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PostAddingService {
  baseUrl = "http://localhost:4573/api/";
  addPostUrl = this.baseUrl+"/Posts/AddPost";
  constructor(private httpClient: HttpClient, private cookieService: CookieService,
    private errorHandlingService: ErrorHandlingService) { }

  AddPost (post: Post): Observable<any>  {
    const formData = new FormData();
    formData.append("Content",post.Content);
    formData.append("Tags",post.Tags);
    if(post.pic) {
      formData.append("Pic",post.pic, post.pic.name);
    }
    return this.httpClient.post("http://localhost:4573/api/Posts/AddPost", formData).pipe(
      retry(3), catchError(this.errorHandlingService.handleError));
  }
}
