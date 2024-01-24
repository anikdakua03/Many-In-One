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
import { authGuard } from './shared/guards/auth.guard';

export const routes: Routes = [
    {
        'path': '', 'title': 'Welcome', component: WelcomeComponent, // for all users
    },
    {
        'path': 'home', 'title': 'Home', component: HomeComponent,   canActivate : [authGuard]
    },
    {
        'path': 'login', 'title': 'Login', component: LoginComponent , // canActivate : [authGuard]
    },
    {
        'path': 'register', 'title': 'Register', component: RegisterComponent
    },
    {
        'path': 'paymentdetails', 'title': 'Payment Details', component: PaymentDetailsComponent,  canActivate : [authGuard]
    },
    {
        'path': 'update-paymentdetails', 'title': 'Payment Details', component: PaymentDetailsUpdateFormComponent
    },
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
