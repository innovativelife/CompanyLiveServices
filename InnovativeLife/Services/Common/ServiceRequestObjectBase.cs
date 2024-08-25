using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace InnovativeLife.Services.Common;

public class ServiceRequestObjectBase
{

    public List<string> Validate()
    {
        var result = new List<string>();
        var context = new ValidationContext(this, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(this, context, validationResults);
        result.AddRange(from item in validationResults
                        select item.ErrorMessage ?? "Validation error");
        return result;
    }
}