import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { FacebookService, LoginResponse, InitParams, LoginOptions   } from 'ngx-facebook';
import { FacebookLoginService } from "../core/facebook-login.service";
import { AuthenticateUserService } from "../core/authenticate-user.service";
import {MatSnackBar} from '@angular/material';



@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;

  constructor(private FB: FacebookService, private fbLoginService: FacebookLoginService, private auth: AuthenticateUserService, public snackBar: MatSnackBar) {
    let initParams: InitParams = {
      appId: '1183102151855520',
      xfbml: true,
      version: 'v3.2'
    };

    this.FB.init(initParams);
  }

  ngOnInit() {
    this.loginForm = new FormGroup({password: new FormControl(null, Validators.required), email: new FormControl(null, [Validators.required, Validators.email])});

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
  
  get input() { return this.loginForm.get('password'); }

  openSnackBar(message: string, action: string, duration: number) {
    this.snackBar.open(message, action || "close", {
      duration: duration || 2000,
    });
  }

  onSubmit() { 
    this.auth.login(this.loginForm.get('email').value, this.loginForm.get('password').value)
    .subscribe(res=>{
      console.log(res); // make sure you get data here.
   },
   (err)=>{
     let errorMessage = err.error.Message;
     if (err.error.Message == "Internal server error"){
       errorMessage = "Oops, something went wrong. Please try again later";
     }
     console.log(this.openSnackBar(errorMessage, "", 10000))}
   );
  }

  facebookLogin() {
    debugger;
    const loginOptions: LoginOptions = {
      enable_profile_selector: true,
      return_scopes: true,
      scope: 'email,public_profile',
      auth_type: 'rerequest'
    };
    this.FB.login(loginOptions).then((response: LoginResponse) => {
        let array = response.authResponse.grantedScopes.split(',');
        if(array.length === 2) {
          console.log(response);
          this.fbLoginService.login(response);
        } else {
           alert('You need to provide all the facebook permissions to use the app.' + array.length);
        }
     })
      .catch((error: any) => console.error(error));
  }


}
