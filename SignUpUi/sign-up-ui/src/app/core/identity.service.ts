import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { ErrorHandlingService } from "./error-handling.service";


@Injectable({
  providedIn: 'root'
})
export class IdentityService {
  baseUrl = "http://localhost:54353/api";
  identityApi = this.baseUrl + "/Identity/";

  constructor(private httpClient: HttpClient, private erorrHandler: ErrorHandlingService) { }

  getDetails():Observable<any> {
    return this.httpClient.get(this.identityApi)
    .pipe(
      retry(3),
      catchError(this.erorrHandler.handleError)
    );
  }
}
