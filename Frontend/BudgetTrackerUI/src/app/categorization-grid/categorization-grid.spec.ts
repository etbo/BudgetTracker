import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategorizationGrid } from './categorization-grid';

describe('CategorizationGrid', () => {
  let component: CategorizationGrid;
  let fixture: ComponentFixture<CategorizationGrid>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategorizationGrid]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CategorizationGrid);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
