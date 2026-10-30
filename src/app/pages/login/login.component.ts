import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { mensagemErro } from '../../utils/mensagem-erro';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  nome = '';
  senha = '';
  erro = signal('');

  entrar() {
    this.auth.entrar(this.nome, this.senha).subscribe({
      next: () => this.router.navigate(['/faixas']),
      error: (e: HttpErrorResponse) =>
        this.erro.set(e.status === 401 ? 'Nome ou senha inválidos.' : mensagemErro(e))
    });
  }
}
