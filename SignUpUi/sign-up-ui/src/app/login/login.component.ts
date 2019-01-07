import { Component, OnInit } from '@angular/core';
import { FacebookService, LoginResponse, InitParams, LoginOptions   } from 'ngx-facebook';
import { FacebookLoginService } from "../core/facebook-login.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {


  constructor(private FB: FacebookService, private fbLoginService: FacebookLoginService) {
    let initParams: InitParams = {
      appId: '1183102151855520',
      xfbml: true,
      version: 'v3.2'
    };

    this.FB.init(initParams);
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
    const loginOptions: LoginOptions = {
      enable_profile_selector: true,
      return_scopes: true,
      scope: 'email,public_profile',
      auth_type: 'rerequest'
    };
    this.FB.login(loginOptions).then((token: LoginResponse) => {
        let array = token.authResponse.grantedScopes.split(',');
        if(array.length === 2) {
          this.fbLoginService.login(token)
            .subscribe((response)=>console.log(response));
        } else {
           alert('You need to provide all the facebook permissions to use the app.');
        }
     })
      .catch((error: any) => console.error(error));
  }


}
