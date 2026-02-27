import { Routes } from '@angular/router';
import { PublicLayoutComponent } from './shared/public-layout/public-layout.component';
import { AdminLayoutComponent } from './shared/admin-layout/admin-layout.component';

export const routes: Routes = [
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      { path: '', redirectTo: 'editais', pathMatch: 'full' },
      { 
        path: 'editais', 
        loadComponent: () => import('./modules/editais/edital-list/edital-list.component').then(m => m.EditalListComponent)
      },
      { 
        path: 'inscricoes/novo/:editalId', 
        loadComponent: () => import('./modules/inscricao/inscricao-form/inscricao-form.component').then(m => m.InscricaoFormComponent)
      },
      { 
        path: 'inscricoes/:id', 
        loadComponent: () => import('./modules/inscricao/inscricao-detail/inscricao-detail.component').then(m => m.InscricaoDetailComponent)
      }
    ]
  },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    children: [
      { path: '', redirectTo: 'processos', pathMatch: 'full' },
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
      { 
        path: 'editais', 
        loadComponent: () => import('./modules/editais/edital-list-admin/edital-list-admin.component').then(m => m.EditalListAdminComponent)
      },
      { 
        path: 'editais/novo', 
        loadComponent: () => import('./modules/editais/edital-form/edital-form.component').then(m => m.EditalFormComponent)
      },
      { 
        path: 'editais/:id', 
        loadComponent: () => import('./modules/editais/edital-form/edital-form.component').then(m => m.EditalFormComponent)
      }
    ]
  },
  { path: '**', redirectTo: '' }
];
