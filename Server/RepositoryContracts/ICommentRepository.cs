﻿using Entities;

namespace RepositoryContracts;

public interface ICommentRepository
{
    Task<Comment> AddAsync(Comment comment);
    Task<Comment> UpdateAsync(Comment comment);
    Task<Comment> DeleteAsync(int id);
    Task<Comment> GetSingleAsync(int id);
    IQueryable<Comment> GetManyAsync();
}