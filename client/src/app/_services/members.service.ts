import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environement';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';


@Injectable({
  providedIn: 'root'
})

export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map(); 
  user: User;
  userParams: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user; // this is the user object
      this.userParams = new UserParams(user); // this is the userParams object
    })
  }
  
  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  // this method is called from member-list.component.ts when the page loads and when the user clicks on the pagination buttons
  getMembers(userParams: UserParams) { 
    // Object.values(userParams) returns an array of the values of userParams 
    console.log(Object.values(userParams).join('-'));
    var response = this.memberCache.get(Object.values(userParams).join('-')); 

    // if the response is not null, then I return the response (and I don't have to wait for the response from the server)
    if (response) {
      return of(response);
    }

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender',userParams.gender);
    params = params.append('orderBy',userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http)
      .pipe(map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response); // I add the response to the memberCache
        return response;
      }
    ))
  } 

  // this method is called from member-detail.component.ts and member-edit.component.ts 
  getMember(username: string) { 
    // I get the member from the memberCache 
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.userName === username);

    if(member) {
      return of(member); // if the member is not null, then I return the member (and I don't have to wait for the response from the server)
    }

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  } 

  // this method is called from member-edit.component.ts 
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  } 

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  // this method is called from member-card.component.ts
  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getLikes(predicate: string, pageNumber , pageSize){
    let params = getPaginationHeaders(pageNumber,pageSize); // this is a new instance of HttpParams
    params = params.append('predicate', predicate); // predicate is either 'liked' or 'likedBy'
    return getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params, this.http); // predicate is either 'liked' or 'likedBy'
  }

}
