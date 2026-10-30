import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { switchMap } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { mensagemErro } from '../../utils/mensagem-erro';

@Component({
  selector: 'app-cadastro',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './cadastro.component.html'
})
export class CadastroComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  nome = '';
  senha = '';
  erro = signal('');

  cadastrar() {
    this.auth
      .cadastrar(this.nome, this.senha)
      .pipe(switchMap(() => this.auth.entrar(this.nome, this.senha)))
      .subscribe({
        next: () => this.router.navigate(['/faixas']),
        error: (e: HttpErrorResponse) => this.erro.set(mensagemErro(e))
      });
  }
}
