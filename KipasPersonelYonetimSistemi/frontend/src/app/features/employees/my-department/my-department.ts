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
    Employee
} from '../../../core/models/employee.models';

import {
    EmployeeService
} from '../../../core/services/employee.service';

import {
    getApiErrorMessage
} from '../../../core/utils/api-error.util';



@Component({
    selector: 'app-my-department',

    imports: [
        FormsModule
    ],

    templateUrl:
        './my-department.html',

    styleUrl:
        './my-department.scss'
})
export class MyDepartment
    implements OnInit {

    private readonly employeeService =
        inject(EmployeeService);


    protected readonly employees =
        signal<Employee[]>([]);

    protected readonly isLoading =
        signal(false);

    protected readonly errorMessage =
        signal('');

    protected readonly totalCount =
        signal(0);

    protected readonly pageNumber =
        signal(1);

    protected readonly pageSize =
        signal(10);


    protected searchText = '';

    protected sortBy = 'id';

    protected sortDirection:
        'asc' | 'desc' = 'asc';


    ngOnInit(): void {

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


    protected loadEmployees(): void {

        this.isLoading.set(true);

        this.errorMessage.set('');


        this.employeeService
            .getMyDepartmentEmployees({

                search:
                    this.searchText,

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
                            'Departman personelleri yüklenemedi.'
                        )
                    );

                    this.isLoading.set(
                        false
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