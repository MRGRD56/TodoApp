import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import LoginRequest from "../../models/LoginRequest";

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

    constructor(http: HttpClient) {
    }

}
