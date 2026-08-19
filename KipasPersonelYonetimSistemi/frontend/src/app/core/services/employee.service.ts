import {
    inject,
    Injectable
} from '@angular/core';

import {
    HttpClient,
    HttpParams
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
    Employee,
    EmployeeCv,
    EmployeeDetail,
    EmployeeQuery,
    EmployeeSaveRequest,
    PagedResult
} from '../models/employee.models';


@Injectable({
    providedIn: 'root'
})
export class EmployeeService {

    private readonly http =
        inject(HttpClient);


    getEmployees(
        query: EmployeeQuery
    ): Observable<PagedResult<Employee>> {

        let params =
            new HttpParams()
                .set(
                    'PageNumber',
                    String(query.pageNumber)
                )
                .set(
                    'PageSize',
                    String(query.pageSize)
                )
                .set(
                    'SortBy',
                    query.sortBy
                )
                .set(
                    'SortDirection',
                    query.sortDirection
                );

        const search =
            query.search?.trim();

        if (search) {
            params = params.set(
                'Search',
                search
            );
        }

        if (query.departmentId) {
            params = params.set(
                'DepartmentId',
                String(query.departmentId)
            );
        }

        return this.http
            .get<
                ApiResponse<
                    PagedResult<Employee>
                >
            >(
                `${API_BASE_URL}/Employee`,
                {
                    params
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
                            'Personel listesi alınamadı.'
                        );
                    }

                    return response.data;
                })
            );
    }


    getEmployeeById(
        id: number
    ): Observable<EmployeeDetail> {

        return this.http
            .get<
                ApiResponse<EmployeeDetail>
            >(
                `${API_BASE_URL}/Employee/${id}`
            )
            .pipe(
                map(response => {

                    if (
                        !response.success ||
                        !response.data
                    ) {
                        throw new Error(
                            response.message ||
                            'Personel bilgileri alınamadı.'
                        );
                    }

                    return response.data;
                })
            );
    }


    createEmployee(
        request: EmployeeSaveRequest
    ): Observable<Employee> {

        return this.http
            .post<
                ApiResponse<Employee>
            >(
                `${API_BASE_URL}/Employee`,
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
                            'Personel eklenemedi.'
                        );
                    }

                    return response.data;
                })
            );
    }


    updateEmployee(
        id: number,
        request: EmployeeSaveRequest
    ): Observable<Employee> {

        return this.http
            .put<
                ApiResponse<Employee>
            >(
                `${API_BASE_URL}/Employee/${id}`,
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
                            'Personel güncellenemedi.'
                        );
                    }

                    return response.data;
                })
            );
    }


    deleteEmployee(
        id: number
    ): Observable<void> {

        return this.http
            .delete<
                ApiResponse<unknown>
            >(
                `${API_BASE_URL}/Employee/${id}`
            )
            .pipe(
                map(response => {

                    if (!response.success) {
                        throw new Error(
                            response.message ||
                            'Personel silinemedi.'
                        );
                    }
                })
            );
    }


    uploadCv(
        employeeId: number,
        file: File
    ): Observable<EmployeeCv> {

        const formData =
            new FormData();

        formData.append(
            'File',
            file
        );

        return this.http
            .post<
                ApiResponse<EmployeeCv>
            >(
                `${API_BASE_URL}/Employee/${employeeId}/cv`,
                formData
            )
            .pipe(
                map(response => {

                    if (
                        !response.success ||
                        !response.data
                    ) {
                        throw new Error(
                            response.message ||
                            'CV yüklenemedi.'
                        );
                    }

                    return response.data;
                })
            );
    }


    downloadCv(
        employeeId: number
    ): Observable<Blob> {

        return this.http.get(
            `${API_BASE_URL}/Employee/${employeeId}/cv`,
            {
                responseType: 'blob'
            }
        );
    }


    deleteCv(
        employeeId: number
    ): Observable<void> {

        return this.http
            .delete<
                ApiResponse<unknown>
            >(
                `${API_BASE_URL}/Employee/${employeeId}/cv`
            )
            .pipe(
                map(response => {

                    if (!response.success) {
                        throw new Error(
                            response.message ||
                            'CV silinemedi.'
                        );
                    }
                })
            );
    }


    getMyProfile():
        Observable<EmployeeDetail> {

        return this.http
            .get<
                ApiResponse<EmployeeDetail>
            >(
                `${API_BASE_URL}/Employee/me`
            )
            .pipe(
                map(response => {

                    if (
                        !response.success ||
                        !response.data
                    ) {
                        throw new Error(
                            response.message ||
                            'Personel profili alınamadı.'
                        );
                    }

                    return response.data;
                })
            );
    }


    getMyDepartmentEmployees(
        query: EmployeeQuery
    ): Observable<PagedResult<Employee>> {

        let params =
            new HttpParams()
                .set(
                    'PageNumber',
                    String(query.pageNumber)
                )
                .set(
                    'PageSize',
                    String(query.pageSize)
                )
                .set(
                    'SortBy',
                    query.sortBy
                )
                .set(
                    'SortDirection',
                    query.sortDirection
                );

        const search =
            query.search?.trim();

        if (search) {
            params = params.set(
                'Search',
                search
            );
        }

        return this.http
            .get<
                ApiResponse<
                    PagedResult<Employee>
                >
            >(
                `${API_BASE_URL}/Employee/my-department`,
                {
                    params
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
                            'Departman personelleri alınamadı.'
                        );
                    }

                    return response.data;
                })
            );
    }
}