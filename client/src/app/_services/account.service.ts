import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';
import { environment } from 'src/environments/environement';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  // cách đưa http vào api
  baseUrl = environment.apiUrl;

  private currentUserSource = new ReplaySubject<User>(1);

  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http:HttpClient) { }

  // Login function
  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((respone : User) => {
          const user = respone;
            if(user){
              this.setCurrentUser(user);
            }
      }))
  }

  register(model: any) {  
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user : User) => {
        if(user){
          this.setCurrentUser(user);
        }
      }
    ))
  }

  setCurrentUser(user: User){
    localStorage.setItem('user', JSON.stringify(user)); // JSON.stringify() converts a JavaScript object or value to a JSON string
    this.currentUserSource.next(user);
  }

  // đăng xuất
  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null!); // Here, instead of passing 'null', you can pass an empty User object or undefined.
  }
  
}
