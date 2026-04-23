import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormularioService, DadosPagina3 } from '../../../core/services/formulario.service';

@Component({
  selector: 'app-pagina3',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './pagina3.component.html',
  styleUrls: ['./pagina3.component.css']
})
export class Pagina3Component implements OnInit {
  @Output() proxima = new EventEmitter<void>();
  @Output() anterior = new EventEmitter<void>();

  form: FormGroup;

  constructor(
    private fb: FormBuilder, 
    private formularioService: FormularioService
  ) {
    this.form = this.fb.group({
      rgCpfCandidato: [null, Validators.required],
      anexoI: [null, Validators.required],
      curriculoLattesCandidato: ['', [Validators.required, Validators.pattern(/^https?:\/\/.+/)]],
      curriculoLattesOrientador: ['', [Validators.required, Validators.pattern(/^https?:\/\/.+/)]],
      anexoII: [null, Validators.required],
      comprovanteMatricula: [null, Validators.required],
      historicoEscolar: [null, Validators.required]
    });
  }

  ngOnInit() {
    this.formularioService.dadosPagina3$.subscribe(dados => {
      if (dados && Object.keys(dados).length > 0) {
        this.form.patchValue(dados);
      }
    });
  }

  onFileChange(event: any, controlName: string) {
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      this.form.patchValue({
        [controlName]: file
      });
    }
  }

  onSubmit() {
    if (this.form.valid) {
      const dados: DadosPagina3 = this.form.value;
      this.formularioService.salvarPagina3(dados);
      this.proxima.emit();
    } else {
      // Marcar campos obrigatórios como touched para mostrar erros
      Object.keys(this.form.controls).forEach(key => {
        const control = this.form.get(key);
        if (control && control.validator) {
          control.markAsTouched();
        }
      });
      
      // Mostrar alerta com campos obrigatórios
      const invalidFields: string[] = [];
      if (this.form.get('rgCpfCandidato')?.invalid) invalidFields.push('RG e CPF Candidato (PDF)');
      if (this.form.get('anexoI')?.invalid) invalidFields.push('Anexo I do edital');
      if (this.form.get('curriculoLattesCandidato')?.invalid) invalidFields.push('Link do Currículo Lattes do candidato');
      if (this.form.get('curriculoLattesOrientador')?.invalid) invalidFields.push('Link do Currículo Lattes do orientador');
      if (this.form.get('anexoII')?.invalid) invalidFields.push('Anexo II (ver edital)');
      if (this.form.get('comprovanteMatricula')?.invalid) invalidFields.push('Comprovante de Matrícula Assinado Instituição de Ensino');
      if (this.form.get('historicoEscolar')?.invalid) invalidFields.push('Histórico Escolar graduação');
      
      if (invalidFields.length > 0) {
        alert(`Por favor, preencha/envie os seguintes campos obrigatórios:\n\n${invalidFields.join('\n')}`);
      }
    }
  }

  onAnterior() {
    this.formularioService.salvarPagina3(this.form.value);
    this.anterior.emit();
  }
}
