import { Component, OnInit, Input } from '@angular/core';
import { UserWithRelations } from './user-with-relations.model';
import { UsersService } from '../core/users.service';
import { SnackBarService } from '../core/snack-bar.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
  @Input() userWithRelations:UserWithRelations = new UserWithRelations();
  constructor(private usersService: UsersService, private snackBarService: SnackBarService) { }

  ngOnInit() {
  }

  createFollow() {
    const formData: FormData = new FormData();
    formData.append("FollowedById", this.userWithRelations.User.Id);
    this.usersService.createFollow(formData).subscribe(success => this.userWithRelations.IsFollow=true,
      err => this.snackBarService.openSnackBar(err,"",5000));
  }

}