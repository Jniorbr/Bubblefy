import { Faixa } from './faixa';

export interface Playlist {
  id: number;
  nome: string | null;
  faixas: Faixa[];
}
