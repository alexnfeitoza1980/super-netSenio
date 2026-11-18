import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component } from '@angular/core';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { AmgButtonComponent } from './amg-button.component';
import { AmgButtonHarness } from './amg-button.harness';

@Component({
  standalone: true,
  imports: [AmgButtonComponent],
  template: `
    <amg-button variant="primary" (click)="aoClicar()">
      Elogiar Colega
    </amg-button>
    <amg-button variant="ghost" [disabled]="true">
      Botão Desabilitado
    </amg-button>
  `
})
class TestHostComponent {
  clicado = false;
  aoClicar() { this.clicado = true; }
}

describe('AmgButtonComponent com Component Harness', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let loader: any;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestHostComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(TestHostComponent);
    fixture.detectChanges();
    loader = TestbedHarnessEnvironment.loader(fixture);
  });

  it('deve inspecionar e interagir com o botão sem tocar em classes CSS internas', async () => {
    const botoes = await loader.getAllHarnesses(AmgButtonHarness);
    expect(botoes.length).toBe(2);

    expect(await botoes[0].getText()).toBe('Elogiar Colega');
    expect(await botoes[0].isDisabled()).toBe(false);

    await botoes[0].click();
    expect(fixture.componentInstance.clicado).toBe(true);

    expect(await botoes[1].getText()).toBe('Botão Desabilitado');
    expect(await botoes[1].isDisabled()).toBe(true);
  });
});

