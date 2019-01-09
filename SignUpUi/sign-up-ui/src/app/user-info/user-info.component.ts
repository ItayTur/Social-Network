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
  user: UserModel;

  constructor(private identity: IdentityService, private snackBar: SnackBarService) { }

  ngOnInit() {
    this.identity.getDetails()
      .subscribe(user => {
        this.user = new UserModel(user.Id, user.Email, user.FirstName, user.LastName, user.Address, user.Job, user.BirthDate)
      },
        (err) => {
          this.snackBar.openSnackBar(err, "", 10000);
        }
      );
  }

}
