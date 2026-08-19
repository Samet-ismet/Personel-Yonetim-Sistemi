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
    DepartmentSaveRequest
} from '../../../core/models/department.models';

import {
    DepartmentService
} from '../../../core/services/department.service';

import {
    getApiErrorMessage
} from '../../../core/utils/api-error.util';


@Component({
    selector: 'app-department-form',

    imports: [
        ReactiveFormsModule
    ],

    templateUrl:
        './department-form.html',

    styleUrl:
        './department-form.scss'
})
export class DepartmentForm
    implements OnInit {

    private readonly formBuilder =
        inject(FormBuilder);

    private readonly departmentService =
        inject(DepartmentService);

    private readonly route =
        inject(ActivatedRoute);

    private readonly router =
        inject(Router);


    protected readonly isEditMode =
        signal(false);

    protected readonly departmentId =
        signal<number | null>(null);

    protected readonly isLoading =
        signal(false);

    protected readonly isSubmitting =
        signal(false);

    protected readonly errorMessage =
        signal('');


    protected readonly form =
        this.formBuilder.nonNullable.group({

            name: [
                '',
                [
                    Validators.required,
                    Validators.maxLength(100)
                ]
            ],

            description: [
                '',
                [
                    Validators.maxLength(250)
                ]
            ],

            isActive: [
                true
            ]
        });


    ngOnInit(): void {

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
                ['/departments']
            );

            return;
        }

        this.departmentId.set(id);

        this.isEditMode.set(true);

        this.loadDepartment(id);
    }


    protected get pageTitle(): string {

        return this.isEditMode()
            ? 'Departman Düzenle'
            : 'Yeni Departman';
    }


    private loadDepartment(
        id: number
    ): void {

        this.isLoading.set(true);

        this.errorMessage.set('');

        this.departmentService
            .getDepartmentById(id)
            .subscribe({

                next: department => {

                    this.form.patchValue({

                        name:
                            department.name,

                        description:
                            department.description ?? '',

                        isActive:
                            department.isActive
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
                            'Departman bilgileri yüklenemedi.'
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

        const description =
            value.description.trim();


        const request:
            DepartmentSaveRequest = {

            name:
                value.name.trim(),

            description:
                description.length > 0
                    ? description
                    : null,

            isActive:
                value.isActive
        };


        const id =
            this.departmentId();


        const operation =
            this.isEditMode() && id
                ? this.departmentService
                    .updateDepartment(
                        id,
                        request
                    )
                : this.departmentService
                    .createDepartment(
                        request
                    );


        operation.subscribe({

            next: () => {

                this.isSubmitting.set(
                    false
                );

                void this.router.navigate(
                    ['/departments']
                );
            },

            error: (
                error: HttpErrorResponse
            ) => {

                this.errorMessage.set(
                    getApiErrorMessage(
                        error,
                        this.isEditMode()
                            ? 'Departman güncellenirken bir hata oluştu.'
                            : 'Departman oluşturulurken bir hata oluştu.'
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
            ['/departments']
        );
    }
}