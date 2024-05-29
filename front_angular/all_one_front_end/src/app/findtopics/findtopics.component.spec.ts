import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FindtopicsComponent } from './findtopics.component';

describe('FindtopicsComponent', () => {
  let component: FindtopicsComponent;
  let fixture: ComponentFixture<FindtopicsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FindtopicsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(FindtopicsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
