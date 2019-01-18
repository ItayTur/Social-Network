import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { ErrorHandlingService } from "./error-handling.service";

@Injectable({
  providedIn: 'root'
})
export class PostsService {

  baseUrl = "http://localhost:4573/api/";
  constructor(private httpClient: HttpClient, private errorHandler: ErrorHandlingService) { }

  GetPosts(): Observable<any> {
    return this.httpClient.get(this.baseUrl+"Posts/GetUsersPosts", { withCredentials: true })
    .pipe(catchError(this.errorHandler.handleError));
  }
}
