import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class GeocodingService {

  constructor(private http : HttpClient) { }

  getLocal(lat:number, lon:number): Observable<any>{
    const apiUrlOSMNominatim = `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lon}`;
    return this.http.get(apiUrlOSMNominatim);
  }
}
