using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QNE.Models.ViewModel;
using System.Collections.Generic;

namespace QNE.App.Server.Controllers
{
    internal static class ControllerExtensions
    {
        /// <summary>
        /// Used to handle access with a token which belongs to different tenant
        /// </summary>
        /// <returns>The denied.</returns>
        /// <param name="controller">Controller.</param>
        internal static IActionResult Denied(this ControllerBase controller)
        {
            return Error(controller, "PERMISSION_DENIED", "Permission denied.");
        }

        /// <summary>
        /// Handle object not found response
        /// </summary>
        /// <returns>The not found.</returns>
        /// <param name="controller">Controller.</param>
        /// <param name="message">Error message</param>
        internal static IActionResult NotExists(this ControllerBase controller, string message = null)
        {
            string msg = message;
            if (string.IsNullOrEmpty(msg))
                msg = "Object does not exist.";
            return Error(controller, "OBJ_NOT_FOUND", msg);
        }

        internal static IActionResult Duplicate(this ControllerBase controller, string message)
        {
            return Error(controller, "DUPLICATE_CODE", message);
        }

        internal static IActionResult InUse(this ControllerBase controller, string message = null, object data = null)
        {
            var msg = message;
            if (string.IsNullOrEmpty(msg))
                msg = "Object is being used elsewhere.";
            return Error(controller, "OBJ_IN_USE", msg, data);
        }

        internal static IActionResult ModelError(this ControllerBase controller)
        {
            var state = controller.ModelState;

            if (!state.IsValid)
            {
                var errorMsg = "";
                var errors = new Dictionary<string, List<string>>();
                foreach (var propertyError in state)
                {
                    if (!errors.ContainsKey(propertyError.Key))
                        errors[propertyError.Key] = new List<string>();
                    foreach (var error in propertyError.Value.Errors)
                    {
                        errors[propertyError.Key].Add(error.ErrorMessage);
                        errorMsg += error.ErrorMessage + "\r\n";
                    }
                }

                return Error(controller, "VALIDATION_ERROR", errorMsg.Trim(), errors);
            }

            return Error(controller, "INVALID_REQUEST", "Unknown request.");
        }

        internal static IActionResult Error(this ControllerBase controller, string code, string message = null, object data = null)
        {
            return controller.BadRequest(new ApiResponseMessage
            {
                Code = code,
                Message = message ?? "Operation was failed with error(s).",
                Data = data
            });
        }

        internal static IActionResult Success(this ControllerBase controller, string message = null, object data = null)
        {
            var result = new ApiResponseMessage
            {
                Code = "0000",
                Message = message ?? "Operation was completed successfully.",
                Data = data
            };
            return controller.Ok(result);
        }

        internal static IActionResult IdentityError(this ControllerBase controller, IdentityResult result, string message)
        {
            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        controller.ModelState.AddModelError(error.Code, error.Description);
                    }

                    return controller.ModelError();
                }
            }


            return controller.Error("IDENTITY_ERROR", message);
        }
    }
}