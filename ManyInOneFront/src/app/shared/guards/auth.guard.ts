import { inject } from "@angular/core"
import { AuthenticationService } from "../services/authentication.service"
import { Router } from "@angular/router";

export const authGuard = () =>{
    const authService = inject(AuthenticationService);
    const route = inject(Router);

    if(authService.isLoggegIn())
    {
        return true;
    }
    else
    {
        route.navigate(['/login']);
        return false;
    }
}