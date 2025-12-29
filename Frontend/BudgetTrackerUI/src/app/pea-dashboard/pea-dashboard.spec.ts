import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PeaDashboard } from './pea-dashboard';

describe('PeaDashboard', () => {
  let component: PeaDashboard;
  let fixture: ComponentFixture<PeaDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PeaDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PeaDashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
