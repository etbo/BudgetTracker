import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PeaWallet } from './pea-wallet';

describe('PeaWallet', () => {
  let component: PeaWallet;
  let fixture: ComponentFixture<PeaWallet>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PeaWallet]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PeaWallet);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
