import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

import { LayoutService } from '../../core/services/layout.service';

@Component({
  selector: 'app-pagina-nao-encontrada',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="container">
      <h1>404</h1>
      <h2>Página Não Encontrada</h2>
      <p>A página que você está procurando não existe ou foi removida.</p>
      <a routerLink="/inscricao/1">Voltar para a página inicial</a>
    </div>
  `,
  styles: [`
    .container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      min-height: 80vh;
      text-align: center;
      font-family: Roboto, sans-serif;
    }
    h1 {
      font-size: 6rem;
      margin: 0;
      color: #666;
    }
    h2 {
      color: #333;
      margin: 10px 0;
    }
    p {
      color: #666;
      margin-bottom: 20px;
    }
    a {
      color: #1976d2;
      text-decoration: none;
      font-weight: 500;
    }
    a:hover {
      text-decoration: underline;
    }
  `]
})
export class PaginaNaoEncontradaComponent {
  constructor(private layoutService: LayoutService) {
    this.layoutService.mostrarElementosPublicos(false);
  }
}
