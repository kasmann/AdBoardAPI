using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace AdBoardAPI.Options.Validation
{
    public class SettingValidationStartupFilter : IStartupFilter
    {
        private readonly IEnumerable<IValidatable> _validatableObjects;
        private readonly ILogger<SettingValidationStartupFilter> _logger;
        public SettingValidationStartupFilter(IEnumerable<IValidatable> validatableObjects, ILogger<SettingValidationStartupFilter> logger)
        {
            _logger = logger;
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
                    _logger.LogError(ex.Message);
                    validated = false;
                }
            }

            if (!validated) throw new ValidationException("Валидация конфигурации не пройдена");

            return next;
        }
    }
}
