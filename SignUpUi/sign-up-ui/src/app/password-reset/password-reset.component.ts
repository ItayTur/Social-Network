import { Component, OnInit } from '@angular/core';
import { AuthenticateUserService } from '../core/authenticate-user.service';
import { SnackBarService } from '../core/snack-bar.service';

@Component({
  selector: 'app-password-reset',
  templateUrl: './password-reset.component.html',
  styleUrls: ['./password-reset.component.scss']
})
export class PasswordResetComponent implements OnInit {

  oldPassword = "";
  newPassword = "";

  constructor(private authService: AuthenticateUserService, private snackBarService: SnackBarService) { }

  ngOnInit() {
  }

  resetPassword() {
    const formData: FormData = new FormData();
    formData.append("NewPassword", this.newPassword);
    formData.append("OldPassword", this.oldPassword);
    this.authService.resetPassword(formData).subscribe( success =>
      this.snackBarService.openSnackBar("Password updated","",5000),
      err => this.snackBarService.openSnackBar(err,"",5000));
  }

}
