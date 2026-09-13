using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTrackerApi.Controllers;

[ApiController]
[Authorize] // 👈 Protège TOUS les contrôleurs qui héritent de celui-ci
public abstract class BaseApiController : ControllerBase
{
}