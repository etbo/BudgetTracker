import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountsStatus } from './accounts-status';

describe('AccountsStatus', () => {
  let component: AccountsStatus;
  let fixture: ComponentFixture<AccountsStatus>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountsStatus]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountsStatus);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
