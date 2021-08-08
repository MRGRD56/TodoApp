import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { HorizontalLoadingComponent } from "./horizontal-loading/horizontal-loading.component";
import { IconLoadingComponent } from "./icon-loading/icon-loading.component";
import {LoginComponent} from "./login/login.component";
import {RegisterComponent} from "./register/register.component";

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        HorizontalLoadingComponent,
        IconLoadingComponent,
        LoginComponent,
        RegisterComponent
    ],
    imports: [
        BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
        HttpClientModule,
        FormsModule,
        RouterModule.forRoot([
            {path: '', component: HomeComponent, pathMatch: 'full'},
            {path: 'login', component: LoginComponent},
            {path: 'register', component: RegisterComponent}
        ])
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule {
}
