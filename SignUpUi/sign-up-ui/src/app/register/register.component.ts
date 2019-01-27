import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthenticateUserService } from "../core/authenticate-user.service";
import { SnackBarService } from "../core/snack-bar.service";
import { CookieService } from 'ngx-cookie-service';
import { RegistrationInfoModel } from '../core/registration-information.model';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;

  constructor(private auth: AuthenticateUserService, private snackBar: SnackBarService, private cookieService: CookieService,
    private router: Router) { }

  ngOnInit() {
    this.registerForm = new FormGroup({ firstName: new FormControl(null, Validators.required),
                                        lastName: new FormControl(null, Validators.required),
                                        password: new FormControl(null, [Validators.required, Validators.minLength(3)]),
                                        address: new FormControl(null),
                                        job: new FormControl(null),
                                        birthdate: new FormControl(null),
                                        email: new FormControl(null, [Validators.required, Validators.email])
                                      });

  }

  get input() { return this.registerForm.get('email'); }

  get firstname() {
    return this.registerForm.get("firstName");
  }
  get password() {
    return this.registerForm.get("password");
  }

  get lastname() {
    return this.registerForm.get("lastName");
  }
  get email() {
    return this.registerForm.get("email");
  }
  get address() {
    return this.registerForm.get("address");
  }
  get birthdate() {
    return this.registerForm.get("birthdate");
  }
  get job() {
    return this.registerForm.get("job");
  }

  onSubmit() {
    debugger;
    let registrationInfo = new RegistrationInfoModel(
                    this.registerForm.get('password').value,
                    this.registerForm.get('email').value,
                    this.registerForm.get('firstName').value,
                    this.registerForm.get('lastName').value,
                    this.registerForm.get('address').value,
                    this.registerForm.get('job').value,
                    this.registerForm.get('birthdate').value);
    this.auth.register(registrationInfo)
      .subscribe(appToken => {
        this.cookieService.set('authToken', appToken, 1);
        this.router.navigate(['/news-feed']);
      },
        (err) => {
          this.snackBar.openSnackBar(err, "", 10000);
        }
      );
  }

}
