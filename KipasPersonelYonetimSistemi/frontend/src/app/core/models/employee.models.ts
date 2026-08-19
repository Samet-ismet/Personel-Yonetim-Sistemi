export interface Employee {
    id: number;
    firstName: string;
    lastName: string;
    departmentId: number;
    departmentName: string;
    isActive: boolean;
}

export interface EmployeeDetail {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    departmentId: number;
    departmentName: string;
    position: string;
    hireDate: string;
    isActive: boolean;
}

export interface EmployeeSaveRequest {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    departmentId: number;
    position: string;
    hireDate: string;
    isActive: boolean;
}

export interface EmployeeQuery {
    search?: string;
    departmentId?: number;

    sortBy: string;

    sortDirection:
    'asc' | 'desc';

    pageNumber: number;
    pageSize: number;
}

export interface PagedResult<T> {
    items: T[];
    pageNumber: number;
    pageSize: number;
    totalCount: number;
}

export interface EmployeeCv {
    employeeId: number;
    fileName: string;
    fileSize: number;
    uploadedAt: string;
}