import { Component, OnInit } from '@angular/core';
import { FacebookService, LoginResponse, InitParams  } from 'ngx-facebook';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  constructor(private FB: FacebookService) {
    let initParams: InitParams = {
      appId: '1183102151855520',
      xfbml: true,
      version: 'v3.2'
    };

    FB.init(initParams);
  }

  ngOnInit() {
    (window as any).fbAsyncInit = function() {
      this.FB.init({
        appId      : '1183102151855520',
        cookie     : true,
        xfbml      : true,
        version    : 'v3.2'
      });
      this.FB.AppEvents.logPageView();
    };

    (function(d, s, id) {
       let js, fjs = d.getElementsByTagName(s)[0];
       if (d.getElementById(id)) {return; }
       js = d.createElement(s); js.id = id;
       js.src = "https://connect.facebook.net/en_US/sdk.js";
       fjs.parentNode.insertBefore(js, fjs);
     }(document, 'script', 'facebook-jssdk'));
  }

  facebookLogin() {
    alert("submit login to facebook");
    console.log("submit login with facebook");
    this.FB.login()
      .then((response: LoginResponse) => console.log(response))
      .catch((error: any) => console.error(error));
  }
}
