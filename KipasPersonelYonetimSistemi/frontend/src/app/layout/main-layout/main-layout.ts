import { Component, inject } from '@angular/core';
import {
    Router,
    RouterLink,
    RouterLinkActive,
    RouterOutlet
} from '@angular/router';

import { AuthService } from '../../core/services/auth.service';

@Component({
    selector: 'app-main-layout',
    imports:[
        RouterOutlet,
        RouterLink,
        RouterLinkActive
    ],
    templateUrl: './main-layout.html',
    styleUrl: './main-layout.scss'
})
export class MainLayout {
    private readonly authService =
        inject(AuthService);

    private readonly router =
        inject(Router);

    protected readonly username =
        this.authService.getUsername();

    protected readonly role =
        this.authService.getRole();

    logout(): void {
        this.authService
            .logout()
            .subscribe({
                next: () => {
                    void this.router.navigate(['/login']);
                },

                error: () => {
                    this.authService.clearSession();

                    void this.router.navigate(['/login']);
                }
            });
    }
}