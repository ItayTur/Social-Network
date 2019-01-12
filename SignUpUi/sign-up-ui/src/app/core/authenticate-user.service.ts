import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { catchError, retry } from 'rxjs/operators';
import { AuthModel } from "./auth.model";
import { ErrorHandlingService } from "./error-handling.service";
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthenticateUserService {

  baseUrl = "http://localhost:54353/api";
  authApi = this.baseUrl + "/Auth/Login";

  constructor(private httpClient: HttpClient, private erorrHandler: ErrorHandlingService) { }

  login(email: string, password: string):Observable<any> {
    let auth: AuthModel = new AuthModel(email, password);
    return this.httpClient.post(this.authApi, auth)
    .pipe(
      retry(3),
      catchError(this.erorrHandler.handleError)
    );
  }
}
