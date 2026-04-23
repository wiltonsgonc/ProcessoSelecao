import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { DocumentoService } from '../../../core/services/documento.service';
import { Documento, TipoDocumento } from '../../../core/models';

@Component({
  selector: 'app-documento-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-header">
      <h1>Documentos</h1>
      <div class="header-actions">
        <button class="btn btn-secondary" *ngIf="hasSelectedDocuments()" (click)="downloadSelected()">
          Baixar Selecionados ({{ getSelectedCount() }})
        </button>
        <button class="btn btn-primary" (click)="showValidateModal = false; showForm = true">Upload Documento</button>
      </div>
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

    <div class="modal-overlay" *ngIf="showViewModal" (click)="closeViewModal()">
      <div class="modal-content modal-lg" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2>Visualizar Documento</h2>
          <button type="button" class="btn-close" (click)="closeViewModal()">&times;</button>
        </div>
        <div class="modal-body">
          <iframe *ngIf="viewUrl" [src]="viewUrl" frameborder="0" class="pdf-viewer"></iframe>
        </div>
      </div>
    </div>

    <div class="card" *ngFor="let grupo of documentosAgrupados">
      <div class="grupo-header" (click)="grupo.expandido = !grupo.expandido" style="cursor: pointer; display: flex; justify-content: space-between; align-items: center;">
        <h3>{{ grupo.candidatoNome || 'Candidato #' + grupo.candidatoId }} ({{ grupo.documentos.length }} documentos)</h3>
        <button type="button" class="btn btn-sm" [class.btn-secondary]="!grupo.expandido" [class.btn-primary]="grupo.expandido">
          {{ grupo.expandido ? 'Recolher' : 'Expandir' }}
        </button>
      </div>
      <div class="table-container" *ngIf="grupo.expandido">
        <table>
          <thead>
            <tr>
              <th>
                <input type="checkbox" (change)="toggleSelectAll($event)" />
              </th>
              <th>ID</th>
              <th>Nome do Arquivo</th>
              <th>Tipo</th>
              <th>Data Upload</th>
              <th>Status</th>
              <th>Motivo</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let doc of grupo.documentos">
              <td>
                <input type="checkbox" [checked]="isSelected(doc.id)" (change)="toggleSelect(doc.id)" />
              </td>
              <td>{{ doc.id }}</td>
              <td>{{ doc.nomeArquivo }}</td>
              <td>
                <a *ngIf="doc.linkUrl" [href]="doc.linkUrl" target="_blank" title="Abrir link">{{ doc.linkUrl }}</a>
                <span *ngIf="!doc.linkUrl">Documento</span>
              </td>
              <td>{{ doc.dataUpload | date:'dd/MM/yyyy HH:mm' }}</td>
              <td>
                <span [class]="getDocStatusClass(doc)">
                  {{ getDocStatusLabel(doc) }}
                </span>
              </td>
              <td>{{ doc.motivoRejeicao || '-' }}</td>
              <td>
                <button class="btn btn-sm btn-info" (click)="viewDocument(doc)">Visualizar</button>
                <button class="btn btn-sm btn-primary" (click)="openValidate(doc)">Validar</button>
                <button class="btn btn-sm btn-danger" (click)="remove(doc.id)">Excluir</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styleUrls: ['./documento-list.component.css']
})
export class DocumentoListComponent implements OnInit {
  documentos: Documento[] = [];
  documentosAgrupados: { candidatoId: number; candidatoNome?: string; documentos: Documento[]; expandido: boolean }[] = [];
  showForm = false;
  showValidateModal = false;
  showViewModal = false;
  selectedFile: File | null = null;
  selectedDocumento: Documento | null = null;
  formData: any = { tipo: 1, candidatoId: null };
  validateData: any = { validado: false, motivoRejeicao: '' };
  viewUrl: SafeResourceUrl | null = null;
  selectedIds: Set<number> = new Set();

  constructor(
    private service: DocumentoService,
    private sanitizer: DomSanitizer
  ) {}

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
    const grupos = new Map<number, { candidatoId: number; candidatoNome?: string; documentos: Documento[]; expandido: boolean }>();
    for (const doc of this.documentos) {
      if (!grupos.has(doc.candidatoId)) {
        grupos.set(doc.candidatoId, {
          candidatoId: doc.candidatoId,
          candidatoNome: doc.candidatoNome,
          documentos: [],
          expandido: false
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

  viewDocument(doc: Documento) {
    this.selectedDocumento = doc;
    
    // Se tiver LinkUrl, abrir em nova aba (Currículo Lattes ou outros)
    if (doc.linkUrl) {
      window.open(doc.linkUrl, '_blank');
      return;
    }
    
    // Para documentos sem linkUrl (PDF), abrir no modal
    const url = this.service.getViewUrl(doc.id);
    console.log('Visualizando documento:', doc.id, url);
    
    this.service.viewDocument(doc.id).subscribe({
      next: (blob) => {
        const blobUrl = URL.createObjectURL(blob);
        this.viewUrl = this.sanitizer.bypassSecurityTrustResourceUrl(blobUrl);
        this.showViewModal = true;
        console.log('PDF carregado com sucesso');
      },
      error: (err) => {
        console.error('Erro ao carregar PDF:', err);
        alert('Erro ao carregar documento: ' + err.message);
      }
    });
  }

  closeViewModal() {
    if (this.viewUrl) {
      // Liberar o blob URL para evitar memory leak
      try {
        const url = this.viewUrl.toString();
        if (url.startsWith('blob:')) {
          URL.revokeObjectURL(url);
        }
      } catch (e) {}
    }
    this.showViewModal = false;
    this.selectedDocumento = null;
    this.viewUrl = null;
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

  toggleSelect(id: number) {
    if (this.selectedIds.has(id)) {
      this.selectedIds.delete(id);
    } else {
      this.selectedIds.add(id);
    }
  }

  isSelected(id: number): boolean {
    return this.selectedIds.has(id);
  }

  toggleSelectAll(event: any) {
    const checked = event.target.checked;
    this.documentosAgrupados.forEach(grupo => {
      grupo.documentos.forEach(doc => {
        if (checked) {
          this.selectedIds.add(doc.id);
        } else {
          this.selectedIds.delete(doc.id);
        }
      });
    });
  }

  getSelectedCount(): number {
    return this.selectedIds.size;
  }

  hasSelectedDocuments(): boolean {
    return this.selectedIds.size > 0;
  }

  downloadSelected() {
    if (this.selectedIds.size === 0) return;
    
    const ids = Array.from(this.selectedIds);
    this.service.downloadMultiple(ids).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'documentos_selecionados.zip';
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
      },
      error: (err) => console.error('Erro ao baixar documentos', err)
    });
  }

  getTipoLabel(tipo: TipoDocumento): string {
    const labels = ['', 'Histórico Escolar', 'Comprovante Matrícula', 'Carta Intenção', 'Curriculum Lattes', 'Carta Recomendação'];
    return labels[tipo] || 'Desconhecido';
  }

  getDocStatusLabel(doc: Documento): string {
    if (doc.validado) return 'Validado';
    if (doc.motivoRejeicao) return 'Rejeitado';
    return 'Pendente';
  }

  getDocStatusClass(doc: Documento): string {
    if (doc.validado) return 'badge badge-success';
    if (doc.motivoRejeicao) return 'badge badge-danger';
    return 'badge badge-warning';
  }
}
