import { inject } from "@angular/core"
import { AuthenticationService } from "../services/authentication.service"
import { Router } from "@angular/router";

export const authGuard = () =>{
    const authService = inject(AuthenticationService);
    const route = inject(Router);

    if(authService.currUserSignal())
    {
        return true;
    }
    else
    {
        route.navigateByUrl('/login');
        return false;
    }
}

/** Login guard  is to protect already logged in user going to login page again */
export const loginGuard = () =>{
    const authService = inject(AuthenticationService);
    const route = inject(Router);

    if(authService.currUserSignal())
    {
        route.navigateByUrl('/home');
        return false;
    }
    else
    {
        return true;
    }
}