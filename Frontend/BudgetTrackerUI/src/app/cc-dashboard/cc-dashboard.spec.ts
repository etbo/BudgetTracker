import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CcDashboard } from './cc-dashboard';

describe('CcDashboard', () => {
  let component: CcDashboard;
  let fixture: ComponentFixture<CcDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CcDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CcDashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
