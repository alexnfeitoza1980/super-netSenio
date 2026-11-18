import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, signal } from '@angular/core';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { AmgDialogComponent } from './amg-dialog.component';
import { AmgDialogHarness } from './amg-dialog.harness';

@Component({
  standalone: true,
  imports: [AmgDialogComponent],
  template: `
    <amg-dialog [(isOpen)]="aberto" (closed)="aoFechar()">
      <h2>Confirmar Elogio</h2>
      <p>O elogio será enviado com carinho.</p>
    </amg-dialog>
  `
})
class DialogHostComponent {
  aberto = signal(true);
  fechadoNotificado = false;
  aoFechar() { this.fechadoNotificado = true; }
}

describe('AmgDialogComponent com Component Harness', () => {
  let fixture: ComponentFixture<DialogHostComponent>;
  let loader: any;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DialogHostComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(DialogHostComponent);
    fixture.detectChanges();
    loader = TestbedHarnessEnvironment.loader(fixture);
  });

  it('deve abrir, verificar o conteúdo e fechar ao clicar no botão X', async () => {
    const dialog = await loader.getHarness(AmgDialogHarness);

    expect(await dialog.isOpen()).toBe(true);
    expect(await dialog.getContentText()).toContain('Confirmar Elogio');

    await dialog.clickCloseButton();
    expect(await dialog.isOpen()).toBe(false);
    expect(fixture.componentInstance.fechadoNotificado).toBe(true);
  });
});

