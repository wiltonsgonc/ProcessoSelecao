import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ProcessoSelecaoService } from '../../../core/services/processo-selecao.service';
import { ProcessoSelecao, StatusProcesso } from '../../../core/models';

@Component({
  selector: 'app-processo-public-list',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatButtonModule, MatIconModule],
  template: `
    <div class="processos-container">
      <h1>Processos Seletivos Abertos</h1>
      
      <div *ngIf="carregando" class="carregando">
        <mat-icon class="spin">refresh</mat-icon>
        <p>Carregando processos...</p>
      </div>
      
      <div *ngIf="!carregando && processos.length === 0" class="sem-processos">
        <mat-icon>assignment_late</mat-icon>
        <p>Nenhum processo seletivo aberto no momento.</p>
      </div>
      
      <div class="processos-grid" *ngIf="!carregando && processos.length > 0">
        <mat-card *ngFor="let processo of processos" class="processo-card">
          <mat-card-header>
            <mat-icon mat-card-avatar>assignment</mat-icon>
            <mat-card-title>{{ processo.nome }}</mat-card-title>
            <mat-card-subtitle>
              <span [class]="'status-badge status-' + getStatusClass(processo.status)">
                {{ getStatusLabel(processo.status) }}
              </span>
            </mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            <p>{{ processo.descricao }}</p>
            <div class="info-row">
              <mat-icon>people</mat-icon>
              <span>{{ processo.vagasDisponiveis }} vagas</span>
            </div>
            <div class="info-row" *ngIf="processo.totalCandidatos > 0">
              <mat-icon>how_to_reg</mat-icon>
              <span>{{ processo.totalCandidatos }} candidatos inscritos</span>
            </div>
          </mat-card-content>
          <mat-card-actions>
            <button mat-raised-button color="primary" [routerLink]="['/inscricao', processo.id]" 
                    *ngIf="processo.status === 1">
              <mat-icon>how_to_reg</mat-icon>
              Inscrever-se
            </button>
            <button mat-raised-button disabled *ngIf="processo.status !== 1">
              <mat-icon>lock</mat-icon>
              Inscrições Encerradas
            </button>
          </mat-card-actions>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .processos-container {
      max-width: 1000px;
      margin: 0 auto;
      padding: 20px;
    }
    
    h1 {
      text-align: center;
      margin-bottom: 30px;
      color: #333;
    }
    
    .carregando, .sem-processos {
      text-align: center;
      padding: 40px;
      color: #666;
    }
    
    .carregando mat-icon, .sem-processos mat-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      margin-bottom: 10px;
    }
    
    .spin {
      animation: spin 1s linear infinite;
    }
    
    @keyframes spin {
      from { transform: rotate(0deg); }
      to { transform: rotate(360deg); }
    }
    
    .processos-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 20px;
    }
    
    .processo-card {
      height: 100%;
      display: flex;
      flex-direction: column;
    }
    
    mat-card-content {
      flex: 1;
    }
    
    .info-row {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-top: 10px;
      color: #666;
    }
    
    .info-row mat-icon {
      font-size: 18px;
      width: 18px;
      height: 18px;
    }
    
    .status-badge {
      padding: 4px 8px;
      border-radius: 4px;
      font-size: 0.8rem;
      font-weight: bold;
    }
    
    .status-0 { background: #6c757d; color: white; }
    .status-1 { background: #28a745; color: white; }
    .status-2 { background: #ffc107; color: #333; }
    .status-3 { background: #dc3545; color: white; }
    .status-4 { background: #6c757d; color: white; }
    
    mat-icon[mat-card-avatar] {
      color: #667eea;
    }
  `]
})
export class ProcessoPublicListComponent implements OnInit {
  processos: ProcessoSelecao[] = [];
  carregando = true;

  constructor(private service: ProcessoSelecaoService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({
      next: (data) => {
        // Filtra apenas processos abertos (status 1)
        this.processos = data.filter(p => p.status === StatusProcesso.Aberto);
        this.carregando = false;
      },
      error: (err) => {
        console.error('Erro ao carregar processos', err);
        this.carregando = false;
      }
    });
  }

  getStatusLabel(status: StatusProcesso): string {
    const labels = ['Rascunho', 'Aberto', 'Em Andamento', 'Finalizado', 'Cancelado'];
    return labels[status] || 'Desconhecido';
  }

  getStatusClass(status: StatusProcesso): string {
    const classes = ['default', 'info', 'warning', 'success', 'danger'];
    return classes[status] || 'default';
  }
}