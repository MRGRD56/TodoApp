import {Component, OnInit} from '@angular/core';
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import LoginRequest from "../../models/LoginRequest";
import {AuthService} from "../../services/auth/auth.service";
import {Router} from "@angular/router";
import {NgModel} from "@angular/forms";

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss']
})
export class RegisterComponent {

    public registrationData: LoginRequest = {
        login: "",
        password: ""
    };
    public passwordRepeat = "";

    public isRegistering: boolean = false;

    public loginError: string;
    public passwordError: string;
    public passwordRepeatError: string;
    public validationError: string; //other errors

    public showErrors: boolean = false;
    private canSubmit: boolean = false;

    constructor(private readonly auth: AuthService,
                private readonly router: Router) {

    }

    public async register() {
        this.loginError = null;
        this.passwordError = null;
        this.validationError = null;

        this.showErrors = true;
        if (!this.canSubmit) return;
        try {
            this.isRegistering = true;
            await this.auth.login(this.registrationData);
            await this.router.navigate([""]);
        } catch (error) {
            if (error instanceof HttpErrorResponse) {
                const httpError = error as HttpErrorResponse;
                switch (httpError.error) {
                    case "The user with the specified login does not exist":
                        this.loginError = "The login is not registered";
                        break;
                    case "Invalid password":
                        this.passwordError = "Invalid password";
                        break;
                    default:
                        this.validationError = "An error occured";
                        console.log(error);
                        break;
                }
            }
        } finally {
            this.isRegistering = false;
        }
    }

    public onInput(e: Event, loginModel: NgModel, passwordModel: NgModel, passwordRepeatModel: NgModel) {
        this.canSubmit = loginModel.valid && passwordModel.valid;
        this.showErrors = false;
    }
}
