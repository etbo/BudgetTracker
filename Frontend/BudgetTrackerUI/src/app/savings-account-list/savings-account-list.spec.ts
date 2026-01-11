import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SavingsAccountList } from './savings-account-list';

describe('SavingsAccountList', () => {
  let component: SavingsAccountList;
  let fixture: ComponentFixture<SavingsAccountList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SavingsAccountList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SavingsAccountList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
