import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IndividualTopicCardComponent } from './individual-topic-card.component';

describe('IndividualTopicCardComponent', () => {
  let component: IndividualTopicCardComponent;
  let fixture: ComponentFixture<IndividualTopicCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [IndividualTopicCardComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(IndividualTopicCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
