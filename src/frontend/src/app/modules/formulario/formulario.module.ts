import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Pagina1Component } from './pagina1/pagina1.component';
import { Pagina2Component } from './pagina2/pagina2.component';
import { Pagina3Component } from './pagina3/pagina3.component';
import { Pagina4Component } from './pagina4/pagina4.component';

@NgModule({
  declarations: [
    Pagina1Component,
    Pagina2Component,
    Pagina3Component,
    Pagina4Component
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  exports: [
    Pagina1Component,
    Pagina2Component,
    Pagina3Component,
    Pagina4Component
  ]
})
export class FormularioModule { }