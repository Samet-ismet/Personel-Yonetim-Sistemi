import {
    Component,
    inject,
    OnInit,
    signal
} from '@angular/core';

import {
    FormBuilder,
    ReactiveFormsModule,
    Validators
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
    EmployeeService
} from '../../../core/services/employee.service';

import {
    UserManagementService
} from '../../../core/services/user-management.service';

import {
    getApiErrorMessage
} from '../../../core/utils/api-error.util';


@Component({
    selector: 'app-user-create',

    imports: [
        ReactiveFormsModule
    ],

    templateUrl:
        './user-create.html',

    styleUrl:
        './user-create.scss'
})
export class UserCreate
    implements OnInit {

    private readonly formBuilder =
        inject(FormBuilder);

    private readonly userManagementService =
        inject(UserManagementService);

    private readonly employeeService =
        inject(EmployeeService);

    private readonly router =
        inject(Router);


    protected readonly employees =
        signal<Employee[]>([]);

    protected readonly isLoadingEmployees =
        signal(false);

    protected readonly isSubmitting =
        signal(false);

    protected readonly errorMessage =
        signal('');


    protected readonly roles = [
        'Admin',
        'HumanResources',
        'Manager',
        'Employee'
    ];


    protected readonly form =
        this.formBuilder.nonNullable.group({

            username: [
                '',
                [
                    Validators.required,
                    Validators.minLength(3),
                    Validators.maxLength(50),
                    Validators.pattern(
                        /^[a-zA-Z0-9._-]+$/
                    )
                ]
            ],

            password: [
                '',
                [
                    Validators.required,
                    Validators.minLength(15),
                    Validators.maxLength(100)
                ]
            ],

            role: [
                'Employee',
                [
                    Validators.required
                ]
            ],

            employeeId: [
                null as number | null
            ]
        });


    ngOnInit(): void {

        this.loadEmployees();
    }


    private loadEmployees(): void {

        this.isLoadingEmployees.set(true);

        this.employeeService
            .getEmployees({

                sortBy:
                    'firstname',

                sortDirection:
                    'asc',

                pageNumber:
                    1,

                pageSize:
                    100
            })
            .subscribe({

                next: result => {

                    this.employees.set(
                        result.items.filter(
                            employee =>
                                employee.isActive
                        )
                    );

                    this.isLoadingEmployees.set(
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

                    this.isLoadingEmployees.set(
                        false
                    );
                }
            });
    }


    protected submit(): void {

        if (
            this.form.invalid ||
            this.isSubmitting()
        ) {

            this.form.markAllAsTouched();

            return;
        }


        this.errorMessage.set('');

        this.isSubmitting.set(true);


        const value =
            this.form.getRawValue();


        this.userManagementService
            .createUser({

                username:
                    value.username.trim(),

                password:
                    value.password,

                role:
                    value.role,

                employeeId:
                    value.employeeId
            })
            .subscribe({

                next: () => {

                    this.isSubmitting.set(
                        false
                    );

                    void this.router.navigate(
                        ['/users']
                    );
                },

                error: (
                    error: HttpErrorResponse
                ) => {

                    this.errorMessage.set(
                        getApiErrorMessage(
                            error,
                            'Kullanıcı oluşturulamadı.'
                        )
                    );

                    this.isSubmitting.set(
                        false
                    );
                }
            });
    }


    protected cancel(): void {

        void this.router.navigate(
            ['/users']
        );
    }
}