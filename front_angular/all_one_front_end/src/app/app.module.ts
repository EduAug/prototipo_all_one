import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LanginComponent } from './langin/langin.component';
import { AuthInterceptor } from './auth.interceptor';
import { HomeComponent } from './home/home.component';
import { CreateAccountComponent } from './create-account/create-account.component';
import { FooterComponent } from './footer/footer.component';
import { LobbyfindComponent } from './lobbyfind/lobbyfind.component';
import { FindtopicsComponent } from './findtopics/findtopics.component';
import { IndividualTopicCardComponent } from './individual-topic-card/individual-topic-card.component';

import { MatSnackBarModule } from '@angular/material/snack-bar';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FindusersComponent } from './findusers/findusers.component';
import { IndividualUserCardComponent } from './individual-user-card/individual-user-card.component';
import { ProfileComponent } from './profile/profile.component';
import { DatePipe } from '@angular/common';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { VotingComponent } from './voting/voting.component';

@NgModule({
  declarations: [
    AppComponent,
    LanginComponent,
    HomeComponent,
    CreateAccountComponent,
    FooterComponent,
    LobbyfindComponent,
    FindtopicsComponent,
    IndividualTopicCardComponent,
    FindusersComponent,
    IndividualUserCardComponent,
    ProfileComponent,
    VotingComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    MatSnackBarModule,
    BrowserAnimationsModule
  ],
  providers: [
    provideClientHydration(),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    DatePipe,
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
