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

  constructor(private fb: FormBuilder, private formularioService: FormularioService) {
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
      sexo: [''],
      cpf: [''],
      telefone1: ['', Validators.required],
      telefone2: [''],
      corRaca: [''],
      autorizacaoDados: [''],
      tipoVisto: [''],
      numeroRegistroGeral: [''],
      dataVencimentoRG: ['']
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
    }
  }

  onAnterior() {
    this.formularioService.salvarPagina2(this.form.value);
    this.anterior.emit();
  }
}
