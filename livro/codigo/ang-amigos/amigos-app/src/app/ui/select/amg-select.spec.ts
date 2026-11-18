import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { AmgSelectComponent, AmgOption } from './amg-select.component';
import { AmgSelectHarness } from './amg-select.harness';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, AmgSelectComponent],
  template: `
    <amg-select 
      [options]="opcoes" 
      [formControl]="controle"
      placeholder="Escolha um mentor" />
  `
})
class SelectHostComponent {
  controle = new FormControl<string>('dale');
  opcoes: AmgOption<string>[] = [
    { label: 'Dale Carnegie', value: 'dale', badge: 'Autor' },
    { label: 'Frank Gammond', value: 'frank', badge: 'Shakespeare' },
    { label: 'Abraham Lincoln', value: 'lincoln', badge: 'Presidente' }
  ];
}

describe('AmgSelectComponent com CVA e Component Harness', () => {
  let fixture: ComponentFixture<SelectHostComponent>;
  let loader: any;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SelectHostComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(SelectHostComponent);
    fixture.detectChanges();
    loader = TestbedHarnessEnvironment.loader(fixture);
  });

  it('deve inicializar com o valor do FormControl e refletir na interface', async () => {
    const select = await loader.getHarness(AmgSelectHarness);
    expect(await select.getSelectedText()).toContain('Dale Carnegie');
  });

  it('deve abrir o dropdown e selecionar uma nova opção atualizando o FormControl', async () => {
    const select = await loader.getHarness(AmgSelectHarness);
    
    expect(await select.getOptionsCount()).toBe(3);

    // Seleciona o terceiro item ('lincoln')
    await select.selectOptionByIndex(2);

    expect(await select.getSelectedText()).toContain('Abraham Lincoln');
    expect(fixture.componentInstance.controle.value).toBe('lincoln');
  });
});

