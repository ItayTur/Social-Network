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
  baseUrl = "http://localhost8080/api/";
  addPostUrl = "baseUrl/"+"Posts";
  constructor(private httpClient: HttpClient, private cookieService: CookieService,
    private errorHandlingService: ErrorHandlingService) { }

  AddPost (post: Post): Observable<any> {

    return this.httpClient.post(this.addPostUrl, post, ).pipe(retry(3),
     catchError(this.errorHandlingService.handleError));
  }
}
