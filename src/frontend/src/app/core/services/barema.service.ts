import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Barema, CreateBarema, UpdateBarema, FinalizarBarema } from '../models';

@Injectable({
  providedIn: 'root'
})
export class BaremaService {
  private endpoint = '/baremas';

  constructor(private api: ApiService) {}

  getAll() {
    return this.api.get<Barema[]>(this.endpoint);
  }

  getById(id: number) {
    return this.api.get<Barema>(`${this.endpoint}/${id}`);
  }

  getByCandidatoId(candidatoId: number) {
    return this.api.get<Barema[]>(`${this.endpoint}/candidato/${candidatoId}`);
  }

  getByAvaliadorId(avaliadorId: number) {
    return this.api.get<Barema[]>(`${this.endpoint}/avaliador/${avaliadorId}`);
  }

  create(data: CreateBarema) {
    return this.api.post<Barema>(this.endpoint, data);
  }

  updateCriterios(id: number, data: UpdateBarema) {
    return this.api.put<Barema>(`${this.endpoint}/${id}/criterios`, data);
  }

  finalizar(id: number, data: FinalizarBarema) {
    return this.api.post<Barema>(`${this.endpoint}/${id}/finalizar`, data);
  }

  delete(id: number) {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
