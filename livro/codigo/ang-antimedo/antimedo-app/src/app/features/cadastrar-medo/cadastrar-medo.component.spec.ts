import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CadastrarMedoComponent } from './cadastrar-medo.component';
import { MedoService } from '../../core/services/medo.service';

describe('CadastrarMedoComponent (Formulário Reativo)', () => {
  let component: CadastrarMedoComponent;
  let fixture: ComponentFixture<CadastrarMedoComponent>;
  let medoService: MedoService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CadastrarMedoComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(CadastrarMedoComponent);
    component = fixture.componentInstance;
    medoService = TestBed.inject(MedoService);
    fixture.detectChanges();
  });

  it('deve inicializar o formulário com valores padrão', () => {
    expect(component.form.valid).toBeFalse();
    expect(component.form.get('categoria')?.value).toBe('TECNICO');
    expect(component.form.get('nivelParalisia')?.value).toBe(5);
  });

  it('deve validar que o título precisa de pelo menos 5 caracteres', () => {
    const tituloControl = component.form.get('titulo');
    tituloControl?.setValue('abc');
    expect(tituloControl?.valid).toBeFalse();

    tituloControl?.setValue('Medo Válido');
    expect(tituloControl?.valid).toBeTrue();
  });

  it('deve adicionar o medo ao serviço e resetar o formulário', () => {
    spyOn(medoService, 'adicionar');

    component.form.patchValue({
      titulo: 'Medo de falar com o cliente',
      categoria: 'PROFISSIONAL',
      nivelParalisia: 7,
      ehRuido: false
    });

    component.salvar();

    expect(medoService.adicionar).toHaveBeenCalledWith({
      titulo: 'Medo de falar com o cliente',
      categoria: 'PROFISSIONAL',
      nivelParalisia: 7,
      ehRuido: false,
      acoes: []
    });

    expect(component.form.get('titulo')?.value).toBe('');
  });
});

