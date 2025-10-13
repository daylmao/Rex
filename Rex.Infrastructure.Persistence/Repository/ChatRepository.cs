using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class ChatRepository(RexContext context) : GenericRepository<Chat>(context), IChatRepository
{
    public async Task<PagedResult<Chat>> GetChatsWithLastMessageByUserIdAsync(
        Guid userId, int page, int size, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        var baseQuery = context.Set<Chat>()
            .AsNoTracking()
            .Where(c => c.UserChats.Any(uc => uc.UserId == userId));

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchTermLike = $"%{searchTerm}%";

            baseQuery = baseQuery.Where(chat =>
                context.Set<UserChat>()
                    .Where(uc => uc.ChatId == chat.Id && uc.UserId != userId)
                    .Any(uc =>
                        EF.Functions.Like(uc.User.FirstName, searchTermLike) ||
                        EF.Functions.Like(uc.User.LastName, searchTermLike)
                    )
            );
        }

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = baseQuery
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(c => new Chat
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt,
                Type = c.Type,
                GroupPhoto = c.GroupPhoto,

                Messages = c.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(1)
                    .Select(m => new Message
                    {
                        Id = m.Id,
                        Description = m.Description,
                        CreatedAt = m.CreatedAt,
                        ChatId = m.ChatId,
                        SenderId = m.SenderId,
                        Sender = new User
                        {
                            Id = m.Sender.Id,
                            FirstName = m.Sender.FirstName,
                            LastName = m.Sender.LastName,
                            ProfilePhoto = m.Sender.ProfilePhoto
                        }
                    }).ToList(),

                UserChats = c.UserChats.Select(uc => new UserChat
                {
                    ChatId = uc.ChatId,
                    UserId = uc.UserId,
                    User = new User
                    {
                        Id = uc.User.Id,
                        FirstName = uc.User.FirstName,
                        LastName = uc.User.LastName,
                        ProfilePhoto = uc.User.ProfilePhoto
                    }
                }).ToList()
            });

        var chats = await query.ToListAsync(cancellationToken);

        return new PagedResult<Chat>(chats, total, page, size);
    }

    public async Task<bool> ChatExistsAsync(Guid chatId, CancellationToken cancellationToken) =>
        await ValidateAsync(c => c.Id == chatId, cancellationToken);

    public Task<Chat?> GetOneToOneChat(Guid firstUser, Guid secondUser, CancellationToken cancellationToken) =>
        context.Set<Chat>()
            .AsNoTracking()
            .Where(c => c.Type == ChatType.Private.ToString() &&
                        c.UserChats.Any(uc => uc.UserId == firstUser) &&
                        c.UserChats.Any(uc => uc.UserId == secondUser))
            .FirstOrDefaultAsync(cancellationToken);
}