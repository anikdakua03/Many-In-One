import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, tap, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from '../services/authentication.service';

let counter: number = 0;

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {

    const authService = inject(AuthenticationService);
    const router = inject(Router);
    const toaster = inject(ToastrService);

    return next(req).pipe(

        catchError((err: HttpErrorResponse) => {
            if (err && err.status === 401 && counter !== 1) {
                counter++; // Increment counter only if attempting refresh

                return authService.refreshToken().pipe(
                    tap(x => {
                        toaster.info("Please reload the page !!!")
                    }), // Emit message without affecting flow

                    catchError(() => {
                        // Refresh failed, revoke token
                        return authService.revokeToken().pipe(
                            tap(x => {
                                router.navigateByUrl('/home');
                                toaster.show("Please login again to continue !!!");
                            }),
                            catchError(() => {
                                console.log("Other error occured --> ", err.message);
                                return throwError(() => new Error("Token revoked")); // Final error handling
                            })
                        );
                    })
                );
            }
            else {
                counter = 0; // Reset counter for other errors
                return throwError(() => new Error(err.message)); // Rethrow original error
            }
        })
    );
};
