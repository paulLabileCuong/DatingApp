using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser,AppRole,int,
                IdentityUserClaim<int>, AppUserRole,IdentityUserLogin<int>,
                IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext (DbContextOptions options) : base(options)
        {   

        }

        public DbSet<UserLike> Likes { get; set; } // tên bảng trong database sẽ là Likes
        public DbSet<Message> Messages { get; set; } // tên bảng trong database sẽ là Messages

        // Để tạo quan hệ many-to-many giữa 2 bảng Users và Likes, ta cần override phương thức OnModelCreating
        // trong class DataContext này
        protected override void OnModelCreating(ModelBuilder builder)
        {
             // gọi phương thức OnModelCreating của class cha (DbContext)
            base.OnModelCreating(builder);

            // đặt khóa chính cho bảng AppUserRole là 2 cột UserId và RoleId
            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId) // khóa ngoại của bảng AppUserRole là UserId
                .IsRequired(); // bắt buộc phải có giá trị

            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId) // khóa ngoại của bảng AppUserRole là UserId
                .IsRequired(); // bắt buộc phải có giá trị

            // đặt khóa chính cho bảng Likes là 2 cột SourceUserId và LikedUserId
            builder.Entity<UserLike>()
                .HasKey(k => new {k.SourceUserId, k.LikedUserId}); 

            // đặt quan hệ many-to-many giữa 2 bảng Users và Likes
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser) // một người dùng có thể có nhiều like
                .WithMany(l => l.LikedUsers) // một like chỉ thuộc về một người dùng
                .HasForeignKey(s => s.SourceUserId) // khóa ngoại của bảng Likes là SourceUserId
                .OnDelete(DeleteBehavior.Cascade); // nếu người dùng bị xóa thì các like của người dùng đó cũng bị xóa
            
            builder.Entity<UserLike>()
                .HasOne(s => s.LikedUser) // một người dùng có thể có nhiều like
                .WithMany(l => l.LikedByUsers) // một like chỉ thuộc về một người dùng
                .HasForeignKey(s => s.LikedUserId) // khóa ngoại của bảng Likes là LikedUserId
                .OnDelete(DeleteBehavior.Cascade); // nếu người dùng bị xóa thì các like của người dùng đó cũng bị xóa
            
            builder.Entity<Message>()
                .HasOne(u => u.Sender) // một người dùng có thể gửi nhiều tin nhắn
                .WithMany(m => m.MessagesSent) // một tin nhắn chỉ thuộc về một người dùng
                .OnDelete(DeleteBehavior.Restrict); // nếu người dùng bị xóa thì tin nhắn của người dùng đó vẫn được giữ lại

            builder.Entity<Message>()
                .HasOne(u => u.Recipient) // một người dùng có thể nhận nhiều tin nhắn
                .WithMany(m => m.MessagesReceived) // một tin nhắn chỉ thuộc về một người dùng
                .OnDelete(DeleteBehavior.Restrict); // nếu người dùng bị xóa thì tin nhắn của người dùng đó vẫn được giữ lại
        }

    }
}