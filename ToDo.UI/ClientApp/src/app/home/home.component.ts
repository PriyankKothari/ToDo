import { Component, OnInit } from '@angular/core';
import { first } from 'rxjs/operators';

import { User } from '../_models';
import { UserService } from '../_services';

@Component({ templateUrl: 'home.component.html', selector: 'home' })
export class HomeComponent implements OnInit {
  users: User[];

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.users = [];
    this.users.push(JSON.parse(localStorage.getItem('currentUser')));
  }
}
