using ProxyChat.Accounts.Dtos;
using ProxyChat.Accounts.Mappers;
using ProxyChat.Accounts.Models;
using ProxyChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Accounts.Repositories
{
    public class MembershipRepository : BaseRepository<AccountsContext, Membership, MembershipDto>, IMembershipRepository<AccountsContext, Membership, MembershipDto>
    {
        MembershipMapper _mapper = new MembershipMapper();

        const int SALT_SIZE = 8;

        protected override IResourceMapper<Membership, MembershipDto> Mapper
        {
            get
            {
                return _mapper;
            }
        }

        protected override bool ValidateDto(MembershipDto dto)
        {
            if (dto.CreatedUTC == default(DateTime)) return false;
            if (dto.HashedPassword == default(byte[])) return false;
            if (dto.Salt == default(byte[])) return false;
            if (dto.Id == 0) return false;

            return true;
        }

        public override async Task<IRepositoryResult<MembershipDto>> Create(MembershipDto dto)
        {
            // currently salt column in DB has 8 bytes
            var salt = CreateSalt(SALT_SIZE);
            var hashedPassword = Hash(dto.TextPassword, salt);

            dto.Salt = salt;
            dto.HashedPassword = hashedPassword;

            return await base.Create(dto);
        }

        public bool ValidatePassword(int userId, string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (userId == 0) return false;

            using (var context = new AccountsContext())
            {
                // membership primary key is the userId.  No actual FK relation exists.
                var membership = context.Set<Membership>().AsNoTracking().FirstOrDefault(m => m.Id == userId);

                if (membership == null) return false;

                var hashedPassword = Hash(password, membership.Salt);

                return hashedPassword.SequenceEqual(membership.Password);
            }
        }

        private byte[] CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return buff;
        }

        private byte[] Hash(string value, byte[] salt)
        {
            return Hash(Encoding.UTF8.GetBytes(value), salt);
        }

        private byte[] Hash(byte[] value, byte[] salt)
        {
            byte[] saltedValue = value.Concat(salt).ToArray();

            return new SHA256Managed().ComputeHash(saltedValue);
        }

        protected override void UpdateEntityProperties(Membership from, Membership to)
        {
            to.Id = from.Id;
            to.Password = from.Password;
            to.Salt = from.Salt;
        }
    }
}
