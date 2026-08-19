import {
    inject,
    Injectable
} from '@angular/core';

import {
    HttpClient
} from '@angular/common/http';

import {
    map,
    Observable
} from 'rxjs';

import {
    API_BASE_URL
} from '../config/api.config';

import {
    ApiResponse
} from '../models/api-response';

import {
    UpdateUserAccessRequest,
    UpdateUserStatusRequest,
    UserAccount
} from '../models/user.models';


@Injectable({
    providedIn: 'root'
})
export class UserManagementService {

    private readonly http =
        inject(HttpClient);


    getUsers():
        Observable<UserAccount[]> {

        return this.http
            .get<ApiResponse<UserAccount[]>>(
                `${API_BASE_URL}/UserManagement`
            )
            .pipe(
                map(response => {

                    if (
                        !response.success ||
                        !response.data
                    ) {
                        throw new Error(
                            response.message ||
                            'Kullanıcı listesi alınamadı.'
                        );
                    }

                    return response.data;
                })
            );
    }


    getUserById(
        id: number
    ): Observable<UserAccount> {

        return this.http
            .get<ApiResponse<UserAccount>>(
                `${API_BASE_URL}/UserManagement/${id}`
            )
            .pipe(
                map(response => {

                    if (
                        !response.success ||
                        !response.data
                    ) {
                        throw new Error(
                            response.message ||
                            'Kullanıcı bilgileri alınamadı.'
                        );
                    }

                    return response.data;
                })
            );
    }


    updateAccess(
        id: number,
        request: UpdateUserAccessRequest
    ): Observable<UserAccount> {

        return this.http
            .put<ApiResponse<UserAccount>>(
                `${API_BASE_URL}/UserManagement/${id}/access`,
                request
            )
            .pipe(
                map(response => {

                    if (
                        !response.success ||
                        !response.data
                    ) {
                        throw new Error(
                            response.message ||
                            'Kullanıcı yetkileri güncellenemedi.'
                        );
                    }

                    return response.data;
                })
            );
    }


    updateStatus(
        id: number,
        request: UpdateUserStatusRequest
    ): Observable<UserAccount> {

        return this.http
            .put<ApiResponse<UserAccount>>(
                `${API_BASE_URL}/UserManagement/${id}/status`,
                request
            )
            .pipe(
                map(response => {

                    if (
                        !response.success ||
                        !response.data
                    ) {
                        throw new Error(
                            response.message ||
                            'Kullanıcı durumu güncellenemedi.'
                        );
                    }

                    return response.data;
                })
            );
    }
}