import { CanDeactivateFn, UrlTree } from '@angular/router';
import { QuizzerPlayComponent } from '../../components/quizzer/quizzer-play/quizzer-play.component';
import { Observable } from 'rxjs';
import { HostListener } from '@angular/core';
import { ComponentCanDeactivate } from './component-can-deactivate';

export type CanDeactivateType = Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree;

export interface CanComponentDeactivate {
  canDeactivate: () => CanDeactivateType;
}

export const quizLeavingGuard: CanDeactivateFn<ComponentCanDeactivate> = (component : ComponentCanDeactivate) => {
  
  // return component.canDeactivate ? component.canDeactivate() : true;

  // if (component !== undefined && component.isQuizStarted === true) {
  //   const confirmation = confirm("You are in the somewhere in between quiz. Are you sure to leave ?");

  //   if (confirmation) {
  //     return true;
  //   }
  //   else {
  //     return false;
  //   }
  // }

  // return !true;
  if (!component.canDeactivate()) {
    if (confirm("You have unsaved changes! If you leave, your changes will be lost.")) {
      return true;
    } else {
      return false;
    }
  }
  return true;

};