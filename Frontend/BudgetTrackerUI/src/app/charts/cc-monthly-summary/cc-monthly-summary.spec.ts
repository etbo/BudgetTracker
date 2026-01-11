import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CcMonthlySummary } from './cc-monthly-summary';

describe('CcMonthlySummary', () => {
  let component: CcMonthlySummary;
  let fixture: ComponentFixture<CcMonthlySummary>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CcMonthlySummary]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CcMonthlySummary);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
