import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Playlist } from '../../models/playlist';
import { PlaylistService } from '../../services/playlist.service';
import { mensagemErro } from '../../utils/mensagem-erro';

@Component({
  selector: 'app-playlists',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './playlists.component.html'
})
export class PlaylistsComponent implements OnInit {
  private playlistService = inject(PlaylistService);

  playlists = signal<Playlist[]>([]);
  erro = signal('');

  ngOnInit() {
    this.playlistService.listar().subscribe({
      next: playlists => this.playlists.set(playlists),
      error: (e: HttpErrorResponse) => this.erro.set(mensagemErro(e))
    });
  }
}
