import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class FacebookLoginService {

  baseUrl = "http://localhost:8080/api";
  authApi = this.baseUrl+"/Auth";

  constructor(private httpClient: HttpClient) { }
  login(auth: any) {
    console.log(auth);
    return this.httpClient.post(this.authApi,auth.accessToken);
  }

}
