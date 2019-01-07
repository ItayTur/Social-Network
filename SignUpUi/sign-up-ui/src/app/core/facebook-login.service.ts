import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { FacebookTokenModel } from "./facebookToken.model";

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type':  'application/json'
  })
};

@Injectable({
  providedIn: 'root'
})
export class FacebookLoginService {

  baseUrl = "http://localhost:54353/api";
  facebookLoginApi = this.baseUrl + "/Auth/FacebookSignIn";


  constructor(private httpClient: HttpClient) { }
  login(auth: any): Observable<any> {
    let facebookToken: FacebookTokenModel = new FacebookTokenModel(auth.authResponse.accessToken);
    return this.httpClient.post(this.facebookLoginApi, facebookToken, httpOptions)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('server error:', error);
    if (error.error instanceof Error) {
        const errMessage = error.error.message;
        return Observable.throw(errMessage);
    }
    return Observable.throw(error || 'server error');
  }

}
