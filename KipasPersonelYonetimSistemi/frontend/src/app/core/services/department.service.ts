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
    Department,
    DepartmentSaveRequest
} from '../models/department.models';


@Injectable({
    providedIn: 'root'
})
export class DepartmentService {

    private readonly http =
        inject(HttpClient);


    getDepartments(
        includeInactive = false
    ): Observable<Department[]> {

        return this.http
            .get<ApiResponse<Department[]>>(
                `${API_BASE_URL}/Department`,
                {
                    params: {
                        includeInactive
                    }
                }
            )
            .pipe(
                map(response => {

                    if (
                        !response.success ||
                        !response.data
                    ) {
                        throw new Error(
                            response.message ||
                            'Departmanlar alınamadı.'
                        );
                    }

                    return response.data;
                })
            );
    }


    getActiveDepartments():
        Observable<Department[]> {

        return this.getDepartments(
            false
        );
    }


    getDepartmentById(
        id: number
    ): Observable<Department> {

        return this.http
            .get<ApiResponse<Department>>(
                `${API_BASE_URL}/Department/${id}`
            )
            .pipe(
                map(response => {

                    if (
                        !response.success ||
                        !response.data
                    ) {
                        throw new Error(
                            response.message ||
                            'Departman bilgileri alınamadı.'
                        );
                    }

                    return response.data;
                })
            );
    }


    createDepartment(
        request: DepartmentSaveRequest
    ): Observable<Department> {

        return this.http
            .post<ApiResponse<Department>>(
                `${API_BASE_URL}/Department`,
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
                            'Departman oluşturulamadı.'
                        );
                    }

                    return response.data;
                })
            );
    }


    updateDepartment(
        id: number,
        request: DepartmentSaveRequest
    ): Observable<Department> {

        return this.http
            .put<ApiResponse<Department>>(
                `${API_BASE_URL}/Department/${id}`,
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
                            'Departman güncellenemedi.'
                        );
                    }

                    return response.data;
                })
            );
    }


    deactivateDepartment(
        id: number
    ): Observable<void> {

        return this.http
            .delete<ApiResponse<unknown>>(
                `${API_BASE_URL}/Department/${id}`
            )
            .pipe(
                map(response => {

                    if (!response.success) {
                        throw new Error(
                            response.message ||
                            'Departman pasif hâle getirilemedi.'
                        );
                    }
                })
            );
    }
}