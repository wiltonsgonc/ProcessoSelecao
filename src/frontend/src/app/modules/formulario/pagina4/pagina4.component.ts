import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormularioService, DadosPagina4 } from '../../../core/services/formulario.service';

@Component({
  selector: 'app-pagina4',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './pagina4.component.html',
  styleUrls: ['./pagina4.component.css']
})
export class Pagina4Component implements OnInit {
  @Output() anterior = new EventEmitter<void>();
  @Output() abrirConfirmacao = new EventEmitter<void>();

  form: FormGroup;

  constructor(
    private fb: FormBuilder, 
    private formularioService: FormularioService
  ) {
    const hoje = new Date();
    const dataFormatada = hoje.toLocaleDateString('en-CA');
    this.form = this.fb.group({
      processoSeletivo: [''],
      areaOfertada: ['', Validators.required],
      formaInscricao: ['', Validators.required],
      localProva: ['', Validators.required],
      campusProva: ['', Validators.required],
      dataInscricao: [dataFormatada],
      valorInscricao: [''],
      deficienciaFisica: [false],
      deficienciaAuditiva: [false],
      deficienciaFala: [false],
      deficienciaVisual: [false],
      deficienciaMental: [false],
      deficienciaIntelectual: [false],
      deficienciaReabilitado: [false],
      deficienciaMultipla: [false],
      motivoOutrasNecessidades: ['']
    });
  }

  ngOnInit() {
    this.formularioService.dadosPagina4$.subscribe(dados => {
      if (dados && Object.keys(dados).length > 0) {
        this.form.patchValue(dados);
      }
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const dados: DadosPagina4 = this.form.value;
      this.formularioService.salvarPagina4(dados);
      this.abrirConfirmacao.emit();
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
      if (this.form.get('areaOfertada')?.invalid) invalidFields.push('Área ofertada - 1ª opção de curso');
      if (this.form.get('formaInscricao')?.invalid) invalidFields.push('Forma de inscrição');
      if (this.form.get('localProva')?.invalid) invalidFields.push('Local de realização da prova');
      if (this.form.get('campusProva')?.invalid) invalidFields.push('Campus de realização da prova');
      
      if (invalidFields.length > 0) {
        alert(`Por favor, preencha os seguintes campos obrigatórios:\n\n${invalidFields.join('\n')}`);
      }
    }
  }

  onAnterior() {
    this.formularioService.salvarPagina4(this.form.value);
    this.anterior.emit();
  }
}
