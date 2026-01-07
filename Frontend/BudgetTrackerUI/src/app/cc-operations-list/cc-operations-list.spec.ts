import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CcOperationsList } from './cc-operations-list';

describe('CcOperationsList', () => {
  let component: CcOperationsList;
  let fixture: ComponentFixture<CcOperationsList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CcOperationsList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CcOperationsList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
