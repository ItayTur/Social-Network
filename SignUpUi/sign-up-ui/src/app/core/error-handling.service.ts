import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlingService {

  constructor() { }

  handleError(error: any) {
    let errorMessage: string;
    if (error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
      errorMessage = 'Something went wrong. Please try again later.'
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
        if (error.message == "Internal server error"){
          errorMessage = "Oops, something went wrong. Please try again later";
        }else{
          errorMessage = error.error.Message;
        }
    }
    // return an observable with a user-facing error message
    return throwError(
      errorMessage);
  };
}
