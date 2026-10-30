import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const tokenInterceptor: HttpInterceptorFn = (requisicao, proximo) => {
  const auth = inject(AuthService);
  const token = auth.token();
  const comToken = token
    ? requisicao.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : requisicao;

  return proximo(comToken).pipe(
    catchError((erro: HttpErrorResponse) => {
      if (erro.status === 401 && token) {
        auth.sair();
      }
      return throwError(() => erro);
    })
  );
};
