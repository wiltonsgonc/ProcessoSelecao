import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Avaliador, CreateAvaliador, UpdateAvaliador } from '../models';

@Injectable({
  providedIn: 'root'
})
export class AvaliadorService {
  private endpoint = '/avaliadores';

  constructor(private api: ApiService) {}

  getAll() {
    return this.api.get<Avaliador[]>(this.endpoint);
  }

  getById(id: number) {
    return this.api.get<Avaliador>(`${this.endpoint}/${id}`);
  }

  getByProcessoId(processoId: number) {
    return this.api.get<Avaliador[]>(`${this.endpoint}/processo/${processoId}`);
  }

  create(data: CreateAvaliador) {
    return this.api.post<Avaliador>(this.endpoint, data);
  }

  update(id: number, data: UpdateAvaliador) {
    return this.api.put<Avaliador>(`${this.endpoint}/${id}`, data);
  }

  delete(id: number) {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
