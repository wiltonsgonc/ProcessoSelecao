import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';

import { InscricaoService } from '../../../core/services/inscricao.service';
import { Inscricao, DocumentoInscricao, TipoDocumentoInscricao, StatusInscricao } from '../../../core/models';

@Component({
  selector: 'app-inscricao-detail',
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
    MatSnackBarModule,
    MatDividerModule
  ],
  template: `
    <div class="container">
      <mat-card *ngIf="loading" class="loading-card">
        <mat-spinner diameter="50"></mat-spinner>
      </mat-card>
      
      <mat-card *ngIf="!loading && inscricao">
        <mat-card-header>
          <mat-card-title>Inscrição #{{inscricao.id}}</mat-card-title>
          <mat-card-subtitle>{{inscricao.nomeEdital}}</mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content>
          <mat-chip [color]="getStatusColor(inscricao.status)" selected>
            {{getStatusLabel(inscricao.status)}}
          </mat-chip>
          
          <h3>Dados do Candidato</h3>
          <div class="info-grid">
            <div><strong>Nome:</strong> {{inscricao.nome}}</div>
            <div><strong>CPF:</strong> {{inscricao.numeroDocumento}}</div>
            <div><strong>Email:</strong> {{inscricao.email}}</div>
            <div><strong>Telefone:</strong> {{inscricao.telefone1}}</div>
            <div><strong>Data Nascimento:</strong> {{inscricao.dataNascimento | date:'dd/MM/yyyy'}}</div>
            <div><strong>Data Inscrição:</strong> {{inscricao.dataInscricao | date:'dd/MM/yyyy HH:mm'}}</div>
          </div>
          
          <mat-divider></mat-divider>
          
          <h3>Documentos</h3>
          <div class="documentos-list">
            <div *ngFor="let doc of documentosObrigatorios" class="documento-item">
              <div class="doc-info">
                <strong>{{getTipoLabel(doc.tipo)}}</strong>
                <span *ngIf="doc.id">Enviado: {{doc.nomeArquivoOriginal}}</span>
                <span *ngIf="!doc.id" class="pending">Pendente</span>
              </div>
              <div class="doc-actions">
                <button *ngIf="doc.id" mat-icon-button color="warn" (click)="deleteDocumento(doc.id!)">
                  <mat-icon>delete</mat-icon>
                </button>
                <label *ngIf="!doc.id" class="upload-btn">
                  <input type="file" (change)="uploadDocumento($event, doc.tipo)" accept="application/pdf" hidden>
                  <mat-icon>upload</mat-icon>
                </label>
              </div>
            </div>
          </div>
          
          <div *ngIf="inscricao.status === 0" class="actions">
            <button mat-raised-button color="primary" (click)="confirmarInscricao()" [disabled]="submitting">
              {{submitting ? 'Enviando...' : 'Confirmar Inscrição'}}
            </button>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .container { padding: 20px; max-width: 800px; margin: 0 auto; }
    .loading-card { display: flex; justify-content: center; padding: 40px; }
    .info-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 12px; margin: 16px 0; }
    .documentos-list { margin: 16px 0; }
    .documento-item { display: flex; justify-content: space-between; align-items: center; padding: 12px; background: #f5f5f5; margin-bottom: 8px; border-radius: 4px; }
    .doc-info { display: flex; flex-direction: column; }
    .doc-info .pending { color: #f44336; }
    .upload-btn { cursor: pointer; color: #1976d2; }
    .actions { margin-top: 24px; text-align: center; }
    h3 { margin-top: 24px; margin-bottom: 12px; }
    mat-divider { margin: 16px 0; }
  `]
})
export class InscricaoDetailComponent implements OnInit {
  inscricao: Inscricao | null = null;
  documentosEnviados: DocumentoInscricao[] = [];
  documentosObrigatorios: { tipo: number; id?: number; nomeArquivoOriginal?: string }[] = [];
  loading = true;
  submitting = false;
  
