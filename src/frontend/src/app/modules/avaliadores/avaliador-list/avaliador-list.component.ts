import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AvaliadorService } from '../../../core/services/avaliador.service';
import { Avaliador, TipoAvaliador } from '../../../core/models';

@Component({
  selector: 'app-avaliador-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-header">
      <h1>Avaliadores</h1>
      <button class="btn btn-primary" (click)="showForm = true">Novo Avaliador</button>
    </div>

    <div class="card" *ngIf="showForm">
      <h2>{{ editingId ? 'Editar' : 'Novo' }} Avaliador</h2>
      <form (ngSubmit)="save()">
        <div class="form-group">
          <label>Nome</label>
          <input type="text" class="form-control" [(ngModel)]="formData.nome" name="nome" required>
        </div>
        <div class="form-group">
          <label>Email</label>
          <input type="email" class="form-control" [(ngModel)]="formData.email" name="email" [readonly]="editingId !== null" required>
        </div>
        <div class="form-group">
          <label>Tipo</label>
          <select class="form-control" [(ngModel)]="formData.tipo" name="tipo" [disabled]="editingId !== null">
            <option [ngValue]="1">Interno</option>
            <option [ngValue]="2">Externo</option>
          </select>
        </div>
        <div class="form-group">
          <label>Área de Especialização</label>
          <input type="text" class="form-control" [(ngModel)]="formData.areaEspecializacao" name="area">
        </div>
        <div class="form-group">
          <label>Instituição</label>
          <input type="text" class="form-control" [(ngModel)]="formData.instituicao" name="instituicao">
        </div>
        <div class="form-group" *ngIf="!editingId">
          <label>ID do Processo (opcional)</label>
          <input type="number" class="form-control" [(ngModel)]="formData.processoSelecaoId" name="processoId">
        </div>
        <div class="form-group" *ngIf="editingId">
          <label>
            <input type="checkbox" [(ngModel)]="formData.ativo" name="ativo"> Ativo
          </label>
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
              <th>Email</th>
              <th>Tipo</th>
              <th>Instituição</th>
              <th>Status</th>
              <th>Pendentes</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let avaliador of avaliadores">
              <td>{{ avaliador.id }}</td>
              <td>{{ avaliador.nome }}</td>
              <td>{{ avaliador.email }}</td>
              <td>
                <span [class]="'badge badge-' + (avaliador.tipo === 1 ? 'info' : 'warning')">
                  {{ avaliador.tipo === 1 ? 'Interno' : 'Externo' }}
                </span>
              </td>
              <td>{{ avaliador.instituicao || '-' }}</td>
              <td>
                <span [class]="'badge badge-' + (avaliador.ativo ? 'success' : 'danger')">
                  {{ avaliador.ativo ? 'Ativo' : 'Inativo' }}
                </span>
              </td>
              <td>{{ avaliador.avaliacoesPendentes }}</td>
              <td>
                <button class="btn btn-sm btn-primary" (click)="edit(avaliador)">Editar</button>
                <button class="btn btn-sm btn-danger" (click)="remove(avaliador.id)">Excluir</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class AvaliadorListComponent implements OnInit {
  avaliadores: Avaliador[] = [];
  showForm = false;
  editingId: number | null = null;
  formData: any = { nome: '', email: '', tipo: 1, areaEspecializacao: '', instituicao: '', processoSelecaoId: null, ativo: true };

  constructor(private service: AvaliadorService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({
      next: (data) => this.avaliadores = data,
      error: (err) => console.error('Erro ao carregar avaliadores', err)
    });
  }

  edit(avaliador: Avaliador) {
    this.editingId = avaliador.id;
    this.formData = {
      nome: avaliador.nome,
      email: avaliador.email,
      tipo: avaliador.tipo,
      areaEspecializacao: avaliador.areaEspecializacao,
      instituicao: avaliador.instituicao,
      ativo: avaliador.ativo
    };
    this.showForm = true;
  }

  save() {
    if (this.editingId) {
      this.service.update(this.editingId, {
        nome: this.formData.nome,
        areaEspecializacao: this.formData.areaEspecializacao,
        instituicao: this.formData.instituicao,
        ativo: this.formData.ativo
      }).subscribe({
        next: () => { this.cancelForm(); this.load(); },
        error: (err) => console.error('Erro ao atualizar', err)
      });
    } else {
      this.service.create(this.formData).subscribe({
        next: () => { this.cancelForm(); this.load(); },
        error: (err) => console.error('Erro ao criar', err)
      });
    }
  }

  cancelForm() {
    this.showForm = false;
    this.editingId = null;
    this.formData = { nome: '', email: '', tipo: 1, areaEspecializacao: '', instituicao: '', processoSelecaoId: null, ativo: true };
  }

  remove(id: number) {
    if (confirm('Deseja excluir este avaliador?')) {
      this.service.delete(id).subscribe({
        next: () => this.load(),
        error: (err) => console.error('Erro ao excluir', err)
      });
    }
  }
}
