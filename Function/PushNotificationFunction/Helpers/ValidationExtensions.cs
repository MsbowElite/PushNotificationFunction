﻿using PushNotificationFunction.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PushNotificationFunction.Helpers
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Creates a <see cref="BadRequestObjectResult"/> containing a collection
        /// of minimal validation error details.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static BadRequestObjectResult ToBadRequest<T>(this ValidatableRequest<T> request)
        {
            return new BadRequestObjectResult(request.Errors.Select(e => new {
                Field = e.PropertyName,
                Error = e.ErrorMessage
            }));
        }
    }
}
