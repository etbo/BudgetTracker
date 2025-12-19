import { Component } from '@angular/core'; // OnInit et signal ne sont plus nécessaires ici
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { NavMenuComponent } from './nav-menu/nav-menu'; 

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    NavMenuComponent,
    MatSidenavModule,
    MatToolbarModule, 
    MatButtonModule, 
    MatIconModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class AppComponent {
  // On ne garde que la logique de l'interface (UI)
  public isDrawerOpen = true; 
  public isDarkMode = false;

  toggleDrawer() {
    this.isDrawerOpen = !this.isDrawerOpen;
  }
}