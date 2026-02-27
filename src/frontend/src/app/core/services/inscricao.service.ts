import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Inscricao, CreateInscricao, DocumentoInscricao } from '../models';

@Injectable({
  providedIn: 'root'
})
export class InscricaoService {
  private endpoint = '/inscricoes';

  constructor(private api: ApiService) {}

  getAll(): Observable<Inscricao[]> {
    return this.api.get<Inscricao[]>(this.endpoint);
  }

  getByEdital(editalId: number): Observable<Inscricao[]> {
    return this.api.get<Inscricao[]>(`${this.endpoint}/edital/${editalId}`);
  }

  getById(id: number): Observable<Inscricao> {
    return this.api.get<Inscricao>(`${this.endpoint}/${id}`);
  }

  create(data: CreateInscricao): Observable<Inscricao> {
    return this.api.post<Inscricao>(this.endpoint, data);
  }

  update(id: number, data: CreateInscricao): Observable<Inscricao> {
    return this.api.put<Inscricao>(`${this.endpoint}/${id}`, data);
  }

  confirmar(id: number): Observable<any> {
    return this.api.post<any>(`${this.endpoint}/${id}/confirmar`, {});
  }

  cancelar(id: number): Observable<any> {
    return this.api.post<any>(`${this.endpoint}/${id}/cancelar`, {});
  }

  validarDocumentos(id: number): Observable<any> {
    return this.api.post<any>(`${this.endpoint}/${id}/validar-documentos`, {});
  }

  uploadDocumento(file: File, inscricaoId: number, tipoDocumento: number): Observable<DocumentoInscricao> {
    const formData = new FormData();
    formData.append('file', file);
    return this.api.uploadFile<DocumentoInscricao>(
      `/documentosinscricao/upload?inscricaoId=${inscricaoId}&tipoDocumento=${tipoDocumento}`,
      formData
    );
  }

  getDocumentos(inscricaoId: number): Observable<DocumentoInscricao[]> {
    return this.api.get<DocumentoInscricao[]>(`/documentosinscricao/inscricao/${inscricaoId}`);
  }

  deleteDocumento(id: number): Observable<void> {
    return this.api.delete<void>(`/documentosinscricao/${id}`);
  }
}
