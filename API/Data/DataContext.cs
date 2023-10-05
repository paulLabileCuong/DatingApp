using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Entities;
namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext (DbContextOptions options) : base(options)
        {   

        }

        public DbSet<AppUser> Users { get; set; } // tên bảng trong database sẽ là Users
        public DbSet<UserLike> Likes { get; set; } // tên bảng trong database sẽ là Likes

        // Để tạo quan hệ many-to-many giữa 2 bảng Users và Likes, ta cần override phương thức OnModelCreating
        // trong class DataContext này
        protected override void OnModelCreating(ModelBuilder builder)
        {
             // gọi phương thức OnModelCreating của class cha (DbContext)
            base.OnModelCreating(builder);

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
            
        }

    }
}