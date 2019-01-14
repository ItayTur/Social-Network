import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root'
})
export class TagsService {

  constructor(private httpClient: HttpClient, private errorHandler: ErrorHandlingService) { }

  GetTags(text: string): Observable<any> {
    const url = "http://localhost:4573/api/Posts/SearchTag/"+text;
    return this.httpClient.get(url).pipe(retry(3), catchError(this.errorHandler.handleError));
  }
}
