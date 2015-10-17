﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficAppAPI.DAL.Contracts;
using TrafficAppAPI.Model;
using TrafficAppAPI.Model.Dto;

namespace TrafficAppAPI.Repository.Contracts
{
    public interface IShoutRepository : IRepository<Shout>
    {
        Task<Shout> AddShout(Shout shout);
        Task<List<Shout>> GetShouts(int? skip, int? limit, string sort);
        Task<Shout> GetShoutById(string shoutId);
        Task<List<Comment>> GetShoutComments(string shoutId, int skip, int limit);
        Task<bool> AddShoutComment(string shoutId, Comment comment);
    }
}
