export interface UserAccount {
    id: number;
    username: string;
    role: string;
    isActive: boolean;
    employeeId: number | null;
    employeeName: string | null;
    departmentName: string | null;
    createdAt: string;
}

export interface CreateUserRequest {
    username: string;
    password: string;
    role: string;
    employeeId: number | null;
}

export interface UpdateUserAccessRequest {
    role: string;
    employeeId: number | null;
}

export interface UpdateUserStatusRequest {
    isActive: boolean;
}