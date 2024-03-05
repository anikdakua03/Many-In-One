import { inject } from "@angular/core"
import { AuthenticationService } from "../services/authentication.service"
import { Router } from "@angular/router";

export const authGuard = () => {
    const authService = inject(AuthenticationService);
    const route = inject(Router);

    // if(authService.currUserSignal !== null && authService.currUserSignal !== undefined)

    // get the user if there 
    let user = authService.isAuthenticated$.value;
    if (user) // true
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
// Need to check this
export const loggedInUserGuard = () => {
    const authService = inject(AuthenticationService);
    const route = inject(Router);

    // get the user if there 
    let user = authService.isAuthenticated$.value;
    if (user) // if true then why to login page....??? so false
    {
        route.navigateByUrl('/home');
        return false;
    }
    else
    {
        return true;
    }
}