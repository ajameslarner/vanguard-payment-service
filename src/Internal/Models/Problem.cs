using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Internal.Models;

internal class Problem : ProblemDetails
{
    public override string ToString()
        => JsonSerializer.Serialize(this);
}
