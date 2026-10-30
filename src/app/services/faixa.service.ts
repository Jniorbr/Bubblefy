import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { API } from '../api';
import { Faixa } from '../models/faixa';
import { FiltroBusca } from '../models/filtro-busca';

@Injectable({ providedIn: 'root' })
export class FaixaService {
  private http = inject(HttpClient);

  buscar(filtro: FiltroBusca) {
    let params = new HttpParams();
    for (const [chave, valor] of Object.entries(filtro)) {
      if (valor !== null && valor !== undefined && valor !== '') {
        params = params.set(chave, String(valor));
      }
    }
    return this.http.get<Faixa[]>(`${API}/tracks/search`, { params });
  }

  generos() {
    return this.http.get<string[]>(`${API}/tracks/genres`);
  }

  paises() {
    return this.http.get<string[]>(`${API}/tracks/countries`);
  }
}
