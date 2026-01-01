import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PeaOperations } from './pea-operations';

describe('PeaOperations', () => {
  let component: PeaOperations;
  let fixture: ComponentFixture<PeaOperations>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PeaOperations]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PeaOperations);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
