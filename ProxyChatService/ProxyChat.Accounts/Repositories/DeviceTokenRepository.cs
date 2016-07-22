using ProxyChat.Accounts.Dtos;
using ProxyChat.Accounts.Mappers;
using ProxyChat.Accounts.Models;
using ProxyChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Accounts.Repositories
{
    public class DeviceTokenRepository : BaseRepository<AccountsContext, DeviceToken, DeviceTokenDto>, IDeviceTokenRepository<AccountsContext, DeviceToken, DeviceTokenDto>
    {
        protected override IResourceMapper<DeviceToken, DeviceTokenDto> Mapper
        {
            get
            {
                return new DeviceTokenMapper();
            }
        }

        public override IRepositoryResult<DeviceTokenDto> Create(DeviceTokenDto dto)
        {
            IRepositoryResult<DeviceTokenDto> result = new RepositoryResult<DeviceTokenDto>();

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

                var readResult = base.Read(dt => dt.DeviceId == dto.DeviceId);

                if (readResult.ResultCode == ResultCode.NotFound)
                {
                    result = base.Create(dto);
                }
                else
                {
                    using (var context = new AccountsContext())
                    {
                        var updateToken = context.Set<DeviceToken>().FirstOrDefault(dt => dt.DeviceId == dto.DeviceId);

                        updateToken.DeviceId = dto.DeviceId;
                        updateToken.ModifiedUTC = DateTime.UtcNow;
                        updateToken.Token = dto.Token;
                        updateToken.UserId = dto.UserId;

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                result.ResultData = null;
                result.ResultCode = ResultCode.Error;
                result.ResultDescription = e.Message;
            }

            return result;
        }

        protected override bool ValidateDto(DeviceTokenDto dto)
        {
            if (dto.DeviceId == 0) return false;
            if (dto.CreatedUTC == default(DateTime)) return false;
            if (string.IsNullOrWhiteSpace(dto.Token)) return false;
            if (dto.UserId == 0) return false;

            return true;
        }
    }
}
