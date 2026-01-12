import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CcMacroCategoriesMonthly } from './cc-macro-categories-monthly';

describe('CcMacroCategoriesMonthly', () => {
  let component: CcMacroCategoriesMonthly;
  let fixture: ComponentFixture<CcMacroCategoriesMonthly>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CcMacroCategoriesMonthly]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CcMacroCategoriesMonthly);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
