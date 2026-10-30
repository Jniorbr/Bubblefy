import { HttpErrorResponse } from '@angular/common/http';

export function mensagemErro(erro: HttpErrorResponse): string {
  if (erro.status === 0) {
    return 'Não foi possível conectar à API.';
  }
  if (erro.error?.detail) {
    return erro.error.detail;
  }
  return 'Ocorreu um erro inesperado.';
}
