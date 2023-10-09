import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';
import { User } from '../_models/user';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit{
  @Input() appHasRole: string[]; // this is the role that we want to check
  user: User;

  constructor(private viewContainerRef: ViewContainerRef
      ,private templateRef: TemplateRef<any>
      ,private accountService: AccountService) {
        this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
          this.user = user; // this is the user object
        })
       }
  ngOnInit(): void {
    // clear view if no roles
    if(!this.user?.roles || this.user == null){
      this.viewContainerRef.clear();
      return;
    }

    // neu user co role nao do trong appHasRole thi se tao view moi va them vao templateRef (templateRef la template cua thang hien tai)
    if(this.user?.roles.some(r => this.appHasRole.includes(r))){
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else
    {
      this.viewContainerRef.clear();
    }
  }
}
