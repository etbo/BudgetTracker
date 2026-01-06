import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CcEvolutionChart } from './cc-evolution-chart';

describe('CcEvolutionChart', () => {
  let component: CcEvolutionChart;
  let fixture: ComponentFixture<CcEvolutionChart>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CcEvolutionChart]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CcEvolutionChart);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
