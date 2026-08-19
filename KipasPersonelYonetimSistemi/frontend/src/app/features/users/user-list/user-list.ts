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
    UserAccount
} from '../../../core/models/user.models';

import {
    UserManagementService
} from '../../../core/services/user-management.service';

import {
    getApiErrorMessage
} from '../../../core/utils/api-error.util';


@Component({
    selector: 'app-user-list',

    imports: [
        FormsModule
    ],

    templateUrl:
        './user-list.html',

    styleUrl:
        './user-list.scss'
})
export class UserList
    implements OnInit {

    private readonly userManagementService =
        inject(UserManagementService);

    private readonly router =
        inject(Router);


    protected readonly users =
        signal<UserAccount[]>([]);

    protected readonly isLoading =
        signal(false);

    protected readonly errorMessage =
        signal('');

    protected readonly successMessage =
        signal('');

    protected readonly busyUserId =
        signal<number | null>(null);


    ngOnInit(): void {

        this.loadUsers();
    }


    protected loadUsers(): void {

        this.isLoading.set(true);

        this.errorMessage.set('');

        this.userManagementService
            .getUsers()
            .subscribe({

                next: users => {

                    this.users.set(
                        users
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
                            'Kullanıcı listesi yüklenemedi.'
                        )
                    );

                    this.isLoading.set(
                        false
                    );
                }
            });
    }


    protected addUser(): void {

        void this.router.navigate(
            ['/users/new']
        );
    }


    protected editAccess(
        id: number
    ): void {

        void this.router.navigate(
            [
                '/users',
                id,
                'access'
            ]
        );
    }


    protected changeStatus(
        user: UserAccount
    ): void {

        const newStatus =
            !user.isActive;

        const action =
            newStatus
                ? 'aktif'
                : 'pasif';


        const confirmed =
            window.confirm(
                `${user.username} kullanıcısını ${action} hâle getirmek istediğinize emin misiniz?`
            );

        if (!confirmed) {
            return;
        }


        this.busyUserId.set(
            user.id
        );

        this.errorMessage.set('');

        this.successMessage.set('');


        this.userManagementService
            .updateStatus(
                user.id,
                {
                    isActive:
                        newStatus
                }
            )
            .subscribe({

                next: updatedUser => {

                    this.users.update(
                        users =>
                            users.map(
                                currentUser =>
                                    currentUser.id ===
                                        updatedUser.id
                                        ? updatedUser
                                        : currentUser
                            )
                    );

                    this.successMessage.set(
                        newStatus
                            ? 'Kullanıcı başarıyla aktif hâle getirildi.'
                            : 'Kullanıcı başarıyla pasif hâle getirildi.'
                    );

                    this.busyUserId.set(
                        null
                    );
                },

                error: (
                    error: HttpErrorResponse
                ) => {

                    this.errorMessage.set(
                        getApiErrorMessage(
                            error,
                            'Kullanıcı durumu değiştirilemedi.'
                        )
                    );

                    this.busyUserId.set(
                        null
                    );
                }
            });
    }
}