import { map } from "rxjs";
import { PaginatedResult } from "../_models/pagination";
import { HttpClient, HttpParams } from "@angular/common/http";

export function getPaginatedResult<T>(url, params, http: HttpClient) { 
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>(); // this is a new instance of PaginatedResult
    return http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body; // response.body is the array of members
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      }
      ));
  }

export function getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams(); // this is a new instance of HttpParams
      params = params.append('pageNumber', pageNumber.toString()); 
      params = params.append('pageSize', pageSize.toString());
    return params;
  }
