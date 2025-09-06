using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Application.Interfaces.Validator
{
    public interface IJsonValidator
    {
        bool Validate<T>(T requestPayload, string jsonRule, out List<string> errors);
        bool ValidateJsonPayload<T>(T requestPayload, string jsonRule, out List<string> errors);
    }
}
