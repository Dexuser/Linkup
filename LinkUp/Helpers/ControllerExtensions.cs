using LinkUp.Core.Application;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Helpers;

public static class ControllerExtensions
{
    public static void SendValidationErrorMessages(this Controller controller, Result serviceResult)
    {
        // Este metodo solamente se usa en caso de que el Result haya fallado, es decir, isFailure
        if (!string.IsNullOrEmpty(serviceResult.GeneralError))
        {
            controller.ViewBag.Message = serviceResult.GeneralError;
        }
        else
        {
            controller.ViewBag.Message = serviceResult.Errors;
        }
    }
}