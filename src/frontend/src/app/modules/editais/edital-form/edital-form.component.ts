import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { EditalService } from '../../../core/services/edital.service';
import { Edital, OpcaoCurso, StatusEdital } from '../../../core/models';

@Component({
  selector: 'app-edital-form',
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
    MatDatepickerModule,
    MatNativeDateModule,
    MatCheckboxModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  template: `
    <div class="container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>{{ isEdit ? 'Editar Edital' : 'Novo Edital' }}</mat-card-title>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <h3>Dados do Edital</h3>
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Título</mat-label>
              <input matInput formControlName="titulo" placeholder="Ex: Edital 042/2025">
              <mat-error *ngIf="form.get('titulo')?.hasError('required')">Título é obrigatório</mat-error>
            </mat-form-field>
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Descrição</mat-label>
              <textarea matInput formControlName="descricao" rows="3"></textarea>
            </mat-form-field>
            
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Data de Publicação</mat-label>
                <input matInput [matDatepicker]="pickerPublicacao" formControlName="dataPublicacao">
                <mat-datepicker-toggle matSuffix [for]="pickerPublicacao"></mat-datepicker-toggle>
                <mat-datepicker #pickerPublicacao></mat-datepicker>
              </mat-form-field>
              
              <mat-form-field appearance="outline">
                <mat-label>Início das Inscrições</mat-label>
                <input matInput [matDatepicker]="pickerInicio" formControlName="dataInicioInscricao">
                <mat-datepicker-toggle matSuffix [for]="pickerInicio"></mat-datepicker-toggle>
                <mat-datepicker #pickerInicio></mat-datepicker>
              </mat-form-field>
              
              <mat-form-field appearance="outline">
                <mat-label>Fim das Inscrições</mat-label>
                <input matInput [matDatepicker]="pickerFim" formControlName="dataFimInscricao">
                <mat-datepicker-toggle matSuffix [for]="pickerFim"></mat-datepicker-toggle>
                <mat-datepicker #pickerFim></mat-datepicker>
              </mat-form-field>
            </div>
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Valor da Inscrição (R$)</mat-label>
              <input matInput type="number" formControlName="valorInscricao" placeholder="0,00">
            </mat-form-field>
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Texto do Edital</mat-label>
              <textarea matInput formControlName="textoEdital" rows="6" placeholder="Cole o conteúdo completo do edital"></textarea>
            </mat-form-field>
            
            <h3>Configurações de Inscrição</h3>
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Locais de Prova (um por linha)</mat-label>
              <textarea matInput formControlName="locaisProva" rows="3" placeholder="São Paulo&#10;Campinas&#10;Ribeirão Preto"></textarea>
            </mat-form-field>
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Campus (um por linha)</mat-label>
              <textarea matInput formControlName="campi" rows="3" placeholder="Campus São Paulo&#10;Campus Campinas"></textarea>
            </mat-form-field>
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Formas de Inscrição (um por linha)</mat-label>
              <textarea matInput formControlName="formasInscricao" rows="2" placeholder="Online&#10;Presencial"></textarea>
            </mat-form-field>
            
            <h3>Documentos Obrigatórios</h3>
            
            <div class="documentos-grid">
              <mat-checkbox formControlName="exigeRgCpf">RG e CPF do Candidato</mat-checkbox>
              <mat-checkbox formControlName="exigeAnexoI">Anexo I do Edital</mat-checkbox>
              <mat-checkbox formControlName="exigeCurriculoLattes">Currículo Lattes do Candidato</mat-checkbox>
              <mat-checkbox formControlName="exigeCurriculoLattesOrientador">Currículo Lattes do Orientador</mat-checkbox>
              <mat-checkbox formControlName="exigeAnexoII">Anexo II (ver edital)</mat-checkbox>
              <mat-checkbox formControlName="exigeComprovanteMatricula">Comprovante de Matrícula</mat-checkbox>
              <mat-checkbox formControlName="exigeHistoricoGraduacao">Histórico Escolar Graduação</mat-checkbox>
            </div>
            
            <h3>Cursos Ofertados</h3>
            
            <div formArrayName="opcoesCurso">
              <div *ngFor="let curso of opcoesCurso.controls; let i = index" [formGroupName]="i" class="curso-card">
                <div class="curso-header">
                  <span>Curso {{ i + 1 }}</span>
                  <button mat-icon-button color="warn" type="button" (click)="removeCurso(i)" *ngIf="opcoesCurso.length > 1">
                    <mat-icon>delete</mat-icon>
                  </button>
                </div>
                
                <div class="form-row">
                  <mat-form-field appearance="outline">
                    <mat-label>Nome do Curso</mat-label>
                    <input matInput formControlName="nome" placeholder="Ex: Mestrado em Engenharia">
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline">
                    <mat-label>Vagas</mat-label>
                    <input matInput type="number" formControlName="vagas">
                  </mat-form-field>
                </div>
                
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Descrição</mat-label>
                  <textarea matInput formControlName="descricao" rows="2"></textarea>
                </mat-form-field>
              </div>
            </div>
            
            <button mat-stroked-button type="button" (click)="addCurso()" class="add-curso-btn">
              <mat-icon>add</mat-icon> Adicionar Curso
            </button>
            
            <div class="form-actions">
              <button mat-button type="button" routerLink="/admin/editais">Cancelar</button>
              <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid || saving">
                <mat-spinner diameter="20" *ngIf="saving"></mat-spinner>
                <span *ngIf="!saving">{{ isEdit ? 'Salvar' : 'Criar' }}</span>
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .container { padding: 20px; max-width: 900px; margin: 0 auto; }
    .full-width { width: 100%; }
    .form-row { display: flex; gap: 16px; flex-wrap: wrap; }
    .form-row mat-form-field { flex: 1; min-width: 200px; }
    h3 { margin: 24px 0 16px; color: #333; border-bottom: 1px solid #eee; padding-bottom: 8px; }
    .documentos-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 8px; margin-bottom: 16px; }
    .curso-card { background: #f9f9f9; padding: 16px; border-radius: 8px; margin-bottom: 16px; }
    .curso-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; font-weight: 500; }
    .add-curso-btn { margin-top: 8px; }
    .form-actions { display: flex; justify-content: flex-end; gap: 12px; margin-top: 24px; padding-top: 16px; border-top: 1px solid #eee; }
  `]
})
export class EditalFormComponent implements OnInit {
  form!: FormGroup;
  isEdit = false;
  saving = false;
  editingId: number | null = null;
  
