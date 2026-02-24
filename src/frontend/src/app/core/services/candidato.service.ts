import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Candidato, CreateCandidato, UpdateCandidato } from '../models';

@Injectable({
  providedIn: 'root'
})
export class CandidatoService {
  private endpoint = '/candidatos';

  constructor(private api: ApiService) {}

  getAll() {
    return this.api.get<Candidato[]>(this.endpoint);
  }

  getById(id: number) {
    return this.api.get<Candidato>(`${this.endpoint}/${id}`);
  }

  getByProcessoId(processoId: number) {
    return this.api.get<Candidato[]>(`${this.endpoint}/processo/${processoId}`);
  }

  getPontuacao(id: number) {
    return this.api.get<number>(`${this.endpoint}/${id}/pontuacao`);
  }

  create(data: CreateCandidato) {
    return this.api.post<Candidato>(this.endpoint, data);
  }

  update(id: number, data: UpdateCandidato) {
    return this.api.put<Candidato>(`${this.endpoint}/${id}`, data);
  }

  delete(id: number) {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
