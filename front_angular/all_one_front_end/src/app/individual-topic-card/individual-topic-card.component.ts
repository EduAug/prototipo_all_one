import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-individual-topic-card',
  templateUrl: './individual-topic-card.component.html',
  styleUrl: './individual-topic-card.component.css'
})
export class IndividualTopicCardComponent {
  @Input() topicName = '';
  @Output() topicSelected = new EventEmitter<void>();

  select(): void{
    this.topicSelected.emit();
  }
}
