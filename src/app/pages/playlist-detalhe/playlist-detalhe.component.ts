import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Playlist } from '../../models/playlist';
import { PlaylistService } from '../../services/playlist.service';
import { mensagemErro } from '../../utils/mensagem-erro';

@Component({
  selector: 'app-playlist-detalhe',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './playlist-detalhe.component.html'
})
export class PlaylistDetalheComponent implements OnInit {
  private rota = inject(ActivatedRoute);
  private playlistService = inject(PlaylistService);

  playlist = signal<Playlist | null>(null);
  erro = signal('');

  ngOnInit() {
    const id = Number(this.rota.snapshot.paramMap.get('id'));
    this.playlistService.obterPorId(id).subscribe({
      next: playlist => this.playlist.set(playlist),
      error: (e: HttpErrorResponse) =>
        this.erro.set(e.status === 404 ? 'Playlist não encontrada.' : mensagemErro(e))
    });
  }
}
