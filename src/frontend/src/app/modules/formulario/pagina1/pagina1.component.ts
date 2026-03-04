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

  constructor(private fb: FormBuilder, private formularioService: FormularioService) {
    this.form = this.fb.group({
      nome: ['', Validators.required],
      dataNascimento: ['', Validators.required],
      tipoDocumento: ['', Validators.required],
      numeroDocumento: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      telefone: ['', Validators.required],
      areaOfertada: ['', Validators.required],
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
    }
  }
}
