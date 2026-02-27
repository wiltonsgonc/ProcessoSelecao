import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { VagasService, Vaga } from '../../../core/services/vagas.service';

@Component({
  selector: 'app-edital-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Processos Seletivos</mat-card-title>
          <mat-card-subtitle>Lista de processos seletivos abertos para inscrição</mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content>
          <div *ngIf="loading" class="loading">
            <mat-spinner diameter="40"></mat-spinner>
          </div>
          
          <div *ngIf="error" class="error">
            <p>Erro ao carregar editais. Tente novamente mais tarde.</p>
          </div>
          
          <table *ngIf="!loading && !error && vagas.length > 0" mat-table [dataSource]="vagas" class="full-width">
            <ng-container matColumnDef="edital">
              <th mat-header-cell *matHeaderCellDef>Edital</th>
              <td mat-cell *matCellDef="let vaga">{{vaga.edital}}</td>
            </ng-container>
            
            <ng-container matColumnDef="programa">
              <th mat-header-cell *matHeaderCellDef>Programa/Curso/Área</th>
              <td mat-cell *matCellDef="let vaga">{{vaga.programa_curso_area}}</td>
            </ng-container>
            
            <ng-container matColumnDef="vagas">
              <th mat-header-cell *matHeaderCellDef>Vagas</th>
              <td mat-cell *matCellDef="let vaga">{{vaga.numero_de_vagas}}</td>
            </ng-container>
            
            <ng-container matColumnDef="dataLimite">
              <th mat-header-cell *matHeaderCellDef>Data Limite</th>
              <td mat-cell *matCellDef="let vaga">{{vaga.data_limite_formatada}}</td>
            </ng-container>
            
            <ng-container matColumnDef="taxa">
              <th mat-header-cell *matHeaderCellDef>Taxa</th>
              <td mat-cell *matCellDef="let vaga">{{vaga.taxa_inscricao}}</td>
            </ng-container>
            
            <ng-container matColumnDef="acoes">
              <th mat-header-cell *matHeaderCellDef>Ações</th>
              <td mat-cell *matCellDef="let vaga">
                <button mat-raised-button color="primary" [routerLink]="['/inscricoes/novo', vaga.id]">
                  <mat-icon>edit</mat-icon> Inscrever-se
                </button>
              </td>
            </ng-container>
            
            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
          
          <div *ngIf="!loading && !error && vagas.length === 0" class="no-data">
            <p>Nenhum processo seletivo aberto no momento.</p>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .container { padding: 20px; }
    .loading, .no-data, .error { text-align: center; padding: 40px; }
    .error { color: red; }
    .full-width { width: 100%; }
  `]
})
export class EditalListComponent implements OnInit {
  vagas: Vaga[] = [];
  loading = true;
  error = false;
  displayedColumns = ['edital', 'programa', 'vagas', 'dataLimite', 'taxa', 'acoes'];
  
  constructor(private vagasService: VagasService) {}
  
  ngOnInit(): void {
    this.loadVagas();
  }
  
  loadVagas(): void {
    this.vagasService.getVagas('aberto').subscribe({
      next: (data) => {
        this.vagas = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = true;
      }
    });
  }
}
