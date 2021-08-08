import {NgModule} from "@angular/core";
import {BrowserModule} from '@angular/platform-browser';
import {FormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';

import {AppComponent} from './components/app.component';
import {NavMenuComponent} from './components/nav-menu/nav-menu.component';
import {HomeComponent} from './components/home/home.component';
import {HorizontalLoadingComponent} from "./components/horizontal-loading/horizontal-loading.component";
import {IconLoadingComponent} from "./components/icon-loading/icon-loading.component";
import {LoginComponent} from "./components/login/login.component";
import {RegisterComponent} from "./components/register/register.component";
import {JwtModule} from "@auth0/angular-jwt";
import {environment} from "../environments/environment";
import {ACCESS_TOKEN_KEY} from "./services/auth/auth.service";
import {AuthorizedGuard} from "./guards/authorized/authorized.guard";
import {NotAuthorizedGuard} from "./guards/not-authorized/not-authorized.guard";

export function getToken() {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
}

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
            {path: '', component: HomeComponent, pathMatch: 'full', canActivate: [AuthorizedGuard]},
            {path: 'login', component: LoginComponent, canActivate: [NotAuthorizedGuard]},
            {path: 'register', component: RegisterComponent, canActivate: [NotAuthorizedGuard]}
        ]),
        JwtModule.forRoot({
            config: {
                tokenGetter: getToken,
                allowedDomains: environment.tokenAllowedDomains
            }
        })
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule {
}
