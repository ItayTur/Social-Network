import { Component, OnInit, Input } from '@angular/core';
import { User } from '../comment/user.model';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
  @Input() user:User = new User();
  constructor() { }

  ngOnInit() {
  }

}
