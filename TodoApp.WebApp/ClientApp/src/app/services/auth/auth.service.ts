import {Inject, Injectable} from '@angular/core';
import {Observable} from "rxjs";
import LoginResponse from "../../models/LoginResponse";
import {HttpClient} from "@angular/common/http";
import {JwtHelperService} from "@auth0/angular-jwt";
import {Router} from "@angular/router";
import LoginRequest from "../../models/LoginRequest";
import {tap} from "rxjs/operators";
import CurrentUser from "../../models/CurrentUser";

export const ACCESS_TOKEN_KEY = "api_access_token";

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    constructor(
        private readonly http: HttpClient,
        @Inject("BASE_URL") private readonly baseUrl: string,
        private readonly jwtHelper: JwtHelperService,
        private readonly router: Router
    ) { }

    private static get accessToken() {
        return localStorage.getItem(ACCESS_TOKEN_KEY)
    }
    private static set accessToken(value: string) {
        localStorage.setItem(ACCESS_TOKEN_KEY, value)
    }

    public async login(loginRequest: LoginRequest): Promise<LoginResponse> {
        return this.http
            .post<LoginResponse>(`${this.baseUrl}api/auth/login`, loginRequest)
            .pipe(
                tap(res => {
                    AuthService.accessToken = res.accessToken;
                })
            )
            .toPromise();
    }

    public isAuthenticated(): boolean {
        const accessToken = AuthService.accessToken;
        return accessToken && !this.jwtHelper.isTokenExpired();
    }

    public async logout(): Promise<void> {
        localStorage.removeItem(ACCESS_TOKEN_KEY);
        await this.router.navigate(["/login"]);
    }

    public async getCurrentUser() {
        return this.http
            .get<CurrentUser>(`${this.baseUrl}api/profile`)
            .toPromise();
    }
}
