import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss']
})
export class RegisterComponent {

    public registrationData = {
        login: "",
        password: ""
    };

    public passwordRepeat = "";

    constructor(http: HttpClient) {
    }

}
