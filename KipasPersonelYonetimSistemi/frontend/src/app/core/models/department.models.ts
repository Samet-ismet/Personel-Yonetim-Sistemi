export interface Department {
    id: number;
    name: string;
    description: string | null;
    isActive: boolean;
    createdAt: string;
}

export interface DepartmentSaveRequest {
    name: string;
    description: string | null;
    isActive: boolean;
}