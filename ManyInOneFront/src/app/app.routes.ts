import { Routes } from '@angular/router';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { LoginComponent } from './components/auth/login/login.component';
import { HomeComponent } from './components/home/home/home.component';
import { WelcomeComponent } from './components/home/welcome/welcome.component';
import { PaymentDetailsComponent } from './components/payment-details/payment-details.component';
import { PaymentDetailsUpdateFormComponent } from './components/payment-details/payment-details-update-form/payment-details-update-form.component';
import { TextOnlyComponent } from './components/genAI/text-only/text-only.component';
import { TextAndImageOnlyComponent } from './components/genAI/text-and-image-only/text-and-image-only.component';
import { authGuard, loginGuard } from './shared/guards/auth.guard';
import { ManageAuthComponent } from './components/auth/manage-auth/manage-auth.component';
import { Enable2FAComponent } from './components/auth/2FA/enable2-fa/enable2-fa.component';
import { Disable2FAComponent } from './components/auth/2FA/disable2-fa/disable2-fa.component';
import { TwoFALoginComponent } from './components/auth/two-falogin/two-falogin.component';

export const routes: Routes = [
    {
        'path': '', 'title': 'Welcome', component: WelcomeComponent, // for all users
    },
    {
        'path': 'home', 'title': 'Home', component: HomeComponent,   canActivate : [authGuard]
    },
    {
        'path': 'login', 'title': 'Login', component: LoginComponent, canActivate : [loginGuard]
        // children: [
        //     {
        //         'path': '2FAa', 'title': 'Login | 2FA', component: TwoFALoginComponent, // canActivate : [authGuard]
        //     },
        // ]
    },
    {
        'path': 'login/2FA', 'title': 'Login with 2FA', component: TwoFALoginComponent, canActivate : [authGuard]
    },
    {
        'path': 'manage', 'title': 'Manage', component: ManageAuthComponent, canActivate : [authGuard]
    },
    {
        'path': 'manage/enable-2FA', 'title': 'Manage', component: Enable2FAComponent, canActivate : [authGuard]
    },
    {
        'path': 'manage/disable-2FA', 'title': 'Manage', component: Disable2FAComponent,  canActivate : [authGuard]
    },
    {
        'path': 'register', 'title': 'Register', component: RegisterComponent
    },
    {
        'path': 'paymentdetails', 'title': 'Payment Details', component: PaymentDetailsComponent,  canActivate : [authGuard]
    },
    // {
    //     'path': 'update-paymentdetails', 'title': 'Payment Details', component: PaymentDetailsUpdateFormComponent, canActivate: [authGuard]
    // },
    {
        'path': 'genAI/textonly', 'title': 'Text Only Input', component: TextOnlyComponent,  canActivate: [authGuard]
    },
    {
        'path': 'genAI/textandimageonly', 'title': 'Image & Text Only Input', component: TextAndImageOnlyComponent,  canActivate : [authGuard]
    },
    {
        'path': '**', 'title': 'Page Not Found', component: PageNotFoundComponent
    }
];
