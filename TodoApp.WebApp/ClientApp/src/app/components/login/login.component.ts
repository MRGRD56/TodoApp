import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import LoginRequest from "../../models/LoginRequest";
import {AuthService} from "../../services/auth/auth.service";
import {Router} from "@angular/router";

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent {

    public loginData: LoginRequest = {
        login: "",
        password: ""
    };

    constructor(private readonly auth: AuthService,
                private readonly router: Router) {

    }

    public async login() {
        try {
            await this.auth.login(this.loginData);
            await this.router.navigate([""]);
        } catch (error) {
            alert(error);
        }
    }
}
