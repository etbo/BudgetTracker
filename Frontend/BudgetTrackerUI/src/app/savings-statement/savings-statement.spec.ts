import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SavingsStatement } from './savings-statement';

describe('SavingsStatement', () => {
  let component: SavingsStatement;
  let fixture: ComponentFixture<SavingsStatement>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SavingsStatement]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SavingsStatement);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
