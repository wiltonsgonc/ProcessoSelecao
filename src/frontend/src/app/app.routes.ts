import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/processos', pathMatch: 'full' },
  { 
    path: 'processos', 
    loadComponent: () => import('./modules/processos/processo-list/processo-list.component').then(m => m.ProcessoListComponent)
  },
  { 
    path: 'candidatos', 
    loadComponent: () => import('./modules/candidatos/candidato-list/candidato-list.component').then(m => m.CandidatoListComponent)
  },
  { 
    path: 'avaliadores', 
    loadComponent: () => import('./modules/avaliadores/avaliador-list/avaliador-list.component').then(m => m.AvaliadorListComponent)
  },
  { 
    path: 'documentos', 
    loadComponent: () => import('./modules/documentos/documento-list/documento-list.component').then(m => m.DocumentoListComponent)
  },
  { 
    path: 'baremas', 
    loadComponent: () => import('./modules/baremas/barema-list/barema-list.component').then(m => m.BaremaListComponent)
  },
  { path: '**', redirectTo: '/processos' }
];
