import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LanginComponent } from './langin.component';

describe('LanginComponent', () => {
  let component: LanginComponent;
  let fixture: ComponentFixture<LanginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LanginComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(LanginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
