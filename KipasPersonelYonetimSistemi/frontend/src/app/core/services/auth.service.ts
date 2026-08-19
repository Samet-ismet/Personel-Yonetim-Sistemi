import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
    finalize,
    map,
    Observable,
    shareReplay,
    throwError
} from 'rxjs';

import { API_BASE_URL } from '../config/api.config';
import { ApiResponse } from '../models/api-response';
import {
    AuthResponse,
    LoginRequest
} from '../models/auth.models';
import { TokenStorageService } from './token-storage.service';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private readonly http =
        inject(HttpClient);

    private readonly tokenStorage =
        inject(TokenStorageService);

    private refreshRequest$:
        Observable<AuthResponse> | null = null;

    login(
        request: LoginRequest
    ): Observable<AuthResponse> {
        return this.http
            .post<ApiResponse<AuthResponse>>(
                `${API_BASE_URL}/Auth/login`,
                request
            )
            .pipe(
                map(response => {
                    if (!response.success ||
                        !response.data) {
                        throw new Error(
                            response.message ||
                            'Giriş işlemi başarısız.'
                        );
                    }

                    this.tokenStorage.saveSession(
                        response.data
                    );

                    return response.data;
                })
            );
    }

    refreshSession():
        Observable<AuthResponse> {

        if (this.refreshRequest$) {
            return this.refreshRequest$;
        }

        const refreshToken =
            this.tokenStorage.getRefreshToken();

        if (!refreshToken) {
            return throwError(
                () => new Error(
                    'Refresh token bulunamadı.'
                )
            );
        }

        this.refreshRequest$ =
            this.http
                .post<ApiResponse<AuthResponse>>(
                    `${API_BASE_URL}/Auth/refresh`,
                    {
                        refreshToken
                    }
                )
                .pipe(
                    map(response => {
                        if (!response.success ||
                            !response.data) {
                            throw new Error(
                                response.message ||
                                'Oturum yenilenemedi.'
                            );
                        }

                        this.tokenStorage.saveSession(
                            response.data
                        );

                        return response.data;
                    }),

                    finalize(() => {
                        this.refreshRequest$ = null;
                    }),

                    shareReplay({
                        bufferSize: 1,
                        refCount: false
                    })
                );

        return this.refreshRequest$;
    }

    logout(): Observable<void> {
        const refreshToken =
            this.tokenStorage.getRefreshToken();

        if (!refreshToken) {
            this.tokenStorage.clear();

            return new Observable<void>(
                subscriber => {
                    subscriber.next();
                    subscriber.complete();
                }
            );
        }

        return this.http
            .post<ApiResponse<unknown>>(
                `${API_BASE_URL}/Auth/logout`,
                {
                    refreshToken
                }
            )
            .pipe(
                map(() => {
                    this.tokenStorage.clear();
                })
            );
    }

    isAuthenticated(): boolean {
        return this.tokenStorage
            .hasAccessToken();
    }

    getUsername(): string | null {
        return this.tokenStorage
            .getUsername();
    }

    getRole(): string | null {
        return this.tokenStorage
            .getRole();
    }

    clearSession(): void {
        this.tokenStorage.clear();
    }
}