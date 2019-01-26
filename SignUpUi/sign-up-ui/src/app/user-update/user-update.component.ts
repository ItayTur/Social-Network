import { Component, OnInit, Input } from '@angular/core';
import { IdentityService } from '../core/identity.service';
import { SnackBarService } from "../core/snack-bar.service";
import { UserModel } from "../core/user.model";

@Component({
  selector: 'app-user-update',
  templateUrl: './user-update.component.html',
  styleUrls: ['./user-update.component.scss']
})
export class UserUpdateComponent implements OnInit {
  @Input() user: UserModel;
  IsUsernamePasswordUser: boolean;
  constructor(private identityService: IdentityService, private snackBarService: SnackBarService) { }

  ngOnInit() {
    if (this.user.email) {
      this.IsUsernamePasswordUser = true;
    } else {
      this.IsUsernamePasswordUser = false;
    }
  }



  updateUser () {
    const formData: FormData = this.getUserFormData();
    this.identityService.updateUser(formData).subscribe(success =>
       this.snackBarService.openSnackBar("user updated","",5000),
       err => this.snackBarService.openSnackBar(err,"",5000));
  }

  private getUserFormData (): FormData {
    const formData:FormData = new FormData();
    formData.append("Address", this.user.address);
    formData.append("BirthDate", JSON.stringify(this.user.birthDate));
    formData.append("Email", this.user.email);
    formData.append("FirstName", this.user.firstName);
    formData.append("LastName", this.user.lastName);
    formData.append("Job", this.user.job);
    return formData;
  }
}
