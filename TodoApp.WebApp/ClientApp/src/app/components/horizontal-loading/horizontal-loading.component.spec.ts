import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HorizontalLoadingComponent } from './horizontal-loading.component';

describe('HorizontalLoadingComponent', () => {
  let component: HorizontalLoadingComponent;
  let fixture: ComponentFixture<HorizontalLoadingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HorizontalLoadingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HorizontalLoadingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
