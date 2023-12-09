﻿using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;

namespace Grimsby_and_Clee_Sells.Repositories
{
    public class SQLUserRepository: IUserRepository
    {
        private readonly GacsDbContext _context;

        public SQLUserRepository(GacsDbContext context)
        {
            this._context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Tbl_Users.ToList();
        }
    }
}