  constructor(
    private route: ActivatedRoute,
    private inscricaoService: InscricaoService,
    private snackBar: MatSnackBar
  ) {}
  
  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadInscricao(+id);
    }
  }
  
  loadInscricao(id: number): void {
    this.inscricaoService.getById(id).subscribe({
      next: (data) => {
        this.inscricao = data;
        this.documentosEnviados = data.documentos || [];
        this.initDocumentosObrigatorios();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.snackBar.open('Inscrição não encontrada', 'Fechar', { duration: 3000 });
      }
    });
  }
  
  initDocumentosObrigatorios(): void {
    const tipos = [
      { tipo: 0, nome: 'RG e CPF Candidato' },
      { tipo: 1, nome: 'Anexo I do Edital' },
      { tipo: 2, nome: 'Currículo Lattes do Candidato' },
      { tipo: 3, nome: 'Currículo Lattes do Orientador' },
      { tipo: 4, nome: 'Anexo II' },
      { tipo: 5, nome: 'Comprovante de Matrícula' },
      { tipo: 6, nome: 'Histórico Escolar Graduação' }
    ];
    
    this.documentosObrigatorios = tipos.map(t => {
      const enviado = this.documentosEnviados.find(d => d.tipo === t.tipo);
      return {
        tipo: t.tipo,
        id: enviado?.id,
        nomeArquivoOriginal: enviado?.nomeArquivoOriginal
      };
    });
  }
  
  uploadDocumento(event: any, tipo: number): void {
    const file = event.target.files[0];
    if (!file) return;
    
    if (file.type !== 'application/pdf') {
      this.snackBar.open('Apenas arquivos PDF são permitidos', 'Fechar', { duration: 3000 });
      return;
    }
    
    this.submitting = true;
    this.inscricaoService.uploadDocumento(file, this.inscricao!.id, tipo).subscribe({
      next: () => {
        this.submitting = false;
        this.snackBar.open('Documento enviado com sucesso', 'Fechar', { duration: 3000 });
        this.loadInscricao(this.inscricao!.id);
      },
      error: () => {
        this.submitting = false;
        this.snackBar.open('Erro ao enviar documento', 'Fechar', { duration: 3000 });
      }
    });
  }
  
  deleteDocumento(id: number): void {
    this.submitting = true;
    this.inscricaoService.deleteDocumento(id).subscribe({
      next: () => {
        this.submitting = false;
        this.snackBar.open('Documento excluído', 'Fechar', { duration: 3000 });
        this.loadInscricao(this.inscricao!.id);
      },
      error: () => {
        this.submitting = false;
        this.snackBar.open('Erro ao excluir documento', 'Fechar', { duration: 3000 });
      }
    });
  }
  
  confirmarInscricao(): void {
    this.submitting = true;
    this.inscricaoService.confirmar(this.inscricao!.id).subscribe({
      next: () => {
        this.submitting = false;
        this.snackBar.open('Inscrição confirmada com sucesso', 'Fechar', { duration: 3000 });
        this.loadInscricao(this.inscricao!.id);
      },
      error: (err) => {
        this.submitting = false;
        this.snackBar.open(err.error?.message || 'Erro ao confirmar inscrição', 'Fechar', { duration: 3000 });
      }
    });
  }
  
  getStatusLabel(status: StatusInscricao): string {
    const labels: { [key: number]: string } = {
      0: 'Pendente',
      1: 'Completa',
      2: 'Confirmada',
      3: 'Cancelada'
    };
    return labels[status] || 'Desconhecido';
  }
  
  getStatusColor(status: StatusInscricao): string {
    const colors: { [key: number]: string } = {
      0: 'warn',
      1: 'accent',
      2: 'primary',
      3: 'warn'
    };
    return colors[status] || 'primary';
  }
  
  getTipoLabel(tipo: number): string {
    const labels: { [key: number]: string } = {
      0: 'RG e CPF Candidato',
      1: 'Anexo I do Edital',
      2: 'Currículo Lattes do Candidato',
      3: 'Currículo Lattes do Orientador',
      4: 'Anexo II',
      5: 'Comprovante de Matrícula',
      6: 'Histórico Escolar Graduação'
    };
    return labels[tipo] || 'Documento';
  }
}
