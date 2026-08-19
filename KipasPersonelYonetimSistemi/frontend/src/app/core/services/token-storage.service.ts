import { Injectable } from '@angular/core';
import { AuthResponse } from '../models/auth.models';

@Injectable({
    providedIn: 'root'
})
export class TokenStorageService {
    private readonly accessTokenKey =
        'kipas_access_token';

    private readonly refreshTokenKey =
        'kipas_refresh_token';

    private readonly usernameKey =
        'kipas_username';

    private readonly roleKey =
        'kipas_role';

    saveSession(auth: AuthResponse): void {
        sessionStorage.setItem(
            this.accessTokenKey,
            auth.token
        );

        sessionStorage.setItem(
            this.refreshTokenKey,
            auth.refreshToken
        );

        sessionStorage.setItem(
            this.usernameKey,
            auth.username
        );

        sessionStorage.setItem(
            this.roleKey,
            auth.role
        );
    }

    getAccessToken(): string | null {
        return sessionStorage.getItem(
            this.accessTokenKey
        );
    }

    getRefreshToken(): string | null {
        return sessionStorage.getItem(
            this.refreshTokenKey
        );
    }

    getUsername(): string | null {
        return sessionStorage.getItem(
            this.usernameKey
        );
    }

    getRole(): string | null {
        return sessionStorage.getItem(
            this.roleKey
        );
    }

    hasAccessToken(): boolean {
        return this.getAccessToken() !== null;
    }

    clear(): void {
        sessionStorage.removeItem(
            this.accessTokenKey
        );

        sessionStorage.removeItem(
            this.refreshTokenKey
        );

        sessionStorage.removeItem(
            this.usernameKey
        );

        sessionStorage.removeItem(
            this.roleKey
        );
    }
}