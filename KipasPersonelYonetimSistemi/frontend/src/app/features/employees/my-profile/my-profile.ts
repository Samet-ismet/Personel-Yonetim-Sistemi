import {
    Component,
    inject,
    OnInit,
    signal
} from '@angular/core';

import {
    HttpErrorResponse
} from '@angular/common/http';

import {
    EmployeeDetail
} from '../../../core/models/employee.models';

import {
    EmployeeService
} from '../../../core/services/employee.service';

import {
    getApiErrorMessage
} from '../../../core/utils/api-error.util';

@Component({
    selector: 'app-my-profile',

    templateUrl:
        './my-profile.html',

    styleUrl:
        './my-profile.scss'
})
export class MyProfile
    implements OnInit {

    private readonly employeeService =
        inject(EmployeeService);


    protected readonly profile =
        signal<EmployeeDetail | null>(
            null
        );

    protected readonly isLoading =
        signal(false);

    protected readonly errorMessage =
        signal('');


    ngOnInit(): void {

        this.loadProfile();
    }


    private loadProfile(): void {

        this.isLoading.set(true);

        this.errorMessage.set('');


        this.employeeService
            .getMyProfile()
            .subscribe({

                next: profile => {

                    this.profile.set(
                        profile
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
                            'Profil bilgileri yüklenemedi.'
                        )
                    );

                    this.isLoading.set(
                        false
                    );
                }
            });
    }
}