import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { EditalService } from '../../../core/services/edital.service';
import { Edital, StatusEdital } from '../../../core/models';

@Component({
  selector: 'app-edital-list-admin',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  template: `
    <div class="container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Editais</mat-card-title>
          <mat-card-subtitle>Gerenciamento de editais</mat-card-subtitle>
          <button mat-raised-button color="primary" routerLink="/admin/editais/novo" class="btn-novo">
            <mat-icon>add</mat-icon> Novo Edital
          </button>
        </mat-card-header>
        
        <mat-card-content>
          <div *ngIf="loading" class="loading">
            <mat-spinner diameter="40"></mat-spinner>
          </div>
          
          <table *ngIf="!loading && editais.length > 0" mat-table [dataSource]="editais" class="full-width">
            <ng-container matColumnDef="titulo">
              <th mat-header-cell *matHeaderCellDef>Título</th>
              <td mat-cell *matCellDef="let edil">{{edil.titulo}}</td>
            </ng-container>
            
            <ng-container matColumnDef="dataInicio">
              <th mat-header-cell *matHeaderCellDef>Início</th>
              <td mat-cell *matCellDef="let edil">{{edil.dataInicioInscricao | date:'dd/MM/yyyy'}}</td>
            </ng-container>
            
            <ng-container matColumnDef="dataFim">
              <th mat-header-cell *matHeaderCellDef>Fim</th>
              <td mat-cell *matCellDef="let edil">{{edil.dataFimInscricao | date:'dd/MM/yyyy'}}</td>
            </ng-container>
            
            <ng-container matColumnDef="valor">
              <th mat-header-cell *matHeaderCellDef>Valor</th>
              <td mat-cell *matCellDef="let edil">{{edil.valorInscricao | currency:'BRL'}}</td>
            </ng-container>
            
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let edil">
                <mat-chip [color]="getStatusColor(edil.status)" selected>
                  {{getStatusLabel(edil.status)}}
                </mat-chip>
              </td>
            </ng-container>
            
            <ng-container matColumnDef="acoes">
              <th mat-header-cell *matHeaderCellDef>Ações</th>
              <td mat-cell *matCellDef="let edil">
                <button mat-icon-button color="primary" [routerLink]="['/admin/editais', edil.id]" title="Editar">
                  <mat-icon>edit</mat-icon>
                </button>
                <button mat-icon-button color="primary" [routerLink]="['/inscricoes/novo', edil.id]" *ngIf="edil.estaAberto" title="Ver inscrição">
                  <mat-icon>visibility</mat-icon>
                </button>
                <button mat-icon-button color="accent" (click)="publicar(edil)" *ngIf="edil.status === 0" title="Publicar">
                  <mat-icon>publish</mat-icon>
                </button>
                <button mat-icon-button color="warn" (click)="encerrar(edil)" *ngIf="edil.status === 1" title="Encerrar">
                  <mat-icon>stop</mat-icon>
                </button>
                <button mat-icon-button color="warn" (click)="delete(edil)" *ngIf="edil.status === 0" title="Excluir">
                  <mat-icon>delete</mat-icon>
                </button>
              </td>
            </ng-container>
            
            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
          
          <div *ngIf="!loading && editais.length === 0" class="no-data">
            <p>Nenhum edital encontrado.</p>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .container { padding: 20px; }
    .loading, .no-data { text-align: center; padding: 40px; }
    .full-width { width: 100%; }
    mat-card-header { display: flex; justify-content: space-between; align-items: center; }
    .btn-novo { margin-left: auto; }
  `]
})
export class EditalListAdminComponent implements OnInit {
  editais: Edital[] = [];
  loading = true;
  displayedColumns = ['titulo', 'dataInicio', 'dataFim', 'valor', 'status', 'acoes'];
  
  constructor(
    private edilService: EditalService,
    private snackBar: MatSnackBar
  ) {}
  
  ngOnInit(): void {
    this.loadEditais();
  }
  
  loadEditais(): void {
    this.edilService.getAll().subscribe({
      next: (data) => {
        this.editais = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
  
  getStatusLabel(status: StatusEdital): string {
    const labels: { [key: number]: string } = {
      0: 'Rascunho',
      1: 'Publicado',
      2: 'Encerrado',
      3: 'Cancelado'
    };
    return labels[status] || 'Desconhecido';
  }
  
  getStatusColor(status: StatusEdital): string {
    const colors: { [key: number]: string } = {
      0: 'warn',
      1: 'primary',
      2: 'accent',
      3: 'warn'
    };
    return colors[status] || 'primary';
  }
  
  publicar(edital: Edital): void {
    this.edilService.publicar(edital.id).subscribe({
      next: () => {
        this.snackBar.open('Edital publicado com sucesso', 'Fechar', { duration: 3000 });
        this.loadEditais();
      },
      error: () => {
        this.snackBar.open('Erro ao publicar edital', 'Fechar', { duration: 3000 });
      }
    });
  }
  
  encerrar(edital: Edital): void {
    this.edilService.encerrar(edital.id).subscribe({
      next: () => {
        this.snackBar.open('Edital encerrado com sucesso', 'Fechar', { duration: 3000 });
        this.loadEditais();
      },
      error: () => {
        this.snackBar.open('Erro ao encerrar edital', 'Fechar', { duration: 3000 });
      }
    });
  }
  
  delete(edital: Edital): void {
    if (confirm(`Tem certeza que deseja excluir o edital "${edital.titulo}"?`)) {
      this.edilService.delete(edital.id).subscribe({
        next: () => {
          this.snackBar.open('Edital excluído com sucesso', 'Fechar', { duration: 3000 });
          this.loadEditais();
        },
        error: () => {
          this.snackBar.open('Erro ao excluir edital', 'Fechar', { duration: 3000 });
        }
      });
    }
  }
}
