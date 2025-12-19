import { TestBed } from '@angular/core/testing';

import { BudgettrackerService } from './budgettracker';

describe('Budgettracker', () => {
  let service: BudgettrackerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BudgettrackerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
