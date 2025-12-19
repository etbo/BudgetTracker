import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportFile } from './import-file';

describe('ImportFile', () => {
  let component: ImportFile;
  let fixture: ComponentFixture<ImportFile>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ImportFile]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ImportFile);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
