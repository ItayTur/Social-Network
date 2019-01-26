import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from "@angular/common/http";

import { FacebookLoginService } from "./facebook-login.service";
import { AuthenticateUserService } from "./authenticate-user.service";
import { SnackBarService } from "./snack-bar.service";
import { PostAddingService } from "./post-adding.service";
import { PostsService } from "./posts.service";
import { AppRoutingModule } from '../app-routing.module';

@NgModule({
  declarations: [],
  imports: [
    BrowserModule,
    CommonModule,
    HttpClientModule,
    AppRoutingModule
  ],
  providers: [FacebookLoginService, AuthenticateUserService, SnackBarService, PostAddingService, PostsService]
})
export class CoreModule { }
