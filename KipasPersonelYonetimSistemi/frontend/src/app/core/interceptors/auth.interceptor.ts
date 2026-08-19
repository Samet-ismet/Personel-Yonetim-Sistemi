import { inject } from '@angular/core';
import {
    HttpErrorResponse,
    HttpInterceptorFn
} from '@angular/common/http';
import { Router } from '@angular/router';
import {
    catchError,
    switchMap,
    throwError
} from 'rxjs';

import { AuthService } from '../services/auth.service';
import { TokenStorageService } from '../services/token-storage.service';

export const authInterceptor:
    HttpInterceptorFn = (request, next) => {

        const tokenStorage =
            inject(TokenStorageService);

        const authService =
            inject(AuthService);

        const router =
            inject(Router);

        const isAuthRequest =
            request.url.includes('/Auth/login') ||
            request.url.includes('/Auth/register') ||
            request.url.includes('/Auth/refresh') ||
            request.url.includes('/Auth/logout');

        const token =
            tokenStorage.getAccessToken();

        const authenticatedRequest =
            token && !isAuthRequest
                ? request.clone({
                    setHeaders: {
                        Authorization:
                            `Bearer ${token}`
                    }
                })
                : request;

        return next(authenticatedRequest)
            .pipe(
                catchError(
                    (error: HttpErrorResponse) => {

                        if (error.status !== 401 ||
                            isAuthRequest) {
                            return throwError(
                                () => error
                            );
                        }

                        const refreshToken =
                            tokenStorage.getRefreshToken();

                        if (!refreshToken) {
                            tokenStorage.clear();

                            void router.navigate(
                                ['/login']
                            );

                            return throwError(
                                () => error
                            );
                        }

                        return authService
                            .refreshSession()
                            .pipe(
                                switchMap(() => {
                                    const newAccessToken =
                                        tokenStorage
                                            .getAccessToken();

                                    if (!newAccessToken) {
                                        return throwError(
                                            () => error
                                        );
                                    }

                                    const retryRequest =
                                        request.clone({
                                            setHeaders: {
                                                Authorization:
                                                    `Bearer ${newAccessToken}`
                                            }
                                        });

                                    return next(
                                        retryRequest
                                    );
                                }),

                                catchError(
                                    refreshError => {
                                        tokenStorage.clear();

                                        void router.navigate(
                                            ['/login']
                                        );

                                        return throwError(
                                            () => refreshError
                                        );
                                    }
                                )
                            );
                    }
                )
            );
    };