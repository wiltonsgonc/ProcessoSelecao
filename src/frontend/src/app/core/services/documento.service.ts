import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Documento, CreateDocumento, ValidateDocumento } from '../models';

@Injectable({
  providedIn: 'root'
})
export class DocumentoService {
  private endpoint = '/documentos';

  constructor(private api: ApiService) {}

  getAll() {
    return this.api.get<Documento[]>(this.endpoint);
  }

  getById(id: number) {
    return this.api.get<Documento>(`${this.endpoint}/${id}`);
  }

  getByCandidatoId(candidatoId: number) {
    return this.api.get<Documento[]>(`${this.endpoint}/candidato/${candidatoId}`);
  }

  upload(file: File, data: CreateDocumento) {
    const formData = new FormData();
    formData.append('arquivo', file);
    formData.append('tipo', data.tipo.toString());
    formData.append('nomeArquivo', data.nomeArquivo);
    formData.append('candidatoId', data.candidatoId.toString());
    return this.api.uploadFile<Documento>(this.endpoint, formData);
  }

  validate(id: number, data: ValidateDocumento) {
    return this.api.put<Documento>(`${this.endpoint}/${id}/validar`, data);
  }

  delete(id: number) {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
