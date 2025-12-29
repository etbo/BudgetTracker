import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CcCategories } from './cc-categories';

describe('CcCategories', () => {
  let component: CcCategories;
  let fixture: ComponentFixture<CcCategories>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CcCategories]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CcCategories);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
