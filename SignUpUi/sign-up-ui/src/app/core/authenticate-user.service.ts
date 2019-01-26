import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { catchError, retry } from 'rxjs/operators';
import { AuthModel } from "./auth.model";
import { ErrorHandlingService } from "./error-handling.service";
import { Observable } from 'rxjs';
import { RegistrationInfoModel } from './registration-information.model';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type':  'application/json'
  })
};

@Injectable({
  providedIn: 'root'
})
export class AuthenticateUserService {

  baseUrl = "http://localhost:54353/api";
  authApi = this.baseUrl + "/Auth";

  constructor(private httpClient: HttpClient, private erorrHandler: ErrorHandlingService) { }

  login(email: string, password: string):Observable<any> {
    let auth: AuthModel = new AuthModel(email, password);
    return this.httpClient.post(this.authApi + "/Login", auth)
    .pipe(
      catchError(this.erorrHandler.handleError)
    );
  }

  register(registrationInfo:RegistrationInfoModel):Observable<any> {
    return this.httpClient.post(this.authApi + "/Register", registrationInfo, httpOptions)
    .pipe(
      catchError(this.erorrHandler.handleError)
    );
  }
}
