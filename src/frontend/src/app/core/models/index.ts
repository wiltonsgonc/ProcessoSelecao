export enum TipoDocumento {
  HistoricoEscolar = 1,
  ComprovanteMatricula = 2,
  CartaIntencao = 3,
  CurriculumLatte = 4,
  CartaRecomendacao = 5
}

export enum StatusValidacao {
  Pendente = 0,
  Validado = 1,
  Rejeitado = 2,
  EmAnalise = 3
}

export enum TipoAvaliador {
  Interno = 1,
  Externo = 2
}

export enum StatusBarema {
  Pendente = 0,
  EmPreenchimento = 1,
  Concluido = 2,
  Cancelado = 3
}

export enum StatusProcesso {
  Rascunho = 0,
  Aberto = 1,
  EmAndamento = 2,
  Finalizado = 3,
  Cancelado = 4
}

export interface Candidato {
  id: number;
  nome: string;
  matricula: string;
  email: string;
  areaPesquisa?: string;
  statusValidacao: StatusValidacao;
  dataCadastro: string;
  processoSelecaoId: number;
  pontuacaoMedia: number;
  totalDocumentos: number;
  documentosValidados: number;
}

export interface CreateCandidato {
  nome: string;
  matricula: string;
  email: string;
  areaPesquisa?: string;
  processoSelecaoId: number;
}

export interface UpdateCandidato {
  nome: string;
  areaPesquisa?: string;
}

export interface Documento {
  id: number;
  tipo: TipoDocumento;
  nomeArquivo: string;
  dataUpload: string;
  validado: boolean;
  motivoRejeicao?: string;
  candidatoId: number;
}

export interface CreateDocumento {
  tipo: TipoDocumento;
  nomeArquivo: string;
  candidatoId: number;
}

export interface ValidateDocumento {
  validado: boolean;
  motivoRejeicao?: string;
}

export interface Avaliador {
  id: number;
  nome: string;
  email: string;
  tipo: TipoAvaliador;
  areaEspecializacao?: string;
  instituicao?: string;
  ativo: boolean;
  avaliacoesPendentes: number;
}

export interface CreateAvaliador {
  nome: string;
  email: string;
  tipo: TipoAvaliador;
  areaEspecializacao?: string;
  instituicao?: string;
  processoSelecaoId?: number;
}

export interface UpdateAvaliador {
  nome: string;
  areaEspecializacao?: string;
  instituicao?: string;
  ativo: boolean;
}

export interface Barema {
  id: number;
  candidatoId: number;
  candidatoNome?: string;
  avaliadorId: number;
  avaliadorNome?: string;
  criterios?: { [key: string]: number };
  notaFinal: number;
  observacoes?: string;
  dataPreenchimento?: string;
  status: StatusBarema;
}

export interface CreateBarema {
  candidatoId: number;
  avaliadorId: number;
}

export interface UpdateBarema {
  criterios: { [key: string]: number };
  observacoes?: string;
}

export interface FinalizarBarema {
  criterios: { [key: string]: number };
  observacoes?: string;
}

export interface ProcessoSelecao {
  id: number;
  nome: string;
  descricao?: string;
  dataInicio: string;
  dataFim?: string;
  vagasDisponiveis: number;
  status: StatusProcesso;
  totalCandidatos: number;
  totalAvaliadores: number;
}

export interface CreateProcessoSelecao {
  nome: string;
  descricao?: string;
  vagasDisponiveis: number;
}

export interface UpdateProcessoSelecao {
  nome: string;
  descricao?: string;
  vagasDisponiveis: number;
}
