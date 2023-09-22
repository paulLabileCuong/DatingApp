using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using Microsoft.AspNetCore.Http;
using API.Interfaces;
using API.Entities;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    // Ủy quyền
    public class UsersController : BaseApiController
    {
        // Sử dụng dấu gạch dưới để có quyền truy cập vào ngữ cảnh phạm vi cơ sở dữ liệu 
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            // Đưa người dùng từ cơ sở dữ liệu vào 1 danh sách
            // để sử dụng 2 danh sách, cần đưa vào 1 khung, lớp hoặc 1 không gian tên khác và gọi Tolisst()
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _userRepository.GetMemberAsync(username);
            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // Lấy tên người dùng trong trường hợp cập nhật
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            // Đang cập nhật hoặc sự dụng điều này để cập nhật 1 đối tượng thì có thể dùng Map()
            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        // Controller cho phép người dùng thêm ảnh mới 
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            // Nhận người dùng theo tên
             var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            // kết quả là 1 dịch vụ ảnh
            var result = await _photoService.AddPhotoAsync(file);
            // Kiểm tra lỗi
            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                // Url an toàn và tuyệt đối
                Url = result.SecureUrl.AbsoluteUri,
                // Id công khai
                PublicId = result.PublicId
            };
            
            // Kiểm tra liệu người dùng có bất kỳ bức ảnh nào vào lúc này hay không
            if (user.Photos.Count ==  0)
            {
                // Nếu không có thì đây là bức ảnh đầu tiên đc tải lên và nó là ảnh chính   
                photo.IsMain = true;
            }

            // Thêm hình ảnh
            user.Photos.Add(photo);

            // Lưu cấc thay đổi
            if (await _userRepository.SaveAllAsync())
            {
                return CreatedAtRoute("GetUser", new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        // Đặt ảnh chính cho người dùng
        public async Task<ActionResult> SetMainPhoto(int photoId){
            // Lấy người dùng
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            // Lấy ảnh chính của người dùng 
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            // Kiểm tra xem ảnh có phải là ảnh chính hay không
            // Nếu là ảnh chính thì trả về BadRequest
            if(photo.IsMain) return BadRequest("This is already your main photo");
            // Lấy ảnh chính hiện tại của người dùng 
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            // Nếu ảnh chính hiện tại khác null thì đặt ảnh chính hiện tại là false
            if(currentMain != null) currentMain.IsMain = false; 
            // Đặt ảnh chính là true
            photo.IsMain = true; 
            // Lưu thay đổi , nếu thành công thì trả về NoContent
            if(await _userRepository.SaveAllAsync()) return NoContent();
            // Nếu không thành công thì trả về BadRequest
            return BadRequest("Failed to set main photo");  
        }
        
        [HttpDelete("delete-photo/{photoId}")]
        // Xóa ảnh
        public async Task<ActionResult> DeletePhoto(int photoId){
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            // Lấy ảnh
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            // Kiểm tra xem ảnh có tồn tại hay không
            if (photo == null) return NotFound();
            // Kiểm tra xem ảnh có phải là ảnh chính hay không
            // Nếu là ảnh chính thì trả về BadRequest
            if(photo.IsMain) return BadRequest("You cannot delete your main photo");
            if(photo.PublicId != null){
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                // Kiểm tra lỗi
                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            // Xóa ảnh
            user.Photos.Remove(photo);
            // Lưu thay đổi
            if(await _userRepository.SaveAllAsync()) return Ok();
            // Nếu không thành công thì trả về BadRequest
            return BadRequest("Failed to delete the photo");
            
        }
    }
}