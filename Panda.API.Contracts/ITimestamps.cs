using System;

namespace Panda.API.Contracts
{
    public interface ITimestamps
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}
