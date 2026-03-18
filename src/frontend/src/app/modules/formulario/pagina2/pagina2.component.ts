import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormularioService, DadosPagina2 } from '../../../core/services/formulario.service';

@Component({
  selector: 'app-pagina2',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './pagina2.component.html',
  styleUrls: ['./pagina2.component.css']
})
export class Pagina2Component implements OnInit {
  @Output() proxima = new EventEmitter<void>();
  @Output() anterior = new EventEmitter<void>();

  form: FormGroup;

  constructor(
    private fb: FormBuilder, 
    private formularioService: FormularioService
  ) {
    this.form = this.fb.group({
      nome: ['', Validators.required],
      dataNascimento: ['', Validators.required],
      paisNatal: [''],
      estadoNatal: [''],
      naturalidade: [''],
      nomeSocial: [''],
      estadoCivil: [''],
      nacionalidade: [''],
      email: ['', [Validators.required, Validators.email]],
      sexo: ['', Validators.required],
      cpf: [''],
      telefone1: ['', Validators.required],
      telefone2: [''],
      corRaca: [''],
      autorizacaoDados: ['', Validators.required],
      tipoVisto: [''],
      numeroRegistroGeral: [''],
      dataVencimentoRG: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.formularioService.dadosPagina2$.subscribe(dados => {
      if (dados && Object.keys(dados).length > 0) {
        this.form.patchValue(dados);
      }
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const dados: DadosPagina2 = this.form.value;
      this.formularioService.salvarPagina2(dados);
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
      if (this.form.get('nome')?.invalid) invalidFields.push('Nome');
      if (this.form.get('dataNascimento')?.invalid) invalidFields.push('Data de nascimento');
      if (this.form.get('email')?.invalid) invalidFields.push('E-mail');
      if (this.form.get('sexo')?.invalid) invalidFields.push('Sexo');
      if (this.form.get('telefone1')?.invalid) invalidFields.push('Telefone I');
      if (this.form.get('autorizacaoDados')?.invalid) invalidFields.push('Autorizo a utilização de dados pessoais');
      if (this.form.get('dataVencimentoRG')?.invalid) invalidFields.push('Data vencimento RG');
      
      if (invalidFields.length > 0) {
        alert(`Por favor, preencha os seguintes campos obrigatórios:\n\n${invalidFields.join('\n')}`);
      }
    }
  }

  onAnterior() {
    this.formularioService.salvarPagina2(this.form.value);
    this.anterior.emit();
  }
}
