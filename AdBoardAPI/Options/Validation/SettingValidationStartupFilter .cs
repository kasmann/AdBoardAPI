using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdBoardAPI
{
    public class SettingValidationStartupFilter : IStartupFilter
    {
        readonly IEnumerable<IValidatable> _validatableObjects;
        public SettingValidationStartupFilter(IEnumerable<IValidatable> validatableObjects)
        {
            _validatableObjects = validatableObjects;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            var validated = true;
            foreach (var validatableObject in _validatableObjects)
            {
                try
                {
                    validatableObject.Validate();
                }
                catch (ValidationException ex)
                {
                    Console.WriteLine(ex.Message);
                    validated = false;
                }
            }

            if (!validated) throw new ValidationException("Валидация конфигурации не пройдена");
            return next;
        }
    }
}
