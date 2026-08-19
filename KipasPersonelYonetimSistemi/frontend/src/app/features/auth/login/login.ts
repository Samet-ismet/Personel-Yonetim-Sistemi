import {
    Component,
    inject,
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
import { Router } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

@Component({
    selector: 'app-login',
    imports: [
        ReactiveFormsModule
    ],
    templateUrl: './login.html',
    styleUrl: './login.scss'
})
export class Login {
    private readonly formBuilder =
        inject(FormBuilder);

    private readonly authService =
        inject(AuthService);

    private readonly router =
        inject(Router);

    protected readonly isSubmitting =
        signal(false);

    protected readonly errorMessage =
        signal<string | null>(null);

    protected readonly loginForm =
        this.formBuilder.nonNullable.group({
            username: [
                '',
                [
                    Validators.required,
                    Validators.maxLength(50)
                ]
            ],

            password: [
                '',
                [
                    Validators.required,
                    Validators.maxLength(100)
                ]
            ]
        });

    submit(): void {
        if (this.loginForm.invalid ||
            this.isSubmitting()) {
            this.loginForm.markAllAsTouched();
            return;
        }

        this.errorMessage.set(null);
        this.isSubmitting.set(true);

        this.authService
            .login(
                this.loginForm.getRawValue()
            )
            .subscribe({
                next: () => {
                    this.isSubmitting.set(false);

                    void this.router.navigateByUrl('/');
                },

                error: (
                    error: HttpErrorResponse
                ) => {
                    this.isSubmitting.set(false);

                    this.errorMessage.set(
                        error.error?.message ??
                        'Giriş sırasında bir hata oluştu.'
                    );
                }
            });
    }
}