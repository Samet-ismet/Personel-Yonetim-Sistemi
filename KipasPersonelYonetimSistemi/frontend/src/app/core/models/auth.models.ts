export interface LoginRequest {
    username: string;
    password: string;
}

export interface AuthResponse {
    token: string;
    expiration: string;
    refreshToken: string;
    refreshTokenExpiration: string;
    username: string;
    role: string;
}