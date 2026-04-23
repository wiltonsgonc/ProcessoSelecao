import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';

export interface DadosPagina1 {
  nome?: string;
  dataNascimento?: string;
  tipoDocumento?: string;
  numeroDocumento?: string;
  email?: string;
  telefone?: string;
  areaOfertada?: string;
  politicaPrivacidade?: boolean;
}

export interface DadosPagina2 {
  nome?: string;
  dataNascimento?: string;
  paisNatal?: string;
  estadoNatal?: string;
  naturalidade?: string;
  nomeSocial?: string;
  estadoCivil?: string;
  nacionalidade?: string;
  email?: string;
  sexo?: string;
  cpf?: string;
  telefone1?: string;
  telefone2?: string;
  corRaca?: string;
  autorizacaoDados?: string;
  tipoVisto?: string;
  numeroRegistroGeral?: string;
  dataVencimentoRG?: string;
}

export interface DadosPagina3 {
  rgCpfCandidato?: File | null;
  anexoI?: File | null;
  curriculoLattesCandidato?: string | null;
  curriculoLattesOrientador?: string | null;
  anexoII?: File | null;
  comprovanteMatricula?: File | null;
  historicoEscolar?: File | null;
}

export interface DadosPagina4 {
  processoSeletivo?: string;
  areaOfertada?: string;
  formaInscricao?: string;
  localProva?: string;
  campusProva?: string;
  dataInscricao?: string;
  valorInscricao?: number;
  deficienciaFisica?: boolean;
  deficienciaAuditiva?: boolean;
  deficienciaFala?: boolean;
  deficienciaVisual?: boolean;
  deficienciaMental?: boolean;
  deficienciaIntelectual?: boolean;
  deficienciaReabilitado?: boolean;
  deficienciaMultipla?: boolean;
  motivoOutrasNecessidades?: string;
}

@Injectable({
  providedIn: 'root'
})
export class FormularioService {
  private apiUrl = 'http://localhost:5002/api/formulario';

  private dadosPagina1 = new BehaviorSubject<DadosPagina1>({});
  private dadosPagina2 = new BehaviorSubject<DadosPagina2>({});
  private dadosPagina3 = new BehaviorSubject<DadosPagina3>({});
  private dadosPagina4 = new BehaviorSubject<DadosPagina4>({});
  private paginaAtual = new BehaviorSubject<number>(1);

  dadosPagina1$ = this.dadosPagina1.asObservable();
  dadosPagina2$ = this.dadosPagina2.asObservable();
  dadosPagina3$ = this.dadosPagina3.asObservable();
  dadosPagina4$ = this.dadosPagina4.asObservable();
  paginaAtual$ = this.paginaAtual.asObservable();

  constructor(private http: HttpClient) { }

  getPaginaAtual(): number {
    return this.paginaAtual.value;
  }

  setPaginaAtual(pagina: number) {
    this.paginaAtual.next(pagina);
  }

  salvarPagina1(dados: DadosPagina1) {
    this.dadosPagina1.next({ ...this.dadosPagina1.value, ...dados });
  }

  salvarPagina2(dados: DadosPagina2) {
    this.dadosPagina2.next({ ...this.dadosPagina2.value, ...dados });
  }

  salvarPagina3(dados: DadosPagina3) {
    this.dadosPagina3.next({ ...this.dadosPagina3.value, ...dados });
  }

  salvarPagina4(dados: DadosPagina4) {
    this.dadosPagina4.next({ ...this.dadosPagina4.value, ...dados });
  }

  getDadosCompletos() {
    return {
      pagina1: this.dadosPagina1.value,
      pagina2: this.dadosPagina2.value,
      pagina4: this.dadosPagina4.value
    };
  }

  getDadosPagina3() {
    return this.dadosPagina3.value;
  }

  limparDados() {
    this.dadosPagina1.next({});
    this.dadosPagina2.next({});
    this.dadosPagina3.next({});
    this.dadosPagina4.next({});
    this.paginaAtual.next(1);
  }

  enviarPagina1(dados: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/pagina1`, dados);
  }

  enviarPagina2(dados: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/pagina2`, dados);
  }

  enviarPagina3(dados: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/pagina3`, dados);
  }

  enviarPagina4(dados: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/pagina4`, dados);
  }

  enviarInscricaoCompleta(processoSelecaoId: number): Observable<any> {
    const dados = this.getDadosCompletos();
    const dadosPagina3 = this.getDadosPagina3();
    
    const formData = new FormData();
    formData.append('processoSelecaoId', processoSelecaoId.toString());
    formData.append('dados', JSON.stringify(dados));
    
    if (dadosPagina3.rgCpfCandidato) {
      formData.append('rgCpfCandidato', dadosPagina3.rgCpfCandidato);
    }
    if (dadosPagina3.anexoI) {
      formData.append('anexoI', dadosPagina3.anexoI);
    }
    if (dadosPagina3.curriculoLattesCandidato) {
      formData.append('curriculoLattesCandidato', dadosPagina3.curriculoLattesCandidato);
    }
    if (dadosPagina3.curriculoLattesOrientador) {
      formData.append('curriculoLattesOrientador', dadosPagina3.curriculoLattesOrientador);
    }
    if (dadosPagina3.anexoII) {
      formData.append('anexoII', dadosPagina3.anexoII);
    }
    if (dadosPagina3.comprovanteMatricula) {
      formData.append('comprovanteMatricula', dadosPagina3.comprovanteMatricula);
    }
    if (dadosPagina3.historicoEscolar) {
      formData.append('historicoEscolar', dadosPagina3.historicoEscolar);
    }

    return this.http.post(`${this.apiUrl}/completa`, formData);
  }
}
