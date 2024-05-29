import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LobbyfindComponent } from './lobbyfind.component';

describe('LobbyfindComponent', () => {
  let component: LobbyfindComponent;
  let fixture: ComponentFixture<LobbyfindComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LobbyfindComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(LobbyfindComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
