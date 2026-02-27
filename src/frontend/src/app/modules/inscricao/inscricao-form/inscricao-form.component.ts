import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatStepperModule } from '@angular/material/stepper';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatRadioModule } from '@angular/material/radio';

import { EditalService } from '../../../core/services/edital.service';
import { VagasService, Vaga } from '../../../core/services/vagas.service';
import { InscricaoService } from '../../../core/services/inscricao.service';
import { CreateInscricao } from '../../../core/models';

@Component({
  selector: 'app-inscricao-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatStepperModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    MatRadioModule
  ],
  template: `
    <div class="container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Inscrição</mat-card-title>
          <mat-card-subtitle *ngIf="vaga">{{vaga.edital}} - {{vaga.programa_curso_area}}</mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content>
          <div *ngIf="loading" class="loading">
            <mat-spinner diameter="50"></mat-spinner>
          </div>
          
          <div *ngIf="!loading && !vaga" class="no-edital">
            <p>Processo seletivo não encontrado ou inscrições encerradas.</p>
          </div>
          
          <mat-stepper *ngIf="!loading && vaga && vaga.status === 'aberto'" #stepper linear>
            <!-- Página 1 - Dados Iniciais -->
            <mat-step [stepControl]="pagina1Form" label="Dados Iniciais">
              <form [formGroup]="pagina1Form">
                <div class="form-grid">
                  <mat-form-field appearance="outline">
                    <mat-label>Nome Completo</mat-label>
                    <input matInput formControlName="nome" required>
                    <mat-error *ngIf="pagina1Form.get('nome')?.hasError('required')">Nome é obrigatório</mat-error>
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Data de Nascimento</mat-label>
                    <input matInput [matDatepicker]="picker" formControlName="dataNascimento" required>
                    <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
                    <mat-datepicker #picker></mat-datepicker>
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Tipo de Documento</mat-label>
                    <mat-select formControlName="tipoDocumento" required>
                      <mat-option value="CPF">CPF</mat-option>
                      <mat-option value="RG">RG</mat-option>
                      <mat-option value="Passaporte">Passaporte</mat-option>
                    </mat-select>
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Número do Documento</mat-label>
                    <input matInput formControlName="numeroDocumento" required>
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>E-mail</mat-label>
                    <input matInput type="email" formControlName="email" required>
                    <mat-error *ngIf="pagina1Form.get('email')?.hasError('email')">E-mail inválido</mat-error>
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Telefone 1</mat-label>
                    <input matInput formControlName="telefone1" required>
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Telefone 2</mat-label>
                    <input matInput formControlName="telefone2">
                  </mat-form-field>
                </div>
                
                <div class="privacy-section">
                  <p class="cookies-notice">
                    Nós utilizamos cookies para personalizar anúncios, gerar dados estatístico e garantir que você tenha a melhor experiência no SENAI!
                    <a href="#">Conheça a Política de Privacidade SENAI</a>.
                  </p>
                  <mat-checkbox formControlName="aceitaPoliticaPrivacidade" required>
                    Concordo com a Política de Privacidade SENAI
                  </mat-checkbox>
                </div>
                
                <div class="buttons">
                  <button mat-raised-button color="primary" matStepperNext [disabled]="!pagina1Form.valid">
                    Continuar
                  </button>
                </div>
              </form>
            </mat-step>
            
            <!-- Página 2 - Dados Pessoais -->
            <mat-step [stepControl]="pagina2Form" label="Dados Pessoais">
              <form [formGroup]="pagina2Form">
                <h3>Dados Pessoais</h3>
                <div class="form-grid">
                  <mat-form-field appearance="outline">
                    <mat-label>País de Nascimento</mat-label>
                    <input matInput formControlName="paisNatal">
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Estado de Nascimento</mat-label>
                    <input matInput formControlName="estadoNatal">
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Naturalidade</mat-label>
                    <input matInput formControlName="naturalidade">
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Nome Social</mat-label>
                    <input matInput formControlName="nomeSocial">
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Estado Civil</mat-label>
                    <mat-select formControlName="estadoCivil">
                      <mat-option value="">Selecione</mat-option>
                      <mat-option value="Solteiro">Solteiro(a)</mat-option>
                      <mat-option value="Casado">Casado(a)</mat-option>
                      <mat-option value="Divorciado">Divorciado(a)</mat-option>
                      <mat-option value="Viúvo">Viúvo(a)</mat-option>
                      <mat-option value="União Estável">União Estável</mat-option>
                    </mat-select>
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Nacionalidade</mat-label>
                    <input matInput formControlName="nacionalidade">
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Sexo</mat-label>
                    <mat-select formControlName="sexo">
                      <mat-option value="">Selecione</mat-option>
                      <mat-option value="Masculino">Masculino</mat-option>
                      <mat-option value="Feminino">Feminino</mat-option>
                      <mat-option value="Outro">Outro</mat-option>
                      <mat-option value="Não informar">Não informar</mat-option>
                    </mat-select>
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Cor/Raça</mat-label>
                    <mat-select formControlName="corRaca">
                      <mat-option value="">Selecione</mat-option>
                      <mat-option value="Branca">Branca</mat-option>
                      <mat-option value="Preta">Preta</mat-option>
                      <mat-option value="Parda">Parda</mat-option>
                      <mat-option value="Amarela">Amarela</mat-option>
                      <mat-option value="Indígena">Indígena</mat-option>
                      <mat-option value="Não declarar">Não declarar</mat-option>
                    </mat-select>
                  </mat-form-field>
                </div>
                
                <h3>Endereço</h3>
                <div class="form-grid">
                  <mat-form-field appearance="outline">
                    <mat-label>Telefone 1</mat-label>
                    <input matInput formControlName="telefone1" required>
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Telefone 2</mat-label>
                    <input matInput formControlName="telefone2">
                  </mat-form-field>
                </div>
                
                <h3>Informações Adicionais</h3>
                <div class="form-grid">
                  <mat-form-field appearance="outline">
                    <mat-label>Autorizo utilização de dados pessoais</mat-label>
                    <mat-select formControlName="autorizaDadosPessoais">
                      <mat-option value="Sim">Sim</mat-option>
                      <mat-option value="Não">Não</mat-option>
                    </mat-select>
                  </mat-form-field>
                </div>
                
                <h3>Dados de Estrangeiros</h3>
                <div class="form-grid">
                  <mat-form-field appearance="outline">
                    <mat-label>Tipo de Visto</mat-label>
                    <input matInput formControlName="tipoVisto">
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Número Registro Geral</mat-label>
                    <input matInput formControlName="numeroRegistroGeral">
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Data Vencimento RG</mat-label>
                    <input matInput [matDatepicker]="pickerRg" formControlName="dataVencimentoRg">
                    <mat-datepicker-toggle matSuffix [for]="pickerRg"></mat-datepicker-toggle>
                    <mat-datepicker #pickerRg></mat-datepicker>
                  </mat-form-field>
                </div>
                
                <div class="buttons">
                  <button mat-button matStepperPrevious>Anterior</button>
                  <button mat-raised-button color="primary" matStepperNext [disabled]="!pagina2Form.valid">
                    Próximo
                  </button>
                </div>
              </form>
            </mat-step>
            
            <!-- Página 3 - Opção de Interesse -->
            <mat-step [stepControl]="pagina3Form" label="Opção de Interesse">
              <form [formGroup]="pagina3Form">
                <h3>Processo Seletivo</h3>
                <div class="info-box">
                  <p><strong>Edital:</strong> {{vaga.edital}}</p>
                  <p><strong>Programa/Curso/Área:</strong> {{vaga.programa_curso_area}}</p>
                  <p><strong>Data da Inscrição:</strong> {{today | date:'dd/MM/yyyy'}}</p>
                  <p *ngIf="vaga.taxa_inscricao"><strong>Taxa de Inscrição:</strong> {{vaga.taxa_inscricao}}</p>
                </div>
                
                <h3>Deficiências</h3>
                <div class="deficiencias-grid">
                  <mat-checkbox formControlName="defFisica">Física</mat-checkbox>
                  <mat-checkbox formControlName="defAuditiva">Auditiva</mat-checkbox>
                  <mat-checkbox formControlName="defFala">Fala</mat-checkbox>
                  <mat-checkbox formControlName="defVisual">Visual</mat-checkbox>
                  <mat-checkbox formControlName="defMental">Mental</mat-checkbox>
                  <mat-checkbox formControlName="defIntelectual">Intelectual</mat-checkbox>
                  <mat-checkbox formControlName="defReabilitado">Reabilitado (BR)</mat-checkbox>
                  <mat-checkbox formControlName="defMultipla">Múltipla</mat-checkbox>
                </div>
                
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Motivo outras necessidades</mat-label>
                  <textarea matInput formControlName="defOutrasNecessidades" rows="3"></textarea>
                </mat-form-field>
                
                <div class="buttons">
                  <button mat-button matStepperPrevious>Anterior</button>
                  <button mat-raised-button color="primary" matStepperNext [disabled]="!pagina3Form.valid">
                    Próximo
                  </button>
                </div>
              </form>
            </mat-step>
            
            <!-- Página 4 - Documentos -->
            <mat-step label="Documentos">
              <h3>Documentos Obrigatórios</h3>
              <p class="info-text">Envie os documentos solicitados no edital. Arquivos em PDF, JPG ou PNG.</p>
              
              <div class="documentos-grid" *ngIf="vaga.anexos && vaga.anexos.length > 0">
                <div class="documento-item" *ngFor="let anexo of vaga.anexos">
                  <mat-label>{{anexo.nome_original || anexo.nome}}</mat-label>
                  <input type="file" (change)="onFileSelected($event, 'anexo_' + anexo.id)" accept=".pdf,.jpg,.jpeg,.png">
                  <p *ngIf="arquivos['anexo_' + anexo.id]" class="file-selected">
                    <mat-icon>check_circle</mat-icon> {{arquivos['anexo_' + anexo.id]!.name}}
                  </p>
                </div>
              </div>
              
              <div *ngIf="!vaga.anexos || vaga.anexos.length === 0" class="info-box">
                <p>Consulte o edital para verificar os documentos necessários.</p>
              </div>
              
              <div class="buttons">
                <button mat-button matStepperPrevious>Anterior</button>
                <button mat-raised-button color="primary" (click)="confirmar()" [disabled]="submitting">
                  {{submitting ? 'Enviando...' : 'Finalizar'}}
                </button>
              </div>
            </mat-step>
          </mat-stepper>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .container {
      padding: 20px;
      max-width: 900px;
      margin: 0 auto;
    }
    
    .loading, .no-edital {
      text-align: center;
      padding: 40px;
    }
    
    .form-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 16px;
      margin-top: 20px;
    }
    
    .full-width {
      width: 100%;
    }
    
    .privacy-section {
      margin-top: 20px;
      padding: 16px;
      background: #f5f5f5;
      border-radius: 4px;
    }
    
    .cookies-notice {
      font-size: 14px;
      margin-bottom: 16px;
    }
    
    .cookies-notice a {
      color: primary;
    }
    
    .info-box {
      background: #e3f2fd;
      padding: 16px;
      border-radius: 4px;
      margin: 16px 0;
    }
    
    .info-text {
      color: #666;
      margin-bottom: 16px;
    }
    
    .deficiencias-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 12px;
      margin: 16px 0;
    }
    
    .buttons {
      display: flex;
      justify-content: flex-end;
      gap: 12px;
      margin-top: 24px;
    }
    
    h3 {
      margin: 24px 0 12px;
      color: #333;
      border-bottom: 1px solid #eee;
      padding-bottom: 8px;
    }
    
    .documentos-grid {
      display: flex;
      flex-direction: column;
      gap: 16px;
      margin: 16px 0;
    }
    
    .documento-item {
      padding: 16px;
      background: #f9f9f9;
      border-radius: 8px;
      border: 1px solid #e0e0e0;
    }
    
    .documento-item mat-label {
      display: block;
      font-weight: 500;
      margin-bottom: 8px;
    }
    
    .documento-item input[type="file"] {
      display: block;
      margin-top: 8px;
    }
    
    .file-selected {
      color: green;
      display: flex;
      align-items: center;
      gap: 4px;
      margin-top: 8px;
    }
  `]
})
export class InscricaoFormComponent implements OnInit {
  vaga: Vaga | null = null;
  loading = true;
  submitting = false;
  today = new Date();
  
