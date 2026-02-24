import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CandidatoService } from '../../../core/services/candidato.service';
import { Candidato, StatusValidacao } from '../../../core/models';

@Component({
  selector: 'app-candidato-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-header">
      <h1>Candidatos</h1>
      <button class="btn btn-primary" (click)="showForm = true">Novo Candidato</button>
    </div>

    <div class="card" *ngIf="showForm">
      <h2>{{ editingId ? 'Editar' : 'Novo' }} Candidato</h2>
      <form (ngSubmit)="save()">
        <div class="form-group">
          <label>Nome</label>
          <input type="text" class="form-control" [(ngModel)]="formData.nome" name="nome" required>
        </div>
        <div class="form-group">
          <label>Matrícula</label>
          <input type="text" class="form-control" [(ngModel)]="formData.matricula" name="matricula" [readonly]="editingId !== null" required>
        </div>
        <div class="form-group">
          <label>Email</label>
          <input type="email" class="form-control" [(ngModel)]="formData.email" name="email" [readonly]="editingId !== null" required>
        </div>
        <div class="form-group">
          <label>Área de Pesquisa</label>
          <input type="text" class="form-control" [(ngModel)]="formData.areaPesquisa" name="areaPesquisa">
        </div>
        <div class="form-group">
          <label>ID do Processo</label>
          <input type="number" class="form-control" [(ngModel)]="formData.processoSelecaoId" name="processoId" [readonly]="editingId !== null" required>
        </div>
        <button type="submit" class="btn btn-primary">Salvar</button>
        <button type="button" class="btn btn-secondary" (click)="cancelForm()">Cancelar</button>
      </form>
    </div>

    <div class="card">
      <div class="table-container">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Nome</th>
              <th>Matrícula</th>
              <th>Email</th>
              <th>Status</th>
              <th>Pontuação</th>
              <th>Docs</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let candidato of candidatos">
              <td>{{ candidato.id }}</td>
              <td>{{ candidato.nome }}</td>
              <td>{{ candidato.matricula }}</td>
              <td>{{ candidato.email }}</td>
              <td>
                <span [class]="'badge badge-' + getStatusClass(candidato.statusValidacao)">
                  {{ getStatusLabel(candidato.statusValidacao) }}
                </span>
              </td>
              <td>{{ candidato.pontuacaoMedia.toFixed(2) }}</td>
              <td>{{ candidato.documentosValidados }}/{{ candidato.totalDocumentos }}</td>
              <td>
                <button class="btn btn-sm btn-primary" (click)="edit(candidato)">Editar</button>
                <button class="btn btn-sm btn-danger" (click)="remove(candidato.id)">Excluir</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class CandidatoListComponent implements OnInit {
  candidatos: Candidato[] = [];
  showForm = false;
  editingId: number | null = null;
  formData: any = { nome: '', matricula: '', email: '', areaPesquisa: '', processoSelecaoId: null };

  constructor(private service: CandidatoService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({
      next: (data) => this.candidatos = data,
      error: (err) => console.error('Erro ao carregar candidatos', err)
    });
  }

  edit(candidato: Candidato) {
    this.editingId = candidato.id;
    this.formData = {
      nome: candidato.nome,
      matricula: candidato.matricula,
      email: candidato.email,
      areaPesquisa: candidato.areaPesquisa,
      processoSelecaoId: candidato.processoSelecaoId
    };
    this.showForm = true;
  }

  save() {
    const op = this.editingId
      ? this.service.update(this.editingId, { nome: this.formData.nome, areaPesquisa: this.formData.areaPesquisa })
      : this.service.create(this.formData);
    
    op.subscribe({
      next: () => { this.cancelForm(); this.load(); },
      error: (err) => console.error('Erro ao salvar', err)
    });
  }

  cancelForm() {
    this.showForm = false;
    this.editingId = null;
    this.formData = { nome: '', matricula: '', email: '', areaPesquisa: '', processoSelecaoId: null };
  }

  remove(id: number) {
    if (confirm('Deseja excluir este candidato?')) {
      this.service.delete(id).subscribe({
        next: () => this.load(),
        error: (err) => console.error('Erro ao excluir', err)
      });
    }
  }

  getStatusLabel(status: StatusValidacao): string {
    const labels = ['Pendente', 'Validado', 'Rejeitado', 'Em Análise'];
    return labels[status] || 'Desconhecido';
  }

  getStatusClass(status: StatusValidacao): string {
    const classes = ['warning', 'success', 'danger', 'info'];
    return classes[status] || 'default';
  }
}
