import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import {provideAnimations} from '@angular/platform-browser/animations';

import { routes } from './app.routes';
import { HttpClient, provideHttpClient } from '@angular/common/http';
import { platformBrowser } from '@angular/platform-browser';
import { animation } from '@angular/animations';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes), 
    provideHttpClient(),provideAnimations()]
    
};