  pagina1Form!: FormGroup;
  pagina2Form!: FormGroup;
  pagina3Form!: FormGroup;
  
  arquivos: { [key: string]: File | null } = {
    rgCpf: null,
    anexoI: null,
    curriculoLattes: null,
    curriculoLattesOrientador: null,
    anexoII: null,
    comprovanteMatricula: null,
    historicoGraduacao: null
  };
  
  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private vagasService: VagasService,
    private edilService: EditalService,
    private inscricaoService: InscricaoService,
    private snackBar: MatSnackBar
  ) {
    this.initForms();
  }
  
  ngOnInit(): void {
    const edilId = this.route.snapshot.paramMap.get('editalId');
    if (edilId) {
      this.loadEdital(+edilId);
    } else {
      this.loading = false;
    }
  }
  
  private initForms(): void {
    this.pagina1Form = this.fb.group({
      nome: ['', Validators.required],
      dataNascimento: ['', Validators.required],
      tipoDocumento: ['', Validators.required],
      numeroDocumento: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      telefone1: ['', Validators.required],
      telefone2: [''],
      opcaoCursoId: [''],
      aceitaPoliticaPrivacidade: [false, Validators.requiredTrue]
    });
    
    this.pagina2Form = this.fb.group({
      paisNatal: [''],
      estadoNatal: [''],
      naturalidade: [''],
      nomeSocial: [''],
      estadoCivil: [''],
      nacionalidade: [''],
      sexo: [''],
      corRaca: [''],
      autorizaDadosPessoais: [''],
      tipoVisto: [''],
      numeroRegistroGeral: [''],
      dataVencimentoRg: ['']
    });
    
    this.pagina3Form = this.fb.group({
      formaInscricao: [null],
      localRealizacaoProva: [''],
      campusRealizacaoProva: [''],
      defFisica: [false],
      defAuditiva: [false],
      defFala: [false],
      defVisual: [false],
      defMental: [false],
      defIntelectual: [false],
      defReabilitado: [false],
      defMultipla: [false],
      defOutrasNecessidades: ['']
    });
  }
  
  private loadEdital(id: number): void {
    this.vagasService.getVagaById(id).subscribe({
      next: (vaga) => {
        this.vaga = vaga;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.snackBar.open('Processo seletivo não encontrado', 'Fechar', { duration: 3000 });
      }
    });
  }
  
  onFileSelected(event: Event, tipo: string): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.arquivos[tipo] = input.files[0];
    }
  }
  
  confirmar(): void {
    if (!this.pagina1Form.valid || !this.pagina2Form.valid || !this.pagina3Form.valid || !this.vaga) {
      this.snackBar.open('Preencha todos os campos obrigatórios', 'Fechar', { duration: 3000 });
      return;
    }
    
    this.submitting = true;
    
    const inscricaoData: CreateInscricao = {
      edilId: this.vaga.id,
      ...this.pagina1Form.value,
      ...this.pagina2Form.value,
      ...this.pagina3Form.value
    };
    
    this.inscricaoService.create(inscricaoData).subscribe({
      next: (inscricao) => {
        this.uploadDocumentos(inscricao.id);
      },
      error: (err) => {
        this.submitting = false;
        const msg = err.error?.message || 'Erro ao realizar inscrição';
        this.snackBar.open(msg, 'Fechar', { duration: 3000 });
      }
    });
  }
  
  private uploadDocumentos(inscricaoId: number): void {
    const tipoMap: { [key: string]: number } = {
      rgCpf: 0,
      anexoI: 1,
      curriculoLattes: 2,
      curriculoLattesOrientador: 3,
      anexoII: 4,
      comprovanteMatricula: 5,
      historicoGraduacao: 6
    };
    
    const uploads = Object.keys(this.arquivos).map(tipo => {
      const arquivo = this.arquivos[tipo];
      if (!arquivo) return null;
      
      const tipoDocumento = tipoMap[tipo];
      return this.inscricaoService.uploadDocumento(arquivo, inscricaoId, tipoDocumento).toPromise();
    }).filter(u => u !== null);
    
    if (uploads.length === 0) {
      this.submitting = false;
      this.snackBar.open('Inscrição realizada com sucesso!', 'Fechar', { duration: 3000 });
      this.router.navigate(['/inscricoes', inscricaoId]);
      return;
    }
    
    Promise.all(uploads).then(() => {
      this.submitting = false;
      this.snackBar.open('Inscrição realizada com sucesso!', 'Fechar', { duration: 3000 });
      this.router.navigate(['/inscricoes', inscricaoId]);
    }).catch(() => {
      this.submitting = false;
      this.snackBar.open('Inscrição criada, mas houve erro ao enviar documentos', 'Fechar', { duration: 3000 });
      this.router.navigate(['/inscricoes', inscricaoId]);
    });
  }
}
