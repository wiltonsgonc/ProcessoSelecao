import { Component } from '@angular/core';
import { RouterLink, RouterOutlet, RouterLinkActive } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatIconModule],
  template: `
    <div class="sidebar">
      <div class="sidebar-header">
        <a routerLink="/">Painel Admin</a>
      </div>
      <ul class="sidebar-nav">
        <li>
          <a routerLink="/admin/processos" routerLinkActive="active">
            <mat-icon>assignment</mat-icon>
            Processos
          </a>
        </li>
        <li>
          <a routerLink="/admin/candidatos" routerLinkActive="active">
            <mat-icon>people</mat-icon>
            Candidatos
          </a>
        </li>
        <li>
          <a routerLink="/admin/avaliadores" routerLinkActive="active">
            <mat-icon>rate_review</mat-icon>
            Avaliadores
          </a>
        </li>
        <li>
          <a routerLink="/admin/documentos" routerLinkActive="active">
            <mat-icon>folder</mat-icon>
            Documentos
          </a>
        </li>
        <li>
          <a routerLink="/admin/baremas" routerLinkActive="active">
            <mat-icon>grade</mat-icon>
            Baremas
          </a>
        </li>
        <li>
          <a routerLink="/admin/editais" routerLinkActive="active">
            <mat-icon>description</mat-icon>
            Editais
          </a>
        </li>
      </ul>
      <div class="sidebar-footer">
        <a routerLink="/">
          <mat-icon>exit_to_app</mat-icon>
          Ver Site Público
        </a>
      </div>
    </div>
    <div class="main-content">
      <router-outlet></router-outlet>
    </div>
  `,
  styles: [`
    :host {
      display: flex;
      min-height: 100vh;
    }
    .sidebar {
      width: 250px;
      background: #2c3e50;
      color: white;
      display: flex;
      flex-direction: column;
    }
    .sidebar-header {
      padding: 20px;
      background: #1a252f;
      font-size: 1.2rem;
      font-weight: bold;
    }
    .sidebar-header a {
      color: white;
      text-decoration: none;
    }
    .sidebar-nav {
      list-style: none;
      padding: 0;
      margin: 0;
      flex: 1;
    }
    .sidebar-nav li a {
      display: flex;
      align-items: center;
      padding: 15px 20px;
      color: #bdc3c7;
      text-decoration: none;
      transition: all 0.3s;
    }
    .sidebar-nav li a:hover,
    .sidebar-nav li a.active {
      background: #34495e;
      color: white;
      border-left: 3px solid #3498db;
    }
    .sidebar-nav li a mat-icon {
      margin-right: 10px;
    }
    .sidebar-footer {
      padding: 15px 20px;
      border-top: 1px solid #34495e;
    }
    .sidebar-footer a {
      display: flex;
      align-items: center;
      color: #bdc3c7;
      text-decoration: none;
    }
    .sidebar-footer a:hover {
      color: white;
    }
    .sidebar-footer a mat-icon {
      margin-right: 10px;
    }
    .main-content {
      flex: 1;
      padding: 20px;
      overflow-y: auto;
    }
  `]
})
export class AdminLayoutComponent {}
