import {
    Component,
    inject,
    OnInit,
    signal
} from '@angular/core';

import {
    FormsModule
} from '@angular/forms';

import {
    HttpErrorResponse
} from '@angular/common/http';

import {
    Router
} from '@angular/router';

import {
    Employee
} from '../../../core/models/employee.models';

import {
    Department
} from '../../../core/models/department.models';

import {
    EmployeeService
} from '../../../core/services/employee.service';

import {
    DepartmentService
} from '../../../core/services/department.service';

import {
    AuthService
} from '../../../core/services/auth.service';

import {
    getApiErrorMessage
} from '../../../core/utils/api-error.util';


@Component({
    selector: 'app-employee-list',

    imports: [
        FormsModule
    ],

    templateUrl:
        './employee-list.html',

    styleUrl:
        './employee-list.scss'
})
export class EmployeeList
    implements OnInit {

    private readonly employeeService =
        inject(EmployeeService);

    private readonly departmentService =
        inject(DepartmentService);

    private readonly authService =
        inject(AuthService);

    private readonly router =
        inject(Router);


    protected readonly employees =
        signal<Employee[]>([]);

    protected readonly departments =
        signal<Department[]>([]);

    protected readonly isLoading =
        signal(false);

    protected readonly errorMessage =
        signal('');

    protected readonly successMessage =
        signal('');

    protected readonly totalCount =
        signal(0);

    protected readonly pageNumber =
        signal(1);

    protected readonly pageSize =
        signal(10);

    protected readonly cvBusyEmployeeId =
        signal<number | null>(null);

    protected readonly role =
        this.authService.getRole();


    protected searchText = '';

    protected selectedDepartmentId = 0;

    protected sortBy = 'id';

    protected sortDirection:
        'asc' | 'desc' = 'asc';


    ngOnInit(): void {

        this.loadDepartments();
        this.loadEmployees();
    }


    protected get totalPages(): number {

        return Math.max(
            1,
            Math.ceil(
                this.totalCount() /
                this.pageSize()
            )
        );
    }


    protected get canDelete(): boolean {

        return this.role === 'Admin';
    }


    private loadDepartments(): void {

        this.departmentService
            .getActiveDepartments()
            .subscribe({

                next: departments => {

                    this.departments.set(
                        departments
                    );
                },

                error: (
                    error: HttpErrorResponse
                ) => {

                    this.errorMessage.set(
                        getApiErrorMessage(
                            error,
                            'Departmanlar yüklenemedi.'
                        )
                    );
                }
            });
    }


    protected loadEmployees(): void {

        this.isLoading.set(true);

        this.errorMessage.set('');

        this.employeeService
            .getEmployees({

                search:
                    this.searchText,

                departmentId:
                    this.selectedDepartmentId ||
                    undefined,

                sortBy:
                    this.sortBy,

                sortDirection:
                    this.sortDirection,

                pageNumber:
                    this.pageNumber(),

                pageSize:
                    this.pageSize()
            })
            .subscribe({

                next: result => {

                    this.employees.set(
                        result.items
                    );

                    this.totalCount.set(
                        result.totalCount
                    );

                    this.pageNumber.set(
                        result.pageNumber
                    );

                    this.pageSize.set(
                        result.pageSize
                    );

                    this.isLoading.set(
                        false
                    );
                },

                error: (
                    error: HttpErrorResponse
                ) => {

                    this.errorMessage.set(
                        getApiErrorMessage(
                            error,
                            'Personel listesi yüklenemedi.'
                        )
                    );

                    this.isLoading.set(
                        false
                    );
                }
            });
    }


    protected addEmployee(): void {

        void this.router.navigate(
            ['/employees/new']
        );
    }


    protected editEmployee(
        id: number
    ): void {

        void this.router.navigate(
            [
                '/employees',
                id,
                'edit'
            ]
        );
    }


    protected deleteEmployee(
        employee: Employee
    ): void {

        if (!this.canDelete) {
            return;
        }

        const confirmed =
            window.confirm(
                `${employee.firstName} ${employee.lastName} adlı personeli silmek istediğinize emin misiniz?`
            );

        if (!confirmed) {
            return;
        }


        this.errorMessage.set('');

        this.successMessage.set('');


        this.employeeService
            .deleteEmployee(
                employee.id
            )
            .subscribe({

                next: () => {

                    this.successMessage.set(
                        'Personel başarıyla silindi.'
                    );

                    if (
                        this.employees()
                            .length === 1 &&
                        this.pageNumber() > 1
                    ) {
                        this.pageNumber.update(
                            value => value - 1
                        );
                    }

                    this.loadEmployees();
                },

                error: (
                    error: HttpErrorResponse
                ) => {

                    this.errorMessage.set(
                        getApiErrorMessage(
                            error,
                            'Personel silinemedi.'
                        )
                    );
                }
            });
    }


    protected uploadCv(
        employee: Employee,
        event: Event
    ): void {

        const input =
            event.target as HTMLInputElement;

        const file =
            input.files?.[0];

        if (!file) {
            return;
        }


        this.errorMessage.set('');

        this.successMessage.set('');


        const isPdf =
            file.name
                .toLowerCase()
                .endsWith('.pdf');

        if (!isPdf) {

            this.errorMessage.set(
                'Yalnızca PDF dosyası yükleyebilirsiniz.'
            );

            input.value = '';

            return;
        }


        const maxSize =
            5 * 1024 * 1024;

        if (file.size > maxSize) {

            this.errorMessage.set(
                'CV dosyası en fazla 5 MB olabilir.'
            );

            input.value = '';

            return;
        }


        this.cvBusyEmployeeId.set(
            employee.id
        );


        this.employeeService
            .uploadCv(
                employee.id,
                file
            )
            .subscribe({

                next: result => {

                    this.successMessage.set(
                        `${result.fileName} başarıyla yüklendi.`
                    );

                    this.cvBusyEmployeeId.set(
                        null
                    );

                    input.value = '';
                },

                error: (
                    error: HttpErrorResponse
                ) => {

                    this.errorMessage.set(
                        getApiErrorMessage(
                            error,
                            'CV yüklenirken bir hata oluştu.'
                        )
                    );

                    this.cvBusyEmployeeId.set(
                        null
                    );

                    input.value = '';
                }
            });
    }


    protected downloadCv(
        employee: Employee
    ): void {

        this.errorMessage.set('');

        this.successMessage.set('');

        this.cvBusyEmployeeId.set(
            employee.id
        );


        this.employeeService
            .downloadCv(
                employee.id
            )
            .subscribe({

                next: blob => {

                    const url =
                        URL.createObjectURL(
                            blob
                        );

                    const link =
                        document.createElement(
                            'a'
                        );

                    link.href = url;

                    link.download =
                        `${employee.firstName}-${employee.lastName}-CV.pdf`;

                    document.body.appendChild(
                        link
                    );

                    link.click();

                    link.remove();

                    URL.revokeObjectURL(
                        url
                    );

                    this.cvBusyEmployeeId.set(
                        null
                    );
                },

                error: (
                    error: HttpErrorResponse
                ) => {

                    this.errorMessage.set(
                        getApiErrorMessage(
                            error,
                            'CV bulunamadı veya indirilemedi.'
                        )
                    );

                    this.cvBusyEmployeeId.set(
                        null
                    );
                }
            });
    }


    protected deleteCv(
        employee: Employee
    ): void {

        const confirmed =
            window.confirm(
                `${employee.firstName} ${employee.lastName} adlı personele ait CV dosyasını silmek istediğinize emin misiniz?`
            );

        if (!confirmed) {
            return;
        }


        this.errorMessage.set('');

        this.successMessage.set('');

        this.cvBusyEmployeeId.set(
            employee.id
        );


        this.employeeService
            .deleteCv(
                employee.id
            )
            .subscribe({

                next: () => {

                    this.successMessage.set(
                        'CV dosyası başarıyla silindi.'
                    );

                    this.cvBusyEmployeeId.set(
                        null
                    );
                },

                error: (
                    error: HttpErrorResponse
                ) => {

                    this.errorMessage.set(
                        getApiErrorMessage(
                            error,
                            'CV dosyası silinemedi.'
                        )
                    );

                    this.cvBusyEmployeeId.set(
                        null
                    );
                }
            });
    }


    protected search(): void {

        this.pageNumber.set(1);

        this.loadEmployees();
    }


    protected clearSearch(): void {

        this.searchText = '';

        this.selectedDepartmentId = 0;

        this.pageNumber.set(1);

        this.loadEmployees();
    }


    protected changeDepartment(): void {

        this.pageNumber.set(1);

        this.loadEmployees();
    }


    protected changeSort(): void {

        this.pageNumber.set(1);

        this.loadEmployees();
    }


    protected previousPage(): void {

        if (this.pageNumber() <= 1) {
            return;
        }

        this.pageNumber.update(
            value => value - 1
        );

        this.loadEmployees();
    }


    protected nextPage(): void {

        if (
            this.pageNumber() >=
            this.totalPages
        ) {
            return;
        }

        this.pageNumber.update(
            value => value + 1
        );

        this.loadEmployees();
    }
}