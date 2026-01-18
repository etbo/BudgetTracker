import { Component } from '@angular/core';
import { AccountsStatus } from '../accounts-status/accounts-status';

@Component({
  selector: 'app-home',
  imports: [AccountsStatus],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {

}
