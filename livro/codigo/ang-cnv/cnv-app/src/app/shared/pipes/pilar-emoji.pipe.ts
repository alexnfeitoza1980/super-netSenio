import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'pilarEmoji',
  standalone: true
})
export class PilarEmojiPipe implements PipeTransform {
  transform(pilar: string): string {
    switch (pilar.toLowerCase()) {
      case 'observação':
      case 'observacao':
        return '👁️';
      case 'sentimento':
        return '💓';
      case 'necessidade':
        return '🌱';
      case 'pedido':
        return '🤝';
      default:
        return '✨';
    }
  }
}

