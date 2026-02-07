using Application.Interfaces;
using HashidsNet;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Common.Services
{
    public class HashIdService : IHashIdService
    {
        private readonly Hashids _hashids;

        public HashIdService(IConfiguration config)
        {
            var salt = config["HashIds:Salt"] ?? "Library-System-2026-Salt";
            _hashids = new Hashids(salt, minHashLength: 8);
        }

        public string Encode(int id) => _hashids.Encode(id);
        public int Decode(string hash) => _hashids.DecodeSingle(hash);
    }
}
