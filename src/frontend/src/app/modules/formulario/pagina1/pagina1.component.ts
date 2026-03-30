import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormularioService, DadosPagina1 } from '../../../core/services/formulario.service';

@Component({
  selector: 'app-pagina1',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './pagina1.component.html',
  styleUrls: ['./pagina1.component.css']
})
export class Pagina1Component implements OnInit {
  @Output() proxima = new EventEmitter<void>();

  form: FormGroup;

  constructor(
    private fb: FormBuilder, 
    private formularioService: FormularioService
  ) {
    this.form = this.fb.group({
      nome: [''],
      dataNascimento: ['', Validators.required],
      tipoDocumento: [''],
      numeroDocumento: [''],
      email: [''],
      telefone: ['', Validators.required],
      areaOfertada: [''],
      politicaPrivacidade: [false, Validators.requiredTrue]
    });
  }

  ngOnInit() {
    this.formularioService.dadosPagina1$.subscribe(dados => {
      if (dados && Object.keys(dados).length > 0) {
        this.form.patchValue(dados);
      }
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const dados: DadosPagina1 = this.form.value;
      this.formularioService.salvarPagina1(dados);
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
      if (this.form.get('dataNascimento')?.invalid) invalidFields.push('Data de nascimento');
      if (this.form.get('telefone')?.invalid) invalidFields.push('Telefone I');
      if (this.form.get('politicaPrivacidade')?.invalid) invalidFields.push('Política de Privacidade');
      
      if (invalidFields.length > 0) {
        alert(`Por favor, preencha os seguintes campos obrigatórios:\n\n${invalidFields.join('\n')}`);
      }
    }
  }
}
