import { Component, OnInit } from '@angular/core';
import { UsersService } from '../core/users.service';
import { UserWithRelations } from '../user/user-with-relations.model';
import { SnackBarService } from '../core/snack-bar.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {

  users: UserWithRelations[] = [];
  followers: UserWithRelations[] =[];


  constructor(private usersService: UsersService, private snackBarService: SnackBarService) { }

  ngOnInit() {
    this.getUsers();
    this.getFollowers();
  }

  private getUsers() {
    this.usersService.getUsers().subscribe( users => {
      this.users = users; } ,
     err => this.snackBarService.openSnackBar(err,"",5000));
  }

  private getFollowers() {
    this.usersService.getFollowers().subscribe( followers => this.followers = followers,
      err => this.snackBarService.openSnackBar(err,"",5000) );
  }

}
