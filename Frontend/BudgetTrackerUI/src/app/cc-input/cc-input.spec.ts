import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CcInput } from './cc-input';

describe('CcInput', () => {
  let component: CcInput;
  let fixture: ComponentFixture<CcInput>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CcInput]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CcInput);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
