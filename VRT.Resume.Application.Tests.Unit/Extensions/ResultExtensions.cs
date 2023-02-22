﻿using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Xunit;

namespace VRT.Resume.Application
{
    internal static class ResultExtensions
    {
        public static async Task<Result> AssertFailure(this Task<Result> result)
        {
            var r = await result;
            return r.AssertFailure();
        }
        public static Result AssertFailure(this Result result)
        {
            Assert.True(result.IsFailure, "Expected failure");
            return result;
        }

        internal static async Task<Result> AssertFail(this Task<Result> result)
        {
            var r = await result;
            return r.AssertFail();
        }
        internal static Result AssertFail(this Result result)
        {
            Assert.True(result.IsFailure, "Expected failure");
            return result;
        }


        internal static async Task<Result> AssertSuccess<T>(this Task<Result<T>> result)
        {
            var r = await result;
            return r.AssertSuccess();
        }

        internal static Result AssertSuccess<T>(this Result<T> result)
        {
            Assert.True(result.IsSuccess, GetErrorSafe(result));
            return result;
        }

        internal static async Task<Result> AssertSuccess(this Task<Result> result)
        {
            var r = await result;
            return r.AssertSuccess();                        
        }

        internal static Result AssertSuccess(this Result result)
        {
            Assert.True(result.IsSuccess, GetErrorSafe(result));
            return result;
        }

        internal static string GetErrorSafe(this Result result)
        {
            return result.IsSuccess ? "" : result.Error;
        }
        internal static string GetErrorSafe<T>(this Result<T> result)
        {
            return result.IsSuccess ? "" : result.Error;
        }
    }
}
