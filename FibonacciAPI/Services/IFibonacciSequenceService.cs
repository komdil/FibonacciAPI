﻿using FibonacciAPI.Queries;
using FibonacciAPI.Responses;

namespace FibonacciAPI.Services
{
    public interface IFibonacciSequenceService
    {
        Task<ServerResponse<List<int>>> GetSubsequence(GetSubsequenceQuery query);
    }
}