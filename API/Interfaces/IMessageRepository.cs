using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id); // lấy tin nhắn theo id
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams); // lấy danh sách tin nhắn của một người dùng
        
        // lấy danh sách tin nhắn giữa 2 người dùng
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
        Task<bool> SaveAllAsync(); // lưu tất cả các thay đổi vào database

    }
}