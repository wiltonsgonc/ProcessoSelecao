import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { LayoutService } from '../../core/services/layout.service';

@Component({
  selector: 'app-public-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, MatToolbarModule],
  template: `
    <mat-toolbar color="primary" class="header">
      <span class="logo" routerLink="/">Processo Seletivo</span>
      <span class="spacer"></span>
    </mat-toolbar>
    <div class="content">
      <router-outlet></router-outlet>
    </div>
    <footer class="footer">
      <p>2026 Sistema de Processo Seletivo</p>
    </footer>
  `,
  styles: [`
    :host {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }
    .header {
      position: sticky;
      top: 0;
      z-index: 100;
    }
    .logo {
      cursor: pointer;
      font-weight: bold;
      font-size: 1.2rem;
    }
    .spacer { flex: 1; }
    .content {
      flex: 1;
      padding: 20px;
      max-width: 1200px;
      margin: 0 auto;
      width: 100%;
      box-sizing: border-box;
    }
    .footer {
      text-align: center;
      padding: 20px;
      background: #f5f5f5;
      color: #666;
    }
  `]
})
export class PublicLayoutComponent {
  layoutService = inject(LayoutService);
}
