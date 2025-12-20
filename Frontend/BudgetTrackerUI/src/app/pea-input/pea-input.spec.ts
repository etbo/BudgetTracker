import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PeaInput } from './pea-input';

describe('PeaInput', () => {
  let component: PeaInput;
  let fixture: ComponentFixture<PeaInput>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PeaInput]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PeaInput);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
