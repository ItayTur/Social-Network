import { Component, OnInit, Input, ViewChild } from "@angular/core";
import { IdentityService } from "../core/identity.service";
import { SnackBarService } from "../core/snack-bar.service";
import { UserModel } from "../core/user.model";

import { FormGroup, FormControl, Validators } from "@angular/forms";

@Component({
  selector: "app-user-update",
  templateUrl: "./user-update.component.html",
  styleUrls: ["./user-update.component.scss"]
})
export class UserUpdateComponent implements OnInit {

  // @Input() user: UserModel;
  user: UserModel = new UserModel(
    "Id",
    "itay_ru@walla.com",
    "itay",
    "tur",
    "fdsa 5",
    "developer",
    new Date()
  );
  IsUsernamePasswordUser: boolean;
  userForm: FormGroup;

  constructor(
    private identityService: IdentityService,
    private snackBarService: SnackBarService
  ) {}

  ngOnInit() {
    this.userForm = new FormGroup({
      firstNameInput: new FormControl(null, Validators.required),
      lastNameInput: new FormControl(null, Validators.required),
      emailInput: new FormControl(null, Validators.email),
      addressInput: new FormControl(null),
      jobInput: new FormControl(null),
      datepickerInput: new FormControl(null)
    });

    if (this.user.email) {
      this.IsUsernamePasswordUser = true;
    } else {
      this.IsUsernamePasswordUser = false;
    }
  }

  get firstNameInput() {
    return this.userForm.get("firstNameInput");
  }

  get lastNameInput() {
    return this.userForm.get("lastNameInput");
  }
  get emailInput() {
    return this.userForm.get("emailInput");
  }
  get addressInput() {
    return this.userForm.get("addressInput");
  }
  get datepickerInput() {
    return this.userForm.get("datepickerInput");
  }
  get jobInput() {
    return this.userForm.get("jobInput");
  }

  updateUser() {
    const formData: FormData = this.getUserFormData();
    this.identityService
      .updateUser(formData)
      .subscribe(
        success => this.snackBarService.openSnackBar("user updated", "", 5000),
        err => this.snackBarService.openSnackBar(err, "", 5000)
      );
  }

  private getUserFormData(): FormData {
    const formData: FormData = new FormData();
    formData.append("Address", this.user.address);
    formData.append("BirthDate", JSON.stringify(this.user.birthDate));
    formData.append("Email", this.user.email);
    formData.append("FirstName", this.user.firstName);
    formData.append("LastName", this.user.lastName);
    formData.append("Job", this.user.job);
    return formData;
  }
}
