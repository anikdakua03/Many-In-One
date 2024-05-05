import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from './app.routes';
import { provideToastr } from 'ngx-toastr';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { CLIPBOARD_OPTIONS, ClipboardButtonComponent, MARKED_OPTIONS, MarkedRenderer, provideMarkdown } from 'ngx-markdown';
import { AuthInterceptor } from './shared/interceptors/auth.interceptor';
import { provideClientHydration, withHttpTransferCacheOptions } from '@angular/platform-browser';

export const appConfig: ApplicationConfig = {

  providers: [
    provideRouter(routes), 
    // HttpClient cached outgoing network requests when running on the server.This information is serialized and transferred to the browser as part of the initial HTML sent from the server.In the browser, HttpClient checks whether it has data in the cache and if so, reuses it instead of making a new HTTP request during initial application rendering.HttpClient stops using the cache once an application becomes stable while running in a browser.
    provideClientHydration(withHttpTransferCacheOptions({includePostRequests : true})),
    provideHttpClient(withFetch(), withInterceptors([AuthInterceptor])),
    provideAnimations(), // required animations providers
    provideToastr({
      timeOut: 10000,
      positionClass: 'toast-top-center',
      preventDuplicates: true,
      progressBar : true,
      autoDismiss : true,
      countDuplicates : true
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
        },
      }
    }),
  ]
};
