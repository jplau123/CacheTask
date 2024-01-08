﻿using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPairRepository
    {
        public Task<int> DeleteOlderThan(DateTimeOffset timestamp);

        public Task<PairEntity?> Create(PairEntity pair);
        public Task<int> Update(PairEntity pair);
        public Task<PairEntity?> GetByKey(string key);
        public Task<int> Delete(string key);
        public Task<int> UpdateEpiresAt(string key, DateTimeOffset timestamp);
    }
}