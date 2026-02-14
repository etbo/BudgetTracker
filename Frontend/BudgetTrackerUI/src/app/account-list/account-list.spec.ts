import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountList } from './account-list';

describe('AccountList', () => {
  let component: AccountList;
  let fixture: ComponentFixture<AccountList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
