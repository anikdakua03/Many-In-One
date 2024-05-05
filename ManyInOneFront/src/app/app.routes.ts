import { Routes } from '@angular/router';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { LoginComponent } from './components/auth/login/login.component';
import { HomeComponent } from './components/home/home/home.component';
import { WelcomeComponent } from './components/home/welcome/welcome.component';
import { PaymentDetailsComponent } from './components/payment-details/payment-details.component';
import { TextOnlyComponent } from './components/genAI/text-only/text-only.component';
import { TextAndImageOnlyComponent } from './components/genAI/text-and-image-only/text-and-image-only.component';
import { authGuard, loggedInUserGuard } from './shared/guards/auth.guard';
import { ManageAuthComponent } from './components/auth/manage-auth/manage-auth.component';
import { Enable2FAComponent } from './components/auth/2FA/enable2-fa/enable2-fa.component';
import { Disable2FAComponent } from './components/auth/2FA/disable2-fa/disable2-fa.component';
import { ClashingComponent } from './components/clashing/clashing.component';
import { SearchPlayerComponent } from './components/clashing/search-player/search-player.component';
import { SearchClanComponent } from './components/clashing/search-clan/search-clan.component';
import { ClanDetailsComponent } from './components/clashing/search-clan/clan-details/clan-details.component';
import { SummarizeTextComponent } from './components/genAI/summarize-text/summarize-text.component';
import { TextToImageComponent } from './components/genAI/text-to-image/text-to-image.component';
import { ConversationComponent } from './components/genAI/conversation/conversation.component';
import { ConfirmEmailComponent } from './components/auth/confirm-email/confirm-email.component';
import { SendEmailComponent } from './components/auth/send-email/send-email.component';
import { ResetPasswordComponent } from './components/auth/reset-password/reset-password.component';
import { GenerateSpeechComponent } from './components/genAI/generate-speech/generate-speech.component';

export const routes: Routes = [
    {
        'path': '', 'title': 'Welcome', component: WelcomeComponent, // for all users
    },
    {
        'path': 'home', 'title': 'Home', component: HomeComponent, canActivate: [authGuard]
    },
    {
        'path': 'login', 'title': 'Login', component: LoginComponent, canActivate: [loggedInUserGuard]
    },
    {
        'path': 'account/confirm-email', 'title': 'Email Confirmation', component: ConfirmEmailComponent, canActivate: [loggedInUserGuard]
    },
    {
        'path': 'account/send-email/:mode', 'title': 'Send Verification Mail', component: SendEmailComponent
    },
    {
        'path': 'account/reset-password', 'title': 'Reset Password', component: ResetPasswordComponent, canActivate: [loggedInUserGuard]
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
        'path': 'register', 'title': 'Register', component: RegisterComponent, canActivate: [loggedInUserGuard]
    },
    {
        'path': 'payment-details', 'title': 'Payment Details', component: PaymentDetailsComponent,  canActivate: [authGuard]
    },
    {
        'path': 'genAI/textonly', 'title': 'Text Only Input', component: TextOnlyComponent,  canActivate: [authGuard]
    },
    {
        'path': 'genAI/textandimageonly', 'title': 'Image & Text Only Input', component: TextAndImageOnlyComponent,  canActivate : [authGuard]
    },
    {
        'path': 'genAI/summarize', 'title': 'Summarize Text', component: SummarizeTextComponent, canActivate: [authGuard]
    },
    {
        'path': 'genAI/text-to-image', 'title': 'Text to Image', component: TextToImageComponent, canActivate: [authGuard]
    },
    {
        'path': 'genAI/conversation', 'title': 'Chat with AI', component: ConversationComponent,  canActivate: [authGuard]
    },
    {
        'path': 'genAI/generate-speech', 'title': 'Text to Speech', component: GenerateSpeechComponent, canActivate: [authGuard]
    },
    {
        'path': 'clashOfClans', 'title': 'Clashing', component: ClashingComponent,  canActivate : [authGuard]
    },
    {
        'path': 'clashOfClans/search-player', 'title': 'Clash Player', component: SearchPlayerComponent,  canActivate: [authGuard]
    },
    {
        'path': 'clashOfClans/search-clan', 'title': 'Clans', component: SearchClanComponent, canActivate: [authGuard]
    },
    {
        'path': 'clashOfClans/search-clan/clan-details', 'title': 'Clan Details', component: ClanDetailsComponent,  canActivate: [authGuard]
    },
    {
        'path': '**', 'title': 'Page Not Found', component: PageNotFoundComponent
    }
];
