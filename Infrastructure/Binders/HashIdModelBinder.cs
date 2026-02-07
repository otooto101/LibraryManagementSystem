using Application.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Infrastructure.Binders
{
    public class HashIdModelBinder(IHashIdService hashIdService) : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

            if (string.IsNullOrEmpty(value)) return Task.CompletedTask;

            try
            {
                var decoded = hashIdService.Decode(value);
                bindingContext.Result = ModelBindingResult.Success(decoded);
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid ID format.");
            }

            return Task.CompletedTask;
        }
    }
}
