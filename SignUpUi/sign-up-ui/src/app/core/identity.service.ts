import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { ErrorHandlingService } from "./error-handling.service";


@Injectable({
  providedIn: 'root'
})
export class IdentityService {
  baseUrl = "http://localhost:9152/api";
  identityApi = this.baseUrl + "/Users";

  constructor(private httpClient: HttpClient, private erorrHandler: ErrorHandlingService) { }

  getDetails():Observable<any> {
    return this.httpClient.get(this.identityApi+"/?id=" + "7d47d17c-387c-45c0-89b2-a040f7ba7e5e", { withCredentials: true })
    .pipe(
      retry(3),
      catchError(this.erorrHandler.handleError)
    );
  }
}
