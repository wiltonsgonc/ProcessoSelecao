import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormularioService } from '../../../core/services/formulario.service';
import { Pagina1Component } from '../pagina1/pagina1.component';
import { Pagina2Component } from '../pagina2/pagina2.component';
import { Pagina3Component } from '../pagina3/pagina3.component';
import { Pagina4Component } from '../pagina4/pagina4.component';

@Component({
  selector: 'app-formulario-inscricao',
  standalone: true,
  imports: [
    CommonModule,
    Pagina1Component,
    Pagina2Component,
    Pagina3Component,
    Pagina4Component
  ],
  templateUrl: './formulario-inscricao.component.html',
  styleUrls: ['./formulario-inscricao.component.css']
})
export class FormularioInscricaoComponent implements OnInit {
  paginaAtual = 1;
  totalPaginas = 4;
  mostrarModalConfirmacao = false;
  termoAceito = false;

  constructor(public formularioService: FormularioService) {}

  ngOnInit() {
    this.formularioService.paginaAtual$.subscribe(pagina => {
      this.paginaAtual = pagina;
    });
  }

  proximaPagina() {
    if (this.paginaAtual < this.totalPaginas) {
      this.formularioService.setPaginaAtual(this.paginaAtual + 1);
    }
  }

  paginaAnterior() {
    if (this.paginaAtual > 1) {
      this.formularioService.setPaginaAtual(this.paginaAtual - 1);
    }
  }

  abrirModalConfirmacao() {
    this.mostrarModalConfirmacao = true;
  }

  fecharModalConfirmacao() {
    this.mostrarModalConfirmacao = false;
  }

  confirmarInscricao() {
    if (this.termoAceito) {
      this.formularioService.enviarInscricaoCompleta().subscribe({
        next: (response) => {
          console.log('Inscrição enviada com sucesso:', response);
          this.fecharModalConfirmacao();
          alert('Inscrição realizada com sucesso!');
          this.formularioService.limparDados();
        },
        error: (error) => {
          console.error('Erro ao enviar inscrição:', error);
          alert('Erro ao enviar inscrição. Tente novamente.');
        }
      });
    }
  }

  getProgresso(): number {
    return (this.paginaAtual / this.totalPaginas) * 100;
  }
}
