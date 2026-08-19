import {
    Routes
} from '@angular/router';

import {
    authGuard
} from './core/guards/auth.guard';

import {
    roleGuard
} from './core/guards/role.guard';

import {
    Login
} from './features/auth/login/login';

import {
    Dashboard
} from './features/dashboard/dashboard';

import {
    EmployeeList
} from './features/employees/employee-list/employee-list';

import {
    EmployeeForm
} from './features/employees/employee-form/employee-form';

import {
    MyProfile
} from './features/employees/my-profile/my-profile';

import {
    MyDepartment
} from './features/employees/my-department/my-department';

import {
    DepartmentList
} from './features/departments/department-list/department-list';

import {
    DepartmentForm
} from './features/departments/department-form/department-form';

import {
    UserList
} from './features/users/user-list/user-list';

import {
    UserCreate
} from './features/users/user-create/user-create';

import {
    UserAccessForm
} from './features/users/user-access-form/user-access-form';

import {
    AccessDenied
} from './features/access-denied/access-denied';

import {
    NotFound
} from './features/not-found/not-found';

import {
    MainLayout
} from './layout/main-layout/main-layout';


export const routes: Routes = [

    {
        path: 'login',
        component: Login
    },

    {
        path: '',

        component:
            MainLayout,

        canActivate: [
            authGuard
        ],

        children: [

            {
                path: '',
                component: Dashboard
            },


            {
                path: 'profile',

                component:
                    MyProfile,

                canActivate: [
                    roleGuard
                ],

                data: {
                    roles: [
                        'Employee'
                    ]
                }
            },


            {
                path: 'my-department',

                component:
                    MyDepartment,

                canActivate: [
                    roleGuard
                ],

                data: {
                    roles: [
                        'Manager'
                    ]
                }
            },


            {
                path: 'employees/new',

                component:
                    EmployeeForm,

                canActivate: [
                    roleGuard
                ],

                data: {
                    roles: [
                        'Admin',
                        'HumanResources'
                    ]
                }
            },


            {
                path: 'employees/:id/edit',

                component:
                    EmployeeForm,

                canActivate: [
                    roleGuard
                ],

                data: {
                    roles: [
                        'Admin',
                        'HumanResources'
                    ]
                }
            },


            {
                path: 'employees',

                component:
                    EmployeeList,

                canActivate: [
                    roleGuard
                ],

                data: {
                    roles: [
                        'Admin',
                        'HumanResources'
                    ]
                }
            },


            {
                path: 'departments/new',

                component:
                    DepartmentForm,

                canActivate: [
                    roleGuard
                ],

                data: {
                    roles: [
                        'Admin'
                    ]
                }
            },


            {
                path: 'departments/:id/edit',

                component:
                    DepartmentForm,

                canActivate: [
                    roleGuard
                ],

                data: {
                    roles: [
                        'Admin'
                    ]
                }
            },


            {
                path: 'departments',

                component:
                    DepartmentList,

                canActivate: [
                    roleGuard
                ],

                data: {
                    roles: [
                        'Admin'
                    ]
                }
            },


            {
                path: 'users/new',

                component:
                    UserCreate,

                canActivate: [
                    roleGuard
                ],

                data: {
                    roles: [
                        'Admin'
                    ]
                }
            },


            {
                path: 'users/:id/access',

                component:
                    UserAccessForm,

                canActivate: [
                    roleGuard
                ],

                data: {
                    roles: [
                        'Admin'
                    ]
                }
            },


            {
                path: 'users',

                component:
                    UserList,

                canActivate: [
                    roleGuard
                ],

                data: {
                    roles: [
                        'Admin'
                    ]
                }
            },


            {
                path: 'access-denied',
                component: AccessDenied
            },


            {
                path: '**',
                component: NotFound
            }

        ]
    }

];