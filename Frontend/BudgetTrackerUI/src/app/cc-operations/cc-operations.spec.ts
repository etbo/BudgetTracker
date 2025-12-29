import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CcOperations } from './cc-operations';

describe('CcOperations', () => {
  let component: CcOperations;
  let fixture: ComponentFixture<CcOperations>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CcOperations]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CcOperations);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
