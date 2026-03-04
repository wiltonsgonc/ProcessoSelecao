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

  constructor(private fb: FormBuilder, private formularioService: FormularioService) {
    this.form = this.fb.group({
      rgCpfCandidato: [null, Validators.required],
      anexoI: [null, Validators.required],
      curriculoLattesCandidato: [null, Validators.required],
      curriculoLattesOrientador: [null, Validators.required],
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
    }
  }

  onAnterior() {
    this.formularioService.salvarPagina3(this.form.value);
    this.anterior.emit();
  }
}
