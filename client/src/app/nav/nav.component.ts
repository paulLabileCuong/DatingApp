import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})

export class NavComponent implements OnInit {
  
  model : any = {} //empty object

  constructor(public accountService: AccountService, private router: Router, private toastr : ToastrService) { }
  
  ngOnInit(): void {
  }

  login() {
    // Gọi phương thức login từ service AccountService và đăng ký để lắng nghe kết quả
    this.accountService.login(this.model).subscribe(response => {
      this.router.navigateByUrl('/members'); // Chuyển hướng đến trang members
    }, error => {
      // Được gọi khi yêu cầu đăng nhập gặp lỗi
      console.log(error); // In ra thông tin lỗi từ yêu cầu đăng nhập
      this.toastr.error(error.error); // Hiển thị thông báo lỗi
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
