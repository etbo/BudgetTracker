import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LifeInsuranceInput } from './life-insurance-input';

describe('LifeInsuranceInput', () => {
  let component: LifeInsuranceInput;
  let fixture: ComponentFixture<LifeInsuranceInput>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LifeInsuranceInput]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LifeInsuranceInput);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
