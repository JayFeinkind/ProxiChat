using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyChat.Accounts.Models;
using ProxyChat.Domain;
using System.Data.Entity;
using ProxyChat.Accounts.Dtos;
using ProxyChat.Accounts.Mappers;

namespace ProxyChat.Accounts.Repositories
{
    public class UserRepository : BaseRepository<AccountsContext, User, UserDto>, IUserRepository<AccountsContext, User, UserDto>
    {
        protected override IResourceMapper<User, UserDto> Mapper
        {
            get
            {
                return new UserMapper();
            }
        }

        protected override bool ValidateDto(UserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName)) return false;
            if (string.IsNullOrWhiteSpace(dto.LastName)) return false;
            if (string.IsNullOrWhiteSpace(dto.EmailAddress)) return false;
            if (string.IsNullOrWhiteSpace(dto.UserName)) return false;

            return true;
        }

        public override async Task<IRepositoryResult<UserDto>> Create(UserDto dto)
        {
            IRepositoryResult<UserDto> result = new RepositoryResult<UserDto>();
            
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                if (!ValidateDto(dto))
                {
                    result.ResultData = null;
                    result.ResultCode = ResultCode.InvalidData;
                    result.ResultDescription = "Missing required data";
                    return result;
                }

                using (var context = new AccountsContext())
                {
                    IQueryable<User> users = context.Set<User>().AsNoTracking().AsQueryable();

                    if (await users.AnyAsync(u => u.UserName.Trim().ToLower() == dto.UserName.Trim().ToLower()))
                    {
                        result.ResultData = null;
                        result.ResultCode = ResultCode.DataConflict;
                        result.ResultDescription = "UserName already exists";

                        return result;
                    }

                    if (await users.AnyAsync(u => u.EmailAddress.Trim().ToLower() == dto.EmailAddress.Trim().ToLower()))
                    {
                        result.ResultData = null;
                        result.ResultCode = ResultCode.DataConflict;
                        result.ResultDescription = "EmailAddress already exists";

                        return result;
                    }
                }

                result = await base.Create(dto);
            }
            catch (Exception e)
            {
                result.ResultData = null;
                result.ResultCode = ResultCode.Error;
                result.ResultDescription = e.Message;
            }

            return result;
        }

        protected override void UpdateEntityProperties(User from, User to)
        {
            to.DeviceTokens = from.DeviceTokens;
            to.EmailAddress = from.EmailAddress;
            to.FirstName = from.FirstName;

            to.LastName = from.LastName;
            to.ModifiedUTC = DateTime.UtcNow;
            to.ResetPasswordToken = from.ResetPasswordToken;
            to.UserName = from.UserName;
        }
    }
}
