import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ProcessoPublicListComponent } from '../processos/processo-public-list/processo-public-list.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, ProcessoPublicListComponent],
  template: `
    <div class="home-container">
      <div class="hero-section">
        <h1>Processo Seletivo</h1>
        <p>Participe dos processos seletivos abertos e acompanhe sua inscrição</p>
      </div>
      
      <app-processo-public-list></app-processo-public-list>
      
      <div class="info-section">
        <h2>Como funciona?</h2>
        <div class="steps">
          <div class="step">
            <div class="step-number">1</div>
            <h3>Inscreva-se</h3>
            <p>Preencha o formulário com seus dados</p>
          </div>
          <div class="step">
            <div class="step-number">2</div>
            <h3>Envie Documentos</h3>
            <p>Anexe os documentos necessários</p>
          </div>
          <div class="step">
            <div class="step-number">3</div>
            <h3>Acompanhe</h3>
            <p>Verifique o status da sua inscrição</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .home-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }
    
    .hero-section {
      text-align: center;
      padding: 40px 20px;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      border-radius: 10px;
      margin-bottom: 40px;
    }
    
    .hero-section h1 {
      font-size: 2.5rem;
      margin-bottom: 10px;
    }
    
    .hero-section p {
      font-size: 1.2rem;
      opacity: 0.9;
    }
    
    .info-section {
      text-align: center;
      padding: 40px 20px;
      background: #f8f9fa;
      border-radius: 10px;
      margin-top: 40px;
    }
    
    .steps {
      display: flex;
      justify-content: space-around;
      flex-wrap: wrap;
      gap: 20px;
      margin-top: 30px;
    }
    
    .step {
      flex: 1;
      min-width: 200px;
      max-width: 300px;
    }
    
    .step-number {
      width: 50px;
      height: 50px;
      background: #667eea;
      color: white;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.5rem;
      font-weight: bold;
      margin: 0 auto 15px;
    }
  `]
})
export class HomeComponent {}