import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LifeInsurranceList } from './life-insurrance-list';

describe('LifeInsurranceList', () => {
  let component: LifeInsurranceList;
  let fixture: ComponentFixture<LifeInsurranceList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LifeInsurranceList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LifeInsurranceList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
