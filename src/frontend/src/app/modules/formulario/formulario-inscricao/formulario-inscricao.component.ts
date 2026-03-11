import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormularioService } from '../../../core/services/formulario.service';
import { ProcessoSelecaoService } from '../../../core/services/processo-selecao.service';
import { Pagina1Component } from '../pagina1/pagina1.component';
import { Pagina2Component } from '../pagina2/pagina2.component';
import { Pagina3Component } from '../pagina3/pagina3.component';
import { Pagina4Component } from '../pagina4/pagina4.component';
import { PaginaNaoEncontradaComponent } from '../../../shared/pagina-nao-encontrada/pagina-nao-encontrada.component';

@Component({
  selector: 'app-formulario-inscricao',
  standalone: true,
  imports: [
    CommonModule,
    Pagina1Component,
    Pagina2Component,
    Pagina3Component,
    Pagina4Component,
    PaginaNaoEncontradaComponent
  ],
  templateUrl: './formulario-inscricao.component.html',
  styleUrls: ['./formulario-inscricao.component.css']
})
export class FormularioInscricaoComponent implements OnInit {
  paginaAtual = 1;
  totalPaginas = 4;
  mostrarModalConfirmacao = false;
  termoAceito = false;
  processoEncontrado = true;
  carregando = true;

  constructor(
    public formularioService: FormularioService,
    private route: ActivatedRoute,
    private router: Router,
    private processoSelecaoService: ProcessoSelecaoService
  ) {}

  ngOnInit() {
    const processoId = this.route.snapshot.paramMap.get('processoId');
    
    if (processoId) {
      this.processoSelecaoService.getById(+processoId).subscribe({
        next: (processo) => {
          this.carregando = false;
          this.processoEncontrado = true;
          this.formularioService.salvarPagina4({ processoSeletivo: processo.nome });
        },
        error: () => {
          this.carregando = false;
          this.processoEncontrado = false;
        }
      });
    } else {
      this.carregando = false;
      this.processoEncontrado = false;
    }

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
