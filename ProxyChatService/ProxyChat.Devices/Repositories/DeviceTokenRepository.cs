using ProxyChat.Devices.Dtos;
using ProxyChat.Devices.Mappers;
using ProxyChat.Devices.Models;
using ProxyChat.Domain;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyChat.Devices.Repositories
{
    public class DeviceTokenRepository : BaseRepository<DevicesContext, DeviceToken, DeviceTokenDto>, IDeviceTokenRepository<DevicesContext, DeviceToken, DeviceTokenDto>
    {
        DeviceTokenMapper _mapper = new DeviceTokenMapper();

        protected override IResourceMapper<DeviceToken, DeviceTokenDto> Mapper
        {
            get
            {
                return _mapper;
            }
        }

        public override async Task<IRepositoryResult<DeviceTokenDto>> Create(DeviceTokenDto dto)
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

                var readResult = await base.Read(dt => dt.DeviceId == dto.DeviceId);

                if (readResult.ResultCode == ResultCode.NotFound)
                {
                    result = await base.Create(dto);
                }
                else
                {
                    using (var context = new DevicesContext())
                    {
                        var updateToken =  await context.Set<DeviceToken>().FirstOrDefaultAsync(dt => dt.DeviceId == dto.DeviceId);

                        updateToken.DeviceId = dto.DeviceId;
                        updateToken.ModifiedUTC = DateTime.UtcNow;
                        updateToken.Token = dto.Token;
                        updateToken.UserId = dto.UserId;

                        await context.SaveChangesAsync();

                        result.ResultCode = ResultCode.Ok;
                        result.ResultData = Mapper.MapEntityToDto(updateToken);
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

        protected override void UpdateEntityProperties(DeviceToken from, DeviceToken to)
        {
            to.Device = from.Device;
            to.DeviceId = from.DeviceId;
            to.ModifiedUTC = DateTime.UtcNow;
            to.Token = from.Token;
            to.User = from.User;
            to.UserId = from.UserId;
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
