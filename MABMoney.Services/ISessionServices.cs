﻿using MABMoney.Services.DTO;
using System;
using System.Collections.Generic;

namespace MABMoney.Services
{
    public interface ISessionServices
    {
        IEnumerable<SessionDTO> All();
        SessionDTO Get(int id);
        SessionDTO GetByKey(string key);
        void Save(SessionDTO dto);
        void Delete(int id);
        void DeleteByKey(string key);
        void DeleteExpired();
        void UpdateSessionExpiry(string key, DateTime expiry);
    }
}
