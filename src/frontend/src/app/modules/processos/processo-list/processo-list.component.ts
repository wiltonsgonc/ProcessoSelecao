import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProcessoSelecaoService } from '../../../core/services/processo-selecao.service';
import { ProcessoSelecao, StatusProcesso } from '../../../core/models';

@Component({
  selector: 'app-processo-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-header">
      <h1>Processos de Seleção</h1>
      <button class="btn btn-primary" (click)="showForm = true">Novo Processo</button>
    </div>

    <div class="card" *ngIf="showForm">
      <h2>{{ editingId ? 'Editar' : 'Novo' }} Processo</h2>
      <form (ngSubmit)="save()">
        <div class="form-group">
          <label>Nome</label>
          <input type="text" class="form-control" [(ngModel)]="formData.nome" name="nome" required>
        </div>
        <div class="form-group">
          <label>Descrição</label>
          <textarea class="form-control" [(ngModel)]="formData.descricao" name="descricao"></textarea>
        </div>
        <div class="form-group">
          <label>Vagas Disponíveis</label>
          <input type="number" class="form-control" [(ngModel)]="formData.vagasDisponiveis" name="vagas" required>
        </div>
        <div class="form-group">
          <label>Data de Início</label>
          <input type="date" class="form-control" [(ngModel)]="formData.dataInicio" name="dataInicio">
        </div>
        <div class="form-group">
          <label>Data de Término</label>
          <input type="date" class="form-control" [(ngModel)]="formData.dataFim" name="dataFim">
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
              <th>Vagas</th>
              <th>Início</th>
              <th>Término</th>
              <th>Status</th>
              <th>Candidatos</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let processo of processos">
              <td>{{ processo.id }}</td>
              <td>{{ processo.nome }}</td>
              <td>{{ processo.vagasDisponiveis }}</td>
              <td>{{ processo.dataInicio | date:'dd/MM/yyyy' }}</td>
              <td>{{ processo.dataFim | date:'dd/MM/yyyy' }}</td>
              <td>
                <span [class]="'badge badge-' + getStatusClass(processo.status)">
                  {{ getStatusLabel(processo.status) }}
                </span>
              </td>
              <td>{{ processo.totalCandidatos }}</td>
              <td>
                <button class="btn btn-sm btn-primary" (click)="edit(processo)">Editar</button>
                <button class="btn btn-sm btn-info" (click)="copiarLink(processo.id)">Copiar Link</button>
                <button class="btn btn-sm btn-secondary" (click)="iniciar(processo.id)" 
                        *ngIf="processo.status === 0">Iniciar</button>
                <button class="btn btn-sm btn-danger" (click)="finalizar(processo.id)" 
                        *ngIf="processo.status === 1 || processo.status === 2">Finalizar</button>
                <button class="btn btn-sm btn-danger" (click)="remove(processo.id)">Excluir</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class ProcessoListComponent implements OnInit {
  processos: ProcessoSelecao[] = [];
  showForm = false;
  editingId: number | null = null;
  formData: any = { nome: '', descricao: '', vagasDisponiveis: 1, dataInicio: '', dataFim: '' };

  constructor(private service: ProcessoSelecaoService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({
      next: (data) => this.processos = data,
      error: (err) => console.error('Erro ao carregar processos', err)
    });
  }

  edit(processo: ProcessoSelecao) {
    this.editingId = processo.id;
    this.formData = { 
      nome: processo.nome, 
      descricao: processo.descricao, 
      vagasDisponiveis: processo.vagasDisponiveis,
      dataInicio: processo.dataInicio ? processo.dataInicio.split('T')[0] : '',
      dataFim: processo.dataFim ? processo.dataFim.split('T')[0] : ''
    };
    this.showForm = true;
  }

  save() {
    console.log('Salvando processo:', this.formData);
    const op = this.editingId 
      ? this.service.update(this.editingId, this.formData)
      : this.service.create(this.formData);
    
    op.subscribe({
      next: (result) => { 
        console.log('Sucesso:', result); 
        this.cancelForm(); 
        this.load(); 
      },
      error: (err) => { 
        console.error('Erro ao salvar', err);
        alert('Erro: ' + JSON.stringify(err));
      }
    });
  }

  cancelForm() {
    this.showForm = false;
    this.editingId = null;
    this.formData = { nome: '', descricao: '', vagasDisponiveis: 1, dataInicio: '', dataFim: '' };
  }

  iniciar(id: number) {
    this.service.iniciar(id).subscribe({
      next: () => this.load(),
      error: (err) => console.error('Erro ao iniciar', err)
    });
  }

  finalizar(id: number) {
    this.service.finalizar(id).subscribe({
      next: () => this.load(),
      error: (err) => console.error('Erro ao finalizar', err)
    });
  }

  remove(id: number) {
    if (confirm('Deseja excluir este processo?')) {
      this.service.delete(id).subscribe({
        next: () => this.load(),
        error: (err) => alert(err.error || 'Erro ao excluir')
      });
    }
  }

  copiarLink(id: number) {
    const url = `${window.location.origin}/inscricao/${id}`;
    navigator.clipboard.writeText(url).then(() => {
      alert('Link copiado para a área de transferência!');
    }).catch(() => {
      alert('Link: ' + url);
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
