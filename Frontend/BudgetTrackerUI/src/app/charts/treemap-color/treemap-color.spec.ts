import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TreemapColor } from './treemap-color';

describe('TreemapColor', () => {
  let component: TreemapColor;
  let fixture: ComponentFixture<TreemapColor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TreemapColor]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TreemapColor);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
