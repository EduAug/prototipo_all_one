import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IndividualUserCardComponent } from './individual-user-card.component';

describe('IndividualUserCardComponent', () => {
  let component: IndividualUserCardComponent;
  let fixture: ComponentFixture<IndividualUserCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [IndividualUserCardComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(IndividualUserCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
