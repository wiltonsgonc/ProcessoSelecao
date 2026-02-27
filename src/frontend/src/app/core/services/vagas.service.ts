import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Vaga {
  id: number;
  edital: string;
  setor: string;
  tipo: string;
  programa_curso_area: string;
  data_limite: string;
  data_limite_formatada: string;
  numero_de_vagas: number;
  taxa_inscricao: string;
  mensalidade_bolsa: string;
  email_responsavel: string;
  descricao: string;
  link_inscricao: string;
  status: string;
  arquivo_edital: string | null;
  nome_original_edital: string | null;
  anexos: VagaAnexo[];
  created_at: string;
}

export interface VagaAnexo {
  id: number;
  nome: string;
  nome_original: string;
  tipo: string;
}

@Injectable({
  providedIn: 'root'
})
export class VagasService {
  private apiUrl = environment.vagasApiUrl;

  constructor(private http: HttpClient) {}

  getVagas(status: string = 'aberto'): Observable<Vaga[]> {
    return this.http.get<Vaga[]>(`${this.apiUrl}/vagas?status=${status}`);
  }

  getVagaById(id: number): Observable<Vaga> {
    return this.http.get<Vaga>(`${this.apiUrl}/vagas/${id}`);
  }

  getVagaAbertaById(id: number): Observable<Vaga> {
    return this.http.get<Vaga>(`${this.apiUrl}/vagas/${id}?status=aberto`);
  }
}
