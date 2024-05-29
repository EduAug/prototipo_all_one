import { Component, Input, Output, EventEmitter } from '@angular/core';
import { GeocodingService } from '../geocoding.service';

@Component({
  selector: 'app-individual-user-card',
  templateUrl: './individual-user-card.component.html',
  styleUrl: './individual-user-card.component.css'
})
export class IndividualUserCardComponent {
  @Input() user:any;
  @Output() cardClicked: EventEmitter<number> = new EventEmitter<number>();
  city: string = '';

  constructor(private geocoding : GeocodingService){}
  ngOnInit(){
    this.getUserLocation();
  }

  getUserLocation(){
    const lat = this.user.latitude;
    const lon = this.user.longitude;
    this.geocoding.getLocal(lat,lon).subscribe(
      (returned) => {
        const cityC = returned.address.city || returned.address.town || returned.address.village;
        const cityS = returned.address.state;
        const cityCn = returned.address.country;
        this.city = `${cityC}, ${cityS}, ${cityCn}`;
        console.log(this.city);
      },
      (error) => {
        console.error("Error getting location:", error);
      }
    );
  }

  onCardClick() : void{
    this.cardClicked.emit(this.user.id);
  }
}
