import {
    Component,
    inject
} from '@angular/core';

import {
    RouterLink
} from '@angular/router';

import {
    AuthService
} from '../../core/services/auth.service';


@Component({
    selector: 'app-dashboard',

    imports:[
        RouterLink
    ],

    templateUrl:
        './dashboard.html',

    styleUrl:
        './dashboard.scss'
})
export class Dashboard {

    private readonly authService =
        inject(AuthService);


    protected readonly username =
        this.authService.getUsername();

    protected readonly role =
        this.authService.getRole();


    protected get roleLabel(): string {

        switch (this.role) {

            case 'Admin':
                return 'Sistem Yöneticisi';

            case 'HumanResources':
                return 'İnsan Kaynakları';

            case 'Manager':
                return 'Departman Yöneticisi';

            case 'Employee':
                return 'Personel';

            default:
                return 'Kullanıcı';
        }
    }
}