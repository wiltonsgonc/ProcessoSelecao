import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LayoutService {
  mostrarCabecalhoRodape = signal<boolean>(true);

  mostrarElementosPublicos(mostrar: boolean) {
    this.mostrarCabecalhoRodape.set(mostrar);
  }
}
