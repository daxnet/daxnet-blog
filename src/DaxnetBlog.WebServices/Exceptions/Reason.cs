using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.WebServices.Exceptions
{
    public enum Reason
    {
        CreateFailed,
        UpdateFailed,
        DeleteFailed,
        ArgumentNull,
        AlreadyExists,
        EntityNotFound,
        InvalidArgument
    }
}
