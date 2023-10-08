import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit { // this component is used to display the details of a member

  @ViewChild('memberTabs', {static: true}) memberTabs; // this is used to access the tabs in the html file
  member : Member;
  galleryOptions : NgxGalleryOptions[] = []; // this is used to configure the gallery
  galleryImages : NgxGalleryImage[] = []; // this is used to store the images for the gallery
  activeTab: TabDirective; // this is used to store the active tab
  messages: Message[] = []; // this is used to store the messages

  constructor(private memberService: MembersService, private route: ActivatedRoute,
               private messageService: MessageService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data['member']; // this is used to get the member from the route
    })

    this.route.queryParams.subscribe(params => {
      params['tab'] ? this.selectTab(params['tab']) : this.selectTab(0); // this is used to select a tab
    })

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation : NgxGalleryAnimation.Slide,
        preview : false
      }
    ]
    this.galleryImages = this.getImages(); 
  }

  getImages(): NgxGalleryImage[]{
    const imageUrls = [];
    for(const photo of this.member.photos){
      imageUrls.push({
        small : photo?.url,
        medium : photo?.url,
        big : photo?.url
      })
    }
    return imageUrls;
  }

// this is used to load the messages
  loadMessages() {
    this.messageService.getMessageThread(this.member.userName).subscribe(messages => { 
      console.log(messages);
      this.messages = messages;
    })
  } 

   // this is used to select a tab
   selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivated(data: TabDirective){ // this is used to set the active tab
    this.activeTab = data;
    if (this.activeTab.heading === "Messages" && this.messages.length === 0){
      this.loadMessages();
    }
  }
}
