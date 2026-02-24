import { Component } from '@angular/core';
import { RouterLink, RouterOutlet, RouterLinkActive } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatIconModule],
  template: `
    <div class="sidebar">
      <div class="sidebar-header">
        Sistema de Seleção IC/Pesquisa
      </div>
      <ul class="sidebar-nav">
        <li>
          <a routerLink="/processos" routerLinkActive="active">
            <mat-icon>assignment</mat-icon>
            Processos
          </a>
        </li>
        <li>
          <a routerLink="/candidatos" routerLinkActive="active">
            <mat-icon>people</mat-icon>
            Candidatos
          </a>
        </li>
        <li>
          <a routerLink="/avaliadores" routerLinkActive="active">
            <mat-icon>rate_review</mat-icon>
            Avaliadores
          </a>
        </li>
        <li>
          <a routerLink="/documentos" routerLinkActive="active">
            <mat-icon>folder</mat-icon>
            Documentos
          </a>
        </li>
        <li>
          <a routerLink="/baremas" routerLinkActive="active">
            <mat-icon>grade</mat-icon>
            Baremas
          </a>
        </li>
      </ul>
    </div>
    <div class="main-content">
      <router-outlet></router-outlet>
    </div>
  `
})
export class AppComponent {}
