import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TradutorCnvComponent } from './tradutor-cnv.component';

describe('TradutorCnvComponent', () => {
  let component: TradutorCnvComponent;
  let fixture: ComponentFixture<TradutorCnvComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TradutorCnvComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(TradutorCnvComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('deve ser criado com sucesso', () => {
    expect(component).toBeTruthy();
  });

  it('deve iniciar com o formulário inválido quando vazio', () => {
    expect(component.inputControl.valid).toBeFalse();
  });

  it('deve validar formulário ao preencher exemplo', () => {
    component.carregarExemplo();
    expect(component.inputControl.valid).toBeTrue();
  });
});

