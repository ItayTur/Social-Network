import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { ErrorHandlingService } from "./error-handling.service";


@Injectable({
  providedIn: 'root'
})
export class NotificationsService {

  baseServerUrl = "http://localhost:51988/api";
  notificationServerApi = this.baseServerUrl + "/Notifications/UserAuth";

  constructor(private httpClient: HttpClient, private erorrHandler: ErrorHandlingService) { }

  getNotificationsAuth(): Observable<any> {
    return this.httpClient.get(this.notificationServerApi, { withCredentials: true })
      .pipe(retry(3), catchError(this.erorrHandler.handleError));
  }
  
}
