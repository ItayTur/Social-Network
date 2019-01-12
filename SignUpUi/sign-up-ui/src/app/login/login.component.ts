import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { FacebookService, LoginResponse, InitParams, LoginOptions } from 'ngx-facebook';
import { FacebookLoginService } from "../core/facebook-login.service";
import { AuthenticateUserService } from "../core/authenticate-user.service";
import { SnackBarService } from "../core/snack-bar.service";
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  cookieValue = 'UNKNOWN';
  loginForm: FormGroup;

  constructor(private FB: FacebookService, private fbLoginService: FacebookLoginService,
    private auth: AuthenticateUserService, private snackBar: SnackBarService, private cookieService: CookieService,
    private router:Router) {
    let initParams: InitParams = {
      appId: '1183102151855520',
      xfbml: true,
      version: 'v3.2'
    };

    this.FB.init(initParams);
  }

  ngOnInit() {
    this.loginForm = new FormGroup({ password: new FormControl(null, Validators.required),
       email: new FormControl(null, [Validators.required, Validators.email]) });

    (window as any).fbAsyncInit = function () {
      this.FB.init({
        appId: '1183102151855520',
        cookie: true,
        xfbml: true,
        version: 'v3.2'
      });
      this.FB.AppEvents.logPageView();
    };

    (function (d, s, id) {
      let js, fjs = d.getElementsByTagName(s)[0];
      if (d.getElementById(id)) { return; }
      js = d.createElement(s); js.id = id;
      js.src = "https://connect.facebook.net/en_US/sdk.js";
      fjs.parentNode.insertBefore(js, fjs);
    }(document, 'script', 'facebook-jssdk'));
  }

  get input() { return this.loginForm.get('password'); }


  onSubmit() {
    this.auth.login(this.loginForm.get('email').value, this.loginForm.get('password').value)
      .subscribe(appToken => {
        this.cookieService.set('authToken', appToken, 1);
        this.router.navigate(['/news-feed']);
      },
        (err) => {
          this.snackBar.openSnackBar(err, "", 10000);
        }
      );
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
            .subscribe((appToken)=>this.cookieService.set("authToken",appToken,1));
            this.router.navigate(['/news-feed']);
        } else {
          this.snackBar.openSnackBar('You need to provide all the facebook permissions to use the app.', "", 10000);
        }
     })
      .catch((error: any) => console.error(error));
  }
}
