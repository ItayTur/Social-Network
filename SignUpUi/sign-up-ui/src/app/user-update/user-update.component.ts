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
    "user3",
    "user3_ru@walla.com",
    "moshe",
    "shami",
    "sela 5",
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
      firstNameInput: new FormControl(this.user.firstName, Validators.required),
      lastNameInput: new FormControl(this.user.lastName, Validators.required),
      emailInput: new FormControl(this.user.email, Validators.email),
      addressInput: new FormControl(this.user.address),
      jobInput: new FormControl(this.user.job),
      datepickerInput: new FormControl(this.user.birthDate)
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
    formData.append("Address", this.addressInput.value);
    formData.append("BirthDate", this.datepickerInput.value);
    formData.append("Email", this.emailInput.value);
    formData.append("FirstName", this.firstNameInput.value);
    formData.append("LastName", this.lastNameInput.value);
    formData.append("Job", this.jobInput.value);
    return formData;
  }
}
