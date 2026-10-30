import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { CadastroComponent } from './pages/cadastro/cadastro.component';
import { FaixasComponent } from './pages/faixas/faixas.component';
import { LoginComponent } from './pages/login/login.component';
import { PlaylistDetalheComponent } from './pages/playlist-detalhe/playlist-detalhe.component';
import { PlaylistsComponent } from './pages/playlists/playlists.component';

export const routes: Routes = [
  { path: '', redirectTo: 'faixas', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'cadastro', component: CadastroComponent },
  { path: 'faixas', component: FaixasComponent },
  { path: 'playlists', component: PlaylistsComponent, canActivate: [authGuard] },
  { path: 'playlists/:id', component: PlaylistDetalheComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: 'faixas' }
];
