import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BaremaService } from '../../../core/services/barema.service';
import { Barema, StatusBarema } from '../../../core/models';

@Component({
  selector: 'app-barema-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-header">
      <h1>Baremas</h1>
      <button class="btn btn-primary" (click)="showForm = true">Novo Barema</button>
    </div>

    <div class="card" *ngIf="showForm">
      <h2>Novo Barema</h2>
      <form (ngSubmit)="create()">
        <div class="form-group">
          <label>ID do Candidato</label>
          <input type="number" class="form-control" [(ngModel)]="formData.candidatoId" name="candidatoId" required>
        </div>
        <div class="form-group">
          <label>ID do Avaliador</label>
          <input type="number" class="form-control" [(ngModel)]="formData.avaliadorId" name="avaliadorId" required>
        </div>
        <button type="submit" class="btn btn-primary">Criar</button>
        <button type="button" class="btn btn-secondary" (click)="showForm = false">Cancelar</button>
      </form>
    </div>

    <div class="card" *ngIf="showFinalizarModal && selectedBarema">
      <h2>Finalizar Barema</h2>
      <form (ngSubmit)="finalizar()">
        <div class="form-group">
          <label>Candidato: {{ selectedBarema.candidatoNome }}</label>
        </div>
        <div class="form-group">
          <label>Nota para Originalidade (0-10)</label>
          <input type="number" class="form-control" [(ngModel)]="criterios.originalidade" name="originalidade" min="0" max="10" step="0.5">
        </div>
        <div class="form-group">
          <label>Nota para Relevância (0-10)</label>
          <input type="number" class="form-control" [(ngModel)]="criterios.relevancia" name="relevancia" min="0" max="10" step="0.5">
        </div>
        <div class="form-group">
          <label>Nota para Metodologia (0-10)</label>
          <input type="number" class="form-control" [(ngModel)]="criterios.metodologia" name="metodologia" min="0" max="10" step="0.5">
        </div>
        <div class="form-group">
          <label>Nota para Apresentação (0-10)</label>
          <input type="number" class="form-control" [(ngModel)]="criterios.apresentacao" name="apresentacao" min="0" max="10" step="0.5">
        </div>
        <div class="form-group">
          <label>Observações</label>
          <textarea class="form-control" [(ngModel)]="finalizarData.observacoes" name="observacoes"></textarea>
        </div>
        <button type="submit" class="btn btn-primary">Finalizar</button>
        <button type="button" class="btn btn-secondary" (click)="showFinalizarModal = false">Cancelar</button>
      </form>
    </div>

    <div class="card">
      <div class="table-container">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Candidato</th>
              <th>Avaliador</th>
              <th>Nota Final</th>
              <th>Status</th>
              <th>Data Preenchimento</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let barema of baremas">
              <td>{{ barema.id }}</td>
              <td>{{ barema.candidatoNome || barema.candidatoId }}</td>
              <td>{{ barema.avaliadorNome || barema.avaliadorId }}</td>
              <td>{{ barema.notaFinal.toFixed(2) }}</td>
              <td>
                <span [class]="'badge badge-' + getStatusClass(barema.status)">
                  {{ getStatusLabel(barema.status) }}
                </span>
              </td>
              <td>{{ barema.dataPreenchimento | date:'dd/MM/yyyy HH:mm' }}</td>
              <td>
                <button class="btn btn-sm btn-primary" (click)="openFinalizar(barema)" 
                        *ngIf="barema.status !== 2">Avaliar</button>
                <button class="btn btn-sm btn-danger" (click)="remove(barema.id)"
                        *ngIf="barema.status !== 2">Excluir</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class BaremaListComponent implements OnInit {
  baremas: Barema[] = [];
  showForm = false;
  showFinalizarModal = false;
  selectedBarema: Barema | null = null;
  formData: any = { candidatoId: null, avaliadorId: null };
  criterios: any = { originalidade: 0, relevancia: 0, metodologia: 0, apresentacao: 0 };
  finalizarData: any = { observacoes: '' };

  constructor(private service: BaremaService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({
      next: (data) => this.baremas = data,
      error: (err) => console.error('Erro ao carregar baremas', err)
    });
  }

  create() {
    this.service.create(this.formData).subscribe({
      next: () => { this.showForm = false; this.load(); },
      error: (err) => console.error('Erro ao criar barema', err)
    });
  }

  openFinalizar(barema: Barema) {
    this.selectedBarema = barema;
    this.criterios = barema.criterios || { originalidade: 0, relevancia: 0, metodologia: 0, apresentacao: 0 };
    this.finalizarData = { observacoes: barema.observacoes || '' };
    this.showFinalizarModal = true;
  }

  finalizar() {
    if (!this.selectedBarema) return;
    
    this.service.finalizar(this.selectedBarema.id, {
      criterios: this.criterios,
      observacoes: this.finalizarData.observacoes
    }).subscribe({
      next: () => { this.showFinalizarModal = false; this.load(); },
      error: (err) => console.error('Erro ao finalizar barema', err)
    });
  }

  remove(id: number) {
    if (confirm('Deseja excluir este barema?')) {
      this.service.delete(id).subscribe({
        next: () => this.load(),
        error: (err) => console.error('Erro ao excluir', err)
      });
    }
  }

  getStatusLabel(status: StatusBarema): string {
    const labels = ['Pendente', 'Em Preenchimento', 'Concluído', 'Cancelado'];
    return labels[status] || 'Desconhecido';
  }

  getStatusClass(status: StatusBarema): string {
    const classes = ['warning', 'info', 'success', 'danger'];
    return classes[status] || 'default';
  }
}
