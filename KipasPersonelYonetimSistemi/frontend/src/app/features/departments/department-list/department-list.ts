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
    Department
} from '../../../core/models/department.models';

import {
    DepartmentService
} from '../../../core/services/department.service';

import {
    getApiErrorMessage
} from '../../../core/utils/api-error.util';


@Component({
    selector: 'app-department-list',

    imports: [
        FormsModule
    ],

    templateUrl:
        './department-list.html',

    styleUrl:
        './department-list.scss'
})
export class DepartmentList
    implements OnInit {

    private readonly departmentService =
        inject(DepartmentService);

    private readonly router =
        inject(Router);


    protected readonly departments =
        signal<Department[]>([]);

    protected readonly isLoading =
        signal(false);

    protected readonly errorMessage =
        signal('');

    protected readonly successMessage =
        signal('');

    protected readonly busyDepartmentId =
        signal<number | null>(null);


    protected includeInactive =
        true;


    ngOnInit(): void {

        this.loadDepartments();
    }


    protected loadDepartments(): void {

        this.isLoading.set(true);

        this.errorMessage.set('');

        this.departmentService
            .getDepartments(
                this.includeInactive
            )
            .subscribe({

                next: departments => {

                    this.departments.set(
                        departments
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
                            'Departman listesi yüklenemedi.'
                        )
                    );

                    this.isLoading.set(
                        false
                    );
                }
            });
    }


    protected addDepartment(): void {

        void this.router.navigate(
            ['/departments/new']
        );
    }


    protected editDepartment(
        id: number
    ): void {

        void this.router.navigate(
            [
                '/departments',
                id,
                'edit'
            ]
        );
    }


    protected deactivateDepartment(
        department: Department
    ): void {

        if (!department.isActive) {
            return;
        }

        const confirmed =
            window.confirm(
                `${department.name} departmanını pasif hâle getirmek istediğinize emin misiniz?`
            );

        if (!confirmed) {
            return;
        }


        this.errorMessage.set('');

        this.successMessage.set('');

        this.busyDepartmentId.set(
            department.id
        );


        this.departmentService
            .deactivateDepartment(
                department.id
            )
            .subscribe({

                next: () => {

                    this.successMessage.set(
                        'Departman başarıyla pasif hâle getirildi.'
                    );

                    this.busyDepartmentId.set(
                        null
                    );

                    this.loadDepartments();
                },

                error: (
                    error: HttpErrorResponse
                ) => {

                    this.errorMessage.set(
                        getApiErrorMessage(
                            error,
                            'Departman pasif hâle getirilemedi.'
                        )
                    );

                    this.busyDepartmentId.set(
                        null
                    );
                }
            });
    }


    protected changeInactiveFilter():
        void {

        this.loadDepartments();
    }
}