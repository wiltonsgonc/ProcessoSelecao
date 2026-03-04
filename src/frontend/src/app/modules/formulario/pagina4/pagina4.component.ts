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

  constructor(private fb: FormBuilder, private formularioService: FormularioService) {
    this.form = this.fb.group({
      processoSeletivo: [''],
      areaOfertada: ['', Validators.required],
      formaInscricao: ['', Validators.required],
      localProva: ['', Validators.required],
      campusProva: ['', Validators.required],
      dataInscricao: [''],
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
    }
  }

  onAnterior() {
    this.formularioService.salvarPagina4(this.form.value);
    this.anterior.emit();
  }
}
