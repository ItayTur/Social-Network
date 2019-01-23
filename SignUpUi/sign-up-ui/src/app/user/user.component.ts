import { Component, OnInit, Input } from '@angular/core';
import { UserWithRelations } from './user-with-relations.model';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
  @Input() userWithRelations:UserWithRelations = new UserWithRelations();
  constructor() { }

  ngOnInit() {
  }

}
