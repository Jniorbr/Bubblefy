import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Faixa } from '../../models/faixa';
import { FiltroBusca } from '../../models/filtro-busca';
import { AuthService } from '../../services/auth.service';
import { FaixaService } from '../../services/faixa.service';
import { PlaylistService } from '../../services/playlist.service';
import { mensagemErro } from '../../utils/mensagem-erro';

@Component({
  selector: 'app-faixas',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './faixas.component.html'
})
export class FaixasComponent implements OnInit {
  auth = inject(AuthService);
  private faixaService = inject(FaixaService);
  private playlistService = inject(PlaylistService);
  private router = inject(Router);

  filtro: FiltroBusca = this.novoFiltro();
  nomePlaylist = '';

  generos = signal<string[]>([]);
  paises = signal<string[]>([]);
  faixas = signal<Faixa[]>([]);
  selecionadas = signal<number[]>([]);
  limite = signal(50);
  erro = signal('');
  exibidas = computed(() => this.faixas().slice(0, this.limite()));

  ngOnInit() {
    this.faixaService.generos().subscribe(generos => this.generos.set(generos));
    this.faixaService.paises().subscribe(paises => this.paises.set(paises));
    this.buscar();
  }

  buscar() {
    this.limite.set(50);
    this.faixaService.buscar(this.filtro).subscribe({
      next: faixas => {
        this.faixas.set(faixas);
        this.erro.set('');
      },
      error: (e: HttpErrorResponse) => this.erro.set(mensagemErro(e))
    });
  }

  limpar() {
    this.filtro = this.novoFiltro();
    this.buscar();
  }

  mostrarMais() {
    this.limite.update(limite => limite + 50);
  }

  alternar(id: number) {
    this.selecionadas.update(lista =>
      lista.includes(id) ? lista.filter(item => item !== id) : [...lista, id]
    );
  }

  criarPlaylist() {
    const nome = this.nomePlaylist.trim();
    if (!nome || this.selecionadas().length === 0) {
      return;
    }
    this.playlistService.criar(nome, this.selecionadas()).subscribe({
      next: playlist => this.router.navigate(['/playlists', playlist.id]),
      error: (e: HttpErrorResponse) => this.erro.set(mensagemErro(e))
    });
  }

  private novoFiltro(): FiltroBusca {
    return {
      genero: null,
      pais: null,
      ano: null,
      bpmMediaMinima: null,
      bpmMediaMaxima: null,
      valenciaMinima: null,
      valenciaMaxima: null
    };
  }
}