  constructor(
    private fb: FormBuilder,
    private edilService: EditalService,
    private snackBar: MatSnackBar,
    private router: Router,
    private route: ActivatedRoute
  ) {}
  
  ngOnInit(): void {
    this.initForm();
    
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.editingId = +id;
      this.loadEdital(+id);
    }
  }
  
  get opcoesCurso(): FormArray {
    return this.form.get('opcoesCurso') as FormArray;
  }
  
  private initForm(): void {
    this.form = this.fb.group({
      titulo: ['', Validators.required],
      descricao: [''],
      dataPublicacao: [new Date(), Validators.required],
      dataInicioInscricao: [new Date(), Validators.required],
      dataFimInscricao: [new Date(), Validators.required],
      valorInscricao: [null],
      textoEdital: [''],
      locaisProva: [''],
      campi: [''],
      formasInscricao: [''],
      exigeRgCpf: [true],
      exigeAnexoI: [true],
      exigeCurriculoLattes: [true],
      exigeCurriculoLattesOrientador: [false],
      exigeAnexoII: [false],
      exigeComprovanteMatricula: [false],
      exigeHistoricoGraduacao: [false],
      opcoesCurso: this.fb.array([this.createCursoGroup()])
    });
  }
  
  private createCursoGroup(): FormGroup {
    return this.fb.group({
      nome: ['', Validators.required],
      descricao: [''],
      vagas: [1, Validators.required],
      campus: [''],
      localProva: ['']
    });
  }
  
  addCurso(): void {
    this.opcoesCurso.push(this.createCursoGroup());
  }
  
  removeCurso(index: number): void {
    this.opcoesCurso.removeAt(index);
  }
  
  private loadEdital(id: number): void {
    this.edilService.getById(id).subscribe({
      next: (edital) => {
        this.form.patchValue({
          titulo: edital.titulo,
          descricao: edital.descricao,
          dataPublicacao: new Date(edital.dataPublicacao),
          dataInicioInscricao: new Date(edital.dataInicioInscricao),
          dataFimInscricao: new Date(edital.dataFimInscricao),
          valorInscricao: edital.valorInscricao,
          textoEdital: edital.textoEdital,
          locaisProva: edital.locaisProva?.join('\n') || '',
          campi: edital.campi?.join('\n') || '',
          formasInscricao: edital.formasInscricao?.join('\n') || '',
          exigeRgCpf: edital.exigeRgCpf,
          exigeAnexoI: edital.exigeAnexoI,
          exigeCurriculoLattes: edital.exigeCurriculoLattes,
          exigeCurriculoLattesOrientador: edital.exigeCurriculoLattesOrientador,
          exigeAnexoII: edital.exigeAnexoII,
          exigeComprovanteMatricula: edital.exigeComprovanteMatricula,
          exigeHistoricoGraduacao: edital.exigeHistoricoGraduacao
        });
        
        this.opcoesCurso.clear();
        for (const curso of edital.opcoesCurso || []) {
          this.opcoesCurso.push(this.fb.group({
            nome: [curso.nome, Validators.required],
            descricao: [curso.descricao || ''],
            vagas: [curso.vagas, Validators.required],
            campus: [curso.campus || ''],
            localProva: [curso.localProva || '']
          }));
        }
        
        if (this.opcoesCurso.length === 0) {
          this.opcoesCurso.push(this.createCursoGroup());
        }
      },
      error: () => {
        this.snackBar.open('Erro ao carregar edital', 'Fechar', { duration: 3000 });
        this.router.navigate(['/admin/editais']);
      }
    });
  }
  
  onSubmit(): void {
    if (this.form.invalid) return;
    
    this.saving = true;
    const formValue = this.form.value;
    
    const dto: any = {
      titulo: formValue.titulo,
      descricao: formValue.descricao,
      dataPublicacao: formValue.dataPublicacao,
      dataInicioInscricao: formValue.dataInicioInscricao,
      dataFimInscricao: formValue.dataFimInscricao,
      valorInscricao: formValue.valorInscricao,
      textoEdital: formValue.textoEdital,
      locaisProva: formValue.locaisProva?.split('\n').filter((l: string) => l.trim()) || [],
      campi: formValue.campi?.split('\n').filter((c: string) => c.trim()) || [],
      formasInscricao: formValue.formasInscricao?.split('\n').filter((f: string) => f.trim()) || [],
      exigeRgCpf: formValue.exigeRgCpf,
      exigeAnexoI: formValue.exigeAnexoI,
      exigeCurriculoLattes: formValue.exigeCurriculoLattes,
      exigeCurriculoLattesOrientador: formValue.exigeCurriculoLattesOrientador,
      exigeAnexoII: formValue.exigeAnexoII,
      exigeComprovanteMatricula: formValue.exigeComprovanteMatricula,
      exigeHistoricoGraduacao: formValue.exigeHistoricoGraduacao,
      opcoesCurso: formValue.opcoesCurso.map((c: any) => ({
        nome: c.nome,
        descricao: c.descricao,
        vagas: c.vagas,
        campus: c.campus,
        localProva: c.localProva
      }))
    };
    
    if (this.isEdit && this.editingId) {
      dto.id = this.editingId;
      dto.status = StatusEdital.Rascunho;
      
      this.edilService.update(this.editingId, dto).subscribe({
        next: () => {
          this.saving = false;
          this.snackBar.open('Edital atualizado com sucesso', 'Fechar', { duration: 3000 });
          this.router.navigate(['/admin/editais']);
        },
        error: () => {
          this.saving = false;
          this.snackBar.open('Erro ao atualizar edital', 'Fechar', { duration: 3000 });
        }
      });
    } else {
      this.edilService.create(dto).subscribe({
        next: () => {
          this.saving = false;
          this.snackBar.open('Edital criado com sucesso', 'Fechar', { duration: 3000 });
          this.router.navigate(['/admin/editais']);
        },
        error: () => {
          this.saving = false;
          this.snackBar.open('Erro ao criar edital', 'Fechar', { duration: 3000 });
        }
      });
    }
  }
}
