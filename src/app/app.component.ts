import { Component, OnInit, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  auth = inject(AuthService);

  ngOnInit() {
    if (this.auth.logado()) {
      this.auth.carregarUsuario();
    }
  }
}
