import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationsEditor } from './operations-editor';

describe('OperationsEditor', () => {
  let component: OperationsEditor;
  let fixture: ComponentFixture<OperationsEditor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OperationsEditor]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OperationsEditor);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
