import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from './app.routes';
import { provideToastr } from 'ngx-toastr';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { CLIPBOARD_OPTIONS, ClipboardButtonComponent, MARKED_OPTIONS, MarkedRenderer, provideMarkdown } from 'ngx-markdown';
import { AuthInterceptor } from './shared/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {

  providers: [
    provideHttpClient(withFetch(), withInterceptors([AuthInterceptor])),
    provideRouter(routes), 
    provideAnimations(), // required animations providers
    provideToastr({
      timeOut: 10000,
      positionClass: 'toast-top-center',
      preventDuplicates: true,
    }),
    provideMarkdown({
      clipboardOptions: {
        provide: CLIPBOARD_OPTIONS,
        useValue: {
          buttonComponent: ClipboardButtonComponent,
        },
      },
      markedOptions: {
        provide: MARKED_OPTIONS,
        useValue: {
          renderer: new MarkedRenderer(),
          gfm: true,
          breaks: false,
          pedantic: false,
          smartLists: true,
          lineNumber: true,
          langPrefix: 'language-',
          // highlight : function(code, lang)
          // {
          //   if(Prism.language[lang])
          //   {
          //     return Prism.highlight(code, Prism.language[lang], lang);
          //   }
          //   else
          //   {
          //     return code;
          //   }
          // }
        },
      }
    }),
  ] //for markdown configuration need to add here] // Toastr providers]
};
