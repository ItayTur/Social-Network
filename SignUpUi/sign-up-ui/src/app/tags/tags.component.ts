import { Component, OnInit, Input } from '@angular/core';
import { User } from '../comment/user.model';

@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
  styleUrls: ['./tags.component.scss']
})
export class TagsComponent implements OnInit {
  @Input()
  taggedUsers: User[];
  constructor() { }

  ngOnInit() {
  }

}
