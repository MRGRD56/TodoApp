import {Injectable} from '@angular/core';
import {CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router} from '@angular/router';
import {AuthService} from "../../services/auth/auth.service";

@Injectable({
    providedIn: 'root'
})
export class NotAuthorizedGuard implements CanActivate {
    constructor(
        private readonly auth: AuthService,
        private readonly router: Router
    ) { }

    public async canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot) {

        if (this.auth.isAuthenticated()) {
            await this.router.navigate([""]);
        }

        return true;
    }
}
