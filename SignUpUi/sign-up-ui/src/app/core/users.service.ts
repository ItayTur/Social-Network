import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ErrorHandlingService } from './error-handling.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  usersBaseUrl = "http://localhost:4573/api/Users";

  constructor(private httpClient: HttpClient, private errHandler: ErrorHandlingService) { }

  getUsers(): Observable<any> {
    return this.httpClient.get(this.usersBaseUrl+"/GetUsers", { withCredentials: true })
    .pipe(catchError(this.errHandler.handleError));
  }

  createFollow(formData: FormData): Observable<any> {
    return this.httpClient.post(this.usersBaseUrl+"/CreateFollow", formData, {withCredentials: true})
    .pipe(catchError(this.errHandler.handleError));
  }

  deleteFollow (formData: FormData): Observable<any> {
    return this.httpClient.post(this.usersBaseUrl+"/DeleteFollow", formData, {withCredentials: true})
    .pipe(catchError(this.errHandler.handleError));
  }

  createBlock (formData: FormData): Observable<any> {
    return this.httpClient.post(this.usersBaseUrl+'/CreateBlock', formData, {withCredentials: true})
    .pipe(catchError(this.errHandler.handleError));
  }

  deleteBlock (formData: FormData): Observable<any> {
    return this.httpClient.post(this.usersBaseUrl+'/DeleteBlock', formData, { withCredentials: true })
    .pipe(catchError(this.errHandler.handleError));
  }
}
