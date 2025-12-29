import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CcRules } from './cc-rules';

describe('CcRules', () => {
  let component: CcRules;
  let fixture: ComponentFixture<CcRules>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CcRules]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CcRules);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
