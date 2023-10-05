import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit{
  members: Partial<Member[]>; // mảng các thành viên mà mỗi thành viên là một đối tượng có các thuộc tính của Member 
  predicate = 'liked'; //  tiêu chí sắp xếp
  pageNumber = 1; // trang hiện tại
  pageSize = 5; // số lượng thành viên trên một trang
  pagination: Pagination; // đối tượng Pagination

  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    this.memberService.getLikes(this.predicate,this.pageNumber,this.pageSize).subscribe(response => {
      this.members = response.result; // mảng các thành viên mà mỗi thành viên là một đối tượng có các thuộc tính của Member
      this.pagination = response.pagination; // đối tượng Pagination
    })
  }

  // this method is called when the user clicks on the pagination buttonsm, it changes the pageNumber and then calls loadLikes()
  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadLikes();
  }

}
