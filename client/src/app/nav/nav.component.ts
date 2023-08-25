import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})

export class NavComponent implements OnInit {
  
  model : any = {} //empty object

  constructor(public accountService: AccountService) { }
  
  ngOnInit(): void {
  }

  login() {
    // Gọi phương thức login từ service AccountService và đăng ký để lắng nghe kết quả
    this.accountService.login(this.model).subscribe(response => {
      // Được gọi khi yêu cầu đăng nhập thành công
      console.log(response); // In ra thông tin phản hồi từ yêu cầu đăng nhập
    }, error => {
      // Được gọi khi yêu cầu đăng nhập gặp lỗi
      console.log(error); // In ra thông tin lỗi từ yêu cầu đăng nhập
    });
  }

  logout() {
    this.accountService.logout();
    console.log('logged out');
  }

}
