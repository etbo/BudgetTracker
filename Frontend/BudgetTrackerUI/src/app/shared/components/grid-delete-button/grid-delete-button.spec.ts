import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GridDeleteButton } from './grid-delete-button';

describe('GridDeleteButton', () => {
  let component: GridDeleteButton;
  let fixture: ComponentFixture<GridDeleteButton>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GridDeleteButton]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GridDeleteButton);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
