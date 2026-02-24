import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { ProcessoSelecao, CreateProcessoSelecao, UpdateProcessoSelecao } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ProcessoSelecaoService {
  private endpoint = '/processosselecao';

  constructor(private api: ApiService) {}

  getAll() {
    return this.api.get<ProcessoSelecao[]>(this.endpoint);
  }

  getById(id: number) {
    return this.api.get<ProcessoSelecao>(`${this.endpoint}/${id}`);
  }

  create(data: CreateProcessoSelecao) {
    return this.api.post<ProcessoSelecao>(this.endpoint, data);
  }

  update(id: number, data: UpdateProcessoSelecao) {
    return this.api.put<ProcessoSelecao>(`${this.endpoint}/${id}`, data);
  }

  iniciar(id: number) {
    return this.api.post<ProcessoSelecao>(`${this.endpoint}/${id}/iniciar`, {});
  }

  finalizar(id: number) {
    return this.api.post<ProcessoSelecao>(`${this.endpoint}/${id}/finalizar`, {});
  }

  delete(id: number) {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
