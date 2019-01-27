import { Component, OnInit } from '@angular/core';
import { IdentityService } from '../core/identity.service';
import { SnackBarService } from "../core/snack-bar.service";
import { UserModel } from '../core/user.model';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss']
})
export class UserInfoComponent implements OnInit {
  user: UserModel = new UserModel("", "","","","","",null);

  isUpdateShow = false;
  isPasswordResetShow = false;
  isPasswordUser = false;

  showUpdate() {
    this.isUpdateShow = !this.isUpdateShow;
    this.isPasswordResetShow = false;
  }

  showPasswordReset() {
    this.isUpdateShow = false;
    this.isPasswordResetShow = !this.isPasswordResetShow;
  }


  onUpdate (user) {
    this.isUpdateShow = false;
    this.user = new UserModel(user.Id, user.Email, user.FirstName, user.LastName, user.Address, user.Job, user.BirthDate); // updatedUser;
  }

  constructor(private identity: IdentityService, private snackBar: SnackBarService) { }

  ngOnInit() {
    this.identity.getDetails()
      .subscribe(user => {
        this.user = new UserModel(user.Id, user.Email, user.FirstName, user.LastName, user.Address, user.Job, user.BirthDate);
        this.user.registrationType = user.RegistrationType;
        if (user.RegistrationType === "UserNamePassword") {
          this.isPasswordUser = true;
        }
      },
        (err) => {
          this.snackBar.openSnackBar(err, "", 10000);
        }
      );
  }

}
