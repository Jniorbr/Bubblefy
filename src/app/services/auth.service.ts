import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { API } from '../api';
import { TokenResposta } from '../models/token-resposta';
import { Usuario } from '../models/usuario';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private chave = 'bubblefy_token';

  token = signal<string | null>(localStorage.getItem(this.chave));
  usuario = signal<Usuario | null>(null);
  logado = computed(() => this.token() !== null);

  cadastrar(nome: string, senha: string) {
    return this.http.post<Usuario>(`${API}/auth/register`, { nome, senha });
  }

  entrar(nome: string, senha: string) {
    return this.http.post<TokenResposta>(`${API}/auth/login`, { nome, senha }).pipe(
      tap(resposta => {
        localStorage.setItem(this.chave, resposta.accessToken);
        this.token.set(resposta.accessToken);
        this.carregarUsuario();
      })
    );
  }

  carregarUsuario() {
    this.http.get<Usuario>(`${API}/users/me`).subscribe({
      next: usuario => this.usuario.set(usuario),
      error: () => this.sair()
    });
  }

  sair() {
    localStorage.removeItem(this.chave);
    this.token.set(null);
    this.usuario.set(null);
    this.router.navigate(['/login']);
  }
}
