import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { API } from '../api';
import { Playlist } from '../models/playlist';

@Injectable({ providedIn: 'root' })
export class PlaylistService {
  private http = inject(HttpClient);

  listar() {
    return this.http.get<Playlist[]>(`${API}/playlists`);
  }

  obterPorId(id: number) {
    return this.http.get<Playlist>(`${API}/playlists/${id}`);
  }

  criar(nome: string, faixaIds: number[]) {
    return this.http.post<Playlist>(`${API}/playlists`, { nome, faixaIds });
  }
}
