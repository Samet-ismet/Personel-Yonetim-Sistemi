import {
    Component,
    inject,
    OnInit,
    signal
} from '@angular/core';

import {
    AbstractControl,
    FormBuilder,
    ReactiveFormsModule,
    ValidationErrors,
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
    Department
} from '../../../core/models/department.models';

import {
    EmployeeSaveRequest
} from '../../../core/models/employee.models';

import {
    DepartmentService
} from '../../../core/services/department.service';

import {
    EmployeeService
} from '../../../core/services/employee.service';

import {
    getApiErrorMessage
} from '../../../core/utils/api-error.util';


function notFutureDate(
    control: AbstractControl
): ValidationErrors | null {

    const value =
        control.value as string;

    if (!value) {
        return null;
    }

    const today =
        new Date()
            .toISOString()
            .slice(0, 10);

    if (value > today) {

        return {
            futureDate: true
        };
    }

    return null;
}


@Component({
    selector: 'app-employee-form',

    imports: [
        ReactiveFormsModule
    ],

    templateUrl:
        './employee-form.html',

    styleUrl:
        './employee-form.scss'
})
export class EmployeeForm
    implements OnInit {

    private readonly formBuilder =
        inject(FormBuilder);

    private readonly employeeService =
        inject(EmployeeService);

    private readonly departmentService =
        inject(DepartmentService);

    private readonly route =
        inject(ActivatedRoute);

    private readonly router =
        inject(Router);


    protected readonly departments =
        signal<Department[]>([]);

    protected readonly isLoading =
        signal(false);

    protected readonly isSubmitting =
        signal(false);

    protected readonly errorMessage =
        signal('');

    protected readonly isEditMode =
        signal(false);

    protected readonly employeeId =
        signal<number | null>(null);

    protected readonly maxHireDate =
        new Date()
            .toISOString()
            .slice(0, 10);


    protected readonly form =
        this.formBuilder.nonNullable.group({

            firstName: [
                '',
                [
                    Validators.required,
                    Validators.maxLength(50)
                ]
            ],

            lastName: [
                '',
                [
                    Validators.required,
                    Validators.maxLength(50)
                ]
            ],

            email: [
                '',
                [
                    Validators.required,
                    Validators.email,
                    Validators.maxLength(254)
                ]
            ],

            phoneNumber: [
                '',
                [
                    Validators.required,
                    Validators.maxLength(30)
                ]
            ],

            departmentId: [
                0,
                [
                    Validators.required,
                    Validators.min(1)
                ]
            ],

            position: [
                '',
                [
                    Validators.required,
                    Validators.maxLength(100)
                ]
            ],

            hireDate: [
                '',
                [
                    Validators.required,
                    notFutureDate
                ]
            ],

            isActive: [
                true
            ]
        });


    ngOnInit(): void {

        this.loadDepartments();

        const idValue =
            this.route.snapshot
                .paramMap
                .get('id');

        if (!idValue) {
            return;
        }

        const id =
            Number(idValue);

        if (
            !Number.isInteger(id) ||
            id <= 0
        ) {

            void this.router.navigate(
                ['/employees']
            );

            return;
        }

        this.employeeId.set(id);

        this.isEditMode.set(true);

        this.loadEmployee(id);
    }


    protected get pageTitle(): string {

        return this.isEditMode()
            ? 'Personel Düzenle'
            : 'Yeni Personel';
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


    private loadEmployee(
        id: number
    ): void {

        this.isLoading.set(true);

        this.errorMessage.set('');

        this.employeeService
            .getEmployeeById(id)
            .subscribe({

                next: employee => {

                    this.form.patchValue({

                        firstName:
                            employee.firstName,

                        lastName:
                            employee.lastName,

                        email:
                            employee.email,

                        phoneNumber:
                            employee.phoneNumber,

                        departmentId:
                            employee.departmentId,

                        position:
                            employee.position,

                        hireDate:
                            employee.hireDate
                                .substring(0, 10),

                        isActive:
                            employee.isActive
                    });

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
                            'Personel bilgileri yüklenemedi.'
                        )
                    );

                    this.isLoading.set(
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


        const request:
            EmployeeSaveRequest = {

            firstName:
                value.firstName.trim(),

            lastName:
                value.lastName.trim(),

            email:
                value.email.trim(),

            phoneNumber:
                value.phoneNumber.trim(),

            departmentId:
                value.departmentId,

            position:
                value.position.trim(),

            hireDate:
                `${value.hireDate}T00:00:00`,

            isActive:
                value.isActive
        };


        const id =
            this.employeeId();


        const operation =
            this.isEditMode() && id
                ? this.employeeService
                    .updateEmployee(
                        id,
                        request
                    )
                : this.employeeService
                    .createEmployee(
                        request
                    );


        operation.subscribe({

            next: () => {

                this.isSubmitting.set(
                    false
                );

                void this.router.navigate(
                    ['/employees']
                );
            },

            error: (
                error: HttpErrorResponse
            ) => {

                this.errorMessage.set(
                    getApiErrorMessage(
                        error,
                        this.isEditMode()
                            ? 'Personel güncellenirken bir hata oluştu.'
                            : 'Personel eklenirken bir hata oluştu.'
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
            ['/employees']
        );
    }
}