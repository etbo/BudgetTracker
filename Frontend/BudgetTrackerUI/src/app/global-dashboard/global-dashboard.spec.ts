import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GlobalDashboard } from './global-dashboard';

describe('GlobalDashboard', () => {
  let component: GlobalDashboard;
  let fixture: ComponentFixture<GlobalDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GlobalDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GlobalDashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
