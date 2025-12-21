import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RulesList } from './rules-list';

describe('RulesList', () => {
  let component: RulesList;
  let fixture: ComponentFixture<RulesList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RulesList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RulesList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
