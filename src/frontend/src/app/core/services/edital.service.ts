import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Edital, CreateEdital, OpcaoCurso } from '../models';

@Injectable({
  providedIn: 'root'
})
export class EditalService {
  private endpoint = '/editais';

  constructor(private api: ApiService) {}

  getAll(): Observable<Edital[]> {
    return this.api.get<Edital[]>(this.endpoint);
  }

  getPublished(): Observable<Edital[]> {
    return this.api.get<Edital[]>(`${this.endpoint}/publicados`);
  }

  getById(id: number): Observable<Edital> {
    return this.api.get<Edital>(`${this.endpoint}/${id}`);
  }

  create(data: CreateEdital): Observable<Edital> {
    return this.api.post<Edital>(this.endpoint, data);
  }

  update(id: number, data: any): Observable<Edital> {
    return this.api.put<Edital>(`${this.endpoint}/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  publicar(id: number): Observable<any> {
    return this.api.post<any>(`${this.endpoint}/${id}/publicar`, {});
  }

  encerrar(id: number): Observable<any> {
    return this.api.post<any>(`${this.endpoint}/${id}/encerrar`, {});
  }

  getOpcoesCurso(editalId: number): Observable<OpcaoCurso[]> {
    return this.api.get<OpcaoCurso[]>(`${this.endpoint}/${editalId}/opcoes-curso`);
  }
}
