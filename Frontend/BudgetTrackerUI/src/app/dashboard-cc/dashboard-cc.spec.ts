import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardCc } from './dashboard-cc';

describe('DashboardCc', () => {
  let component: DashboardCc;
  let fixture: ComponentFixture<DashboardCc>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardCc]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardCc);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
