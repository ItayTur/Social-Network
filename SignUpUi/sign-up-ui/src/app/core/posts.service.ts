import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { ErrorHandlingService } from "./error-handling.service";

@Injectable({
  providedIn: 'root'
})
export class PostsService {

  baseUrl = "http://localhost:4573/api/Posts";
  constructor(private httpClient: HttpClient, private errorHandler: ErrorHandlingService) { }

  GetPosts(): Observable<any> {
    return this.httpClient.get(this.baseUrl+"/GetUsersPosts", { withCredentials: true })
    .pipe(retry(3), catchError(this.errorHandler.handleError));
  }

  LikePost(formData: FormData): Observable<any> {
    return this.httpClient.post(this.baseUrl+"/LikePost", formData, { withCredentials: true })
    .pipe(catchError(this.errorHandler.handleError));
  }

  IsPostLiked(formData: FormData): Observable<any> {
    return this.httpClient.post(this.baseUrl+"/IsPostLiked", formData, { withCredentials: true })
    .pipe(catchError(this.errorHandler.handleError));
  }

  UnLikePost(formData: FormData) {
    return this.httpClient.post(this.baseUrl+"/UnLikePost",formData,{ withCredentials: true })
    .pipe(retry(3), catchError(this.errorHandler.handleError));
  }
}
