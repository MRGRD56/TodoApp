import {Component, OnInit} from '@angular/core';
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import LoginRequest from "../../models/LoginRequest";
import {AuthService} from "../../services/auth/auth.service";
import {Router} from "@angular/router";
import {NgModel} from "@angular/forms";
import {BrowserError} from "protractor/built/exitCodes";

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
        this.passwordRepeatError = null;
        this.validationError = null;

        if (this.registrationData.password !== this.passwordRepeat) {
            this.passwordRepeatError = "The passwords must match";
            this.canSubmit = false;
        }
        if (!/^(?=.*[A-Za-z0-9]$)[A-Za-z][A-Za-z\d.-_]{2,19}$/.test(this.registrationData.login)) {
            this.loginError = "Invalid login. The login must be at least 3 characters long.";
            this.canSubmit = false;
        }
        if (!/^(?=.*[A-Za-zА-Яа-яЁё])(?=.*\d).{6,}$/.test(this.registrationData.password)) {
            this.passwordError = "The password must be at least 6 characters long and contain at least one number and one letter";
            this.canSubmit = false;
        }

        this.showErrors = true;
        if (!this.canSubmit) return;
        try {
            this.isRegistering = true;
            await this.auth.register(this.registrationData);
            await this.router.navigate([""]);
        } catch (error) {
            if (error instanceof HttpErrorResponse) {
                const httpError = error as HttpErrorResponse;
                console.log(httpError);
                if (httpError.error === "The user with the specified login already exists.") {
                    this.loginError = "The login is already registered";
                } else if (httpError.error?.errors) {
                    if (httpError.error.errors.Login) {
                        const loginErrors = <string[]>httpError.error.errors.Login;
                        this.loginError = loginErrors.join("\n");
                    }
                    if (httpError.error.errors.Password) {
                        const passwordErrors = <string[]>httpError.error.errors.Password;
                        this.passwordError = passwordErrors.join("\n");
                    }
                } else {
                    this.validationError = "An error occured";
                    console.log(httpError);
                }
            }
        } finally {
            this.isRegistering = false;
        }
    }

    public onInput(e: Event, loginModel: NgModel, passwordModel: NgModel, passwordRepeatModel: NgModel) {
        this.canSubmit = loginModel.valid && passwordModel.valid && passwordRepeatModel.valid;
        this.showErrors = false;
    }
}
