import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DateFilter } from './date-filter';

describe('DateFilter', () => {
  let component: DateFilter;
  let fixture: ComponentFixture<DateFilter>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DateFilter]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DateFilter);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
