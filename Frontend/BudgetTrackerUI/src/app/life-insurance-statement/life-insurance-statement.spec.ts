import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LifeInsuranceStatement } from './life-insurance-statement';

describe('LifeInsuranceStatement', () => {
  let component: LifeInsuranceStatement;
  let fixture: ComponentFixture<LifeInsuranceStatement>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LifeInsuranceStatement]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LifeInsuranceStatement);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
