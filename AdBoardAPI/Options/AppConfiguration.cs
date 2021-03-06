﻿using System.ComponentModel.DataAnnotations;
using AdBoardAPI.Options.Validation;

namespace AdBoardAPI.Options
{
    public class AppConfiguration : IValidatable
    {
        [Required(ErrorMessage = "Раздел SystemOptions конфигурации приложения не заполнен")]
        public SystemOptions SystemOptions { get; set; }
        public PublishOptions PublishOptions { get; set; }

        [Required(ErrorMessage = "Раздел CacheOptions конфигурации приложения не заполнен")]
        public CacheOptions CacheOptions { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
            Validator.ValidateObject(PublishOptions, new ValidationContext(PublishOptions), validateAllProperties: true);
            Validator.ValidateObject(SystemOptions, new ValidationContext(SystemOptions), validateAllProperties: true);
            Validator.ValidateObject(CacheOptions, new ValidationContext(CacheOptions), validateAllProperties: true);
        }
    }
}
