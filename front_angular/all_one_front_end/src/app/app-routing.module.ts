import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LanginComponent } from './langin/langin.component';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './auth.guard';
import { CreateAccountComponent } from './create-account/create-account.component';
import { FindtopicsComponent } from './findtopics/findtopics.component';
import { LobbyfindComponent } from './lobbyfind/lobbyfind.component';
import { FindusersComponent } from './findusers/findusers.component';
import { ProfileComponent } from './profile/profile.component';
import { VotingComponent } from './voting/voting.component';


const routes: Routes = [
  {path: '', pathMatch:'full', redirectTo:'signin'},
  {path: 'signin',pathMatch:'full',component: LanginComponent},
  {path: 'home', pathMatch:'full', component: HomeComponent, canActivate: [AuthGuard]},
  {path: 'signup', pathMatch: 'full', component: CreateAccountComponent},
  {path: 'findlobby', pathMatch: 'full', component: LobbyfindComponent},
  {path: 'topics', pathMatch: 'full', component: FindtopicsComponent},
  {path: 'users', pathMatch: 'full', component: FindusersComponent},
  {path: 'profile', pathMatch: 'full', component: ProfileComponent, canActivate: [AuthGuard]},
  {path: 'voting', pathMatch: 'full', component: VotingComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
