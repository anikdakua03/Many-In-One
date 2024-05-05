import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, tap, throwError } from 'rxjs';
import { AuthenticationService } from '../services/authentication.service';


export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
    // <>
    const authService = inject(AuthenticationService);
    const router = inject(Router);
    const toaster = inject(ToastrService);

    let refresh : boolean = false;

    return next(req).pipe(
        catchError((err: HttpErrorResponse) => {
            if (err && err.status === 401 && !refresh) {
                refresh = true; // will refresh , so set to true
                return authService.refreshToken().pipe(
                    tap(x => {
                        if (x.isSuccess) {
                            toaster.info("Inactive for too long, Please reload the page !!!", "Authentication Info.");
                        }
                        else {
                            toaster.info("Login expired , Please log out and then log in again to continue ... ", "Authentication Failed.");
                        }
                    }), // Emit message without affecting flow

                    catchError(() => {
                        // Refresh failed, some error occurred while refreshing, so revoke all token
                        authService.removeToken();
                        toaster.show("Please log out and then log in again to continue ... ", "Please Log out and log in again");
                        return authService.revokeToken().pipe(
                            tap(x => {
                                router.navigateByUrl('/login');
                                toaster.show("Please login again to continue !!!", "Please Login Again.");
                            }),
                            catchError(() => {
                                return throwError(() => new Error("Token revoked")); // Final error handling
                            })
                        );
                    })
                );
            }
            else {
                refresh = false; // Reset counter for other errors
                return throwError(() => new Error(err.message)); // Rethrow original error
            }
        })
    );


};
