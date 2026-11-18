import { Directive, HostListener, HostBinding, Input } from '@angular/core';

@Directive({
  selector: '[appBrilhoAntimedo]',
  standalone: true
})
export class BrilhoAntimedoDirective {
  @Input('appBrilhoAntimedo') corBrilho: string = '#dd1b44';

  @HostBinding('style.boxShadow') sombraCss: string = 'none';
  @HostBinding('style.transition') transicaoCss: string = 'all 0.25s ease';
  @HostBinding('style.cursor') cursorCss: string = 'pointer';

  @HostListener('mouseenter')
  aoEntrarComMouse(): void {
    this.sombraCss = `0 8px 24px ${this.corBrilho}55`;
  }

  @HostListener('mouseleave')
  aoSairComMouse(): void {
    this.sombraCss = 'none';
  }
}

