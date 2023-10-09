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

    user.roles = [];
    const roles = this.getDecodedToken(user.token).role; // getDecodedToken() is a custom method that decodes the token.
    
    //neu roles la 1 mang thi gan roles cho user.roles, neu khong thi push roles vao user.roles
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles); 

    localStorage.setItem('user', JSON.stringify(user)); // JSON.stringify() converts a JavaScript object or value to a JSON string
    this.currentUserSource.next(user);
  }

  // đăng xuất
  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null!); // Here, instead of passing 'null', you can pass an empty User object or undefined.
  }
  
  // lấy token từ localStorage và decode nó để lấy ra các thông tin của user đó 
  getDecodedToken(token: string) {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
