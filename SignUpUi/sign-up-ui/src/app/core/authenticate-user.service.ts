import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class AuthenticateUserService {

  baseUrl = "http://localhost:54353/api";
  authApi = this.baseUrl + "/Auth/Login";

  constructor(private httpClient: HttpClient) { }

  login(email: string, password: string) {
    return this.httpClient.post(this.authApi + '?email=' + email + '&password=' + password, null);
  }
}
