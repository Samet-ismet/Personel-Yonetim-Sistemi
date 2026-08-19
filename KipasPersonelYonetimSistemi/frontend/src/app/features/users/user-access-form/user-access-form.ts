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
    ActivatedRoute,
    Router
} from '@angular/router';

import {
    Employee
} from '../../../core/models/employee.models';

import {
    UserAccount
} from '../../../core/models/user.models';

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
    selector: 'app-user-access-form',

    imports: [
        ReactiveFormsModule
    ],

    templateUrl:
        './user-access-form.html',

    styleUrl:
        './user-access-form.scss'
})
export class UserAccessForm
    implements OnInit {

    private readonly formBuilder =
        inject(FormBuilder);

    private readonly userManagementService =
        inject(UserManagementService);

    private readonly employeeService =
        inject(EmployeeService);

    private readonly route =
        inject(ActivatedRoute);

    private readonly router =
        inject(Router);


    protected readonly user =
        signal<UserAccount | null>(null);

    protected readonly employees =
        signal<Employee[]>([]);

    protected readonly isLoading =
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
        this.formBuilder.group({

            role: [
                '',
                [
                    Validators.required
                ]
            ],

            employeeId: [
                null as number | null
            ]
        });


    ngOnInit(): void {

        const idValue =
            this.route.snapshot
                .paramMap
                .get('id');

        const id =
            Number(idValue);


        if (
            !Number.isInteger(id) ||
            id <= 0
        ) {

            void this.router.navigate(
                ['/users']
            );

            return;
        }


        this.loadEmployees();

        this.loadUser(id);
    }


    private loadEmployees(): void {

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
                },

                error: (
                    error: HttpErrorResponse
                ) => {

                    this.errorMessage.set(
                        error.error?.message ??
                        'Personel listesi yüklenemedi.'
                    );
                }
            });
    }


    private loadUser(
        id: number
    ): void {

        this.isLoading.set(true);

        this.errorMessage.set('');


        this.userManagementService
            .getUserById(id)
            .subscribe({

                next: user => {

                    this.user.set(
                        user
                    );

                    this.form.patchValue({

                        role:
                            user.role,

                        employeeId:
                            user.employeeId
                    });

                    this.isLoading.set(
                        false
                    );
                },

                error: (
                    error: HttpErrorResponse
                ) => {

                    this.errorMessage.set(
                        error.error?.message ??
                        'Kullanıcı bilgileri yüklenemedi.'
                    );

                    this.isLoading.set(
                        false
                    );
                }
            });
    }


    protected submit(): void {

        const currentUser =
            this.user();


        if (
            !currentUser ||
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
            .updateAccess(
                currentUser.id,
                {
                    role:
                        value.role!,

                    employeeId:
                        value.employeeId
                }
            )
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
                            'Kullanıcı yetkileri güncellenemedi.'
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