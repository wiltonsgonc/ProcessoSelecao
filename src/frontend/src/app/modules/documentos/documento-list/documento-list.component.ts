import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DocumentoService } from '../../../core/services/documento.service';
import { Documento, TipoDocumento } from '../../../core/models';

@Component({
  selector: 'app-documento-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-header">
      <h1>Documentos</h1>
      <button class="btn btn-primary" (click)="showValidateModal = false; showForm = true">Upload Documento</button>
    </div>

    <div class="card" *ngIf="showForm">
      <h2>Upload de Documento</h2>
      <form (ngSubmit)="upload()">
        <div class="form-group">
          <label>Arquivo</label>
          <input type="file" class="form-control" (change)="onFileSelected($event)" required>
        </div>
        <div class="form-group">
          <label>Tipo</label>
          <select class="form-control" [(ngModel)]="formData.tipo" name="tipo" required>
            <option [ngValue]="1">Histórico Escolar</option>
            <option [ngValue]="2">Comprovante de Matrícula</option>
            <option [ngValue]="3">Carta de Intenção</option>
            <option [ngValue]="4">Curriculum Lattes</option>
            <option [ngValue]="5">Carta de Recomendação</option>
          </select>
        </div>
        <div class="form-group">
          <label>ID do Candidato</label>
          <input type="number" class="form-control" [(ngModel)]="formData.candidatoId" name="candidatoId" required>
        </div>
        <button type="submit" class="btn btn-primary">Upload</button>
        <button type="button" class="btn btn-secondary" (click)="showForm = false">Cancelar</button>
      </form>
    </div>

    <div class="card" *ngIf="showValidateModal && selectedDocumento">
      <h2>Validar Documento</h2>
      <form (ngSubmit)="validate()">
        <div class="form-group">
          <label>Documento: {{ selectedDocumento.nomeArquivo }}</label>
        </div>
        <div class="form-group">
          <label>
            <input type="checkbox" [(ngModel)]="validateData.validado" name="validado"> Aprovado
          </label>
        </div>
        <div class="form-group" *ngIf="!validateData.validado">
          <label>Motivo da Rejeição</label>
          <textarea class="form-control" [(ngModel)]="validateData.motivoRejeicao" name="motivo"></textarea>
        </div>
        <button type="submit" class="btn btn-primary">Salvar</button>
        <button type="button" class="btn btn-secondary" (click)="showValidateModal = false">Cancelar</button>
      </form>
    </div>

    <div class="card" *ngFor="let grupo of documentosAgrupados">
      <h3>{{ grupo.candidatoNome || 'Candidato #' + grupo.candidatoId }}</h3>
      <div class="table-container">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Nome do Arquivo</th>
              <th>Tipo</th>
              <th>Data Upload</th>
              <th>Status</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let doc of grupo.documentos">
              <td>{{ doc.id }}</td>
              <td>{{ doc.nomeArquivo }}</td>
              <td>{{ getTipoLabel(doc.tipo) }}</td>
              <td>{{ doc.dataUpload | date:'dd/MM/yyyy HH:mm' }}</td>
              <td>
                <span [class]="'badge badge-' + (doc.validado ? 'success' : 'warning')">
                  {{ doc.validado ? 'Validado' : 'Pendente' }}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-primary" (click)="openValidate(doc)">Validar</button>
                <button class="btn btn-sm btn-danger" (click)="remove(doc.id)">Excluir</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class DocumentoListComponent implements OnInit {
  documentos: Documento[] = [];
  documentosAgrupados: { candidatoId: number; candidatoNome?: string; documentos: Documento[] }[] = [];
  showForm = false;
  showValidateModal = false;
  selectedFile: File | null = null;
  selectedDocumento: Documento | null = null;
  formData: any = { tipo: 1, candidatoId: null };
  validateData: any = { validado: false, motivoRejeicao: '' };

  constructor(private service: DocumentoService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({
      next: (data) => {
        this.documentos = data;
        this.agruparDocumentos();
      },
      error: (err) => console.error('Erro ao carregar documentos', err)
    });
  }

  agruparDocumentos() {
    const grupos = new Map<number, { candidatoId: number; candidatoNome?: string; documentos: Documento[] }>();
    for (const doc of this.documentos) {
      if (!grupos.has(doc.candidatoId)) {
        grupos.set(doc.candidatoId, {
          candidatoId: doc.candidatoId,
          candidatoNome: doc.candidatoNome,
          documentos: []
        });
      }
      grupos.get(doc.candidatoId)!.documentos.push(doc);
    }
    this.documentosAgrupados = Array.from(grupos.values());
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
    this.formData.nomeArquivo = this.selectedFile?.name || '';
  }

  upload() {
    if (!this.selectedFile) return;
    
    this.service.upload(this.selectedFile, this.formData).subscribe({
      next: () => { this.showForm = false; this.load(); },
      error: (err) => console.error('Erro ao fazer upload', err)
    });
  }

  openValidate(doc: Documento) {
    this.selectedDocumento = doc;
    this.validateData = { validado: doc.validado, motivoRejeicao: doc.motivoRejeicao || '' };
    this.showValidateModal = true;
  }

  validate() {
    if (!this.selectedDocumento) return;
    
    this.service.validate(this.selectedDocumento.id, this.validateData).subscribe({
      next: () => { this.showValidateModal = false; this.load(); },
      error: (err) => console.error('Erro ao validar', err)
    });
  }

  remove(id: number) {
    if (confirm('Deseja excluir este documento?')) {
      this.service.delete(id).subscribe({
        next: () => this.load(),
        error: (err) => console.error('Erro ao excluir', err)
      });
    }
  }

  getTipoLabel(tipo: TipoDocumento): string {
    const labels = ['', 'Histórico Escolar', 'Comprovante Matrícula', 'Carta Intenção', 'Curriculum Lattes', 'Carta Recomendação'];
    return labels[tipo] || 'Desconhecido';
  }
}
