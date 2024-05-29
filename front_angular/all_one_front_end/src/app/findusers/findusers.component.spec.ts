import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FindusersComponent } from './findusers.component';

describe('FindusersComponent', () => {
  let component: FindusersComponent;
  let fixture: ComponentFixture<FindusersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FindusersComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(FindusersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
