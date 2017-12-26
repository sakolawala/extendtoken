using icy2.idsvr.Endpoints.Results;
using icy2.idsvr.Extensions;
using icy2.idsvr.Utility;
using icy2.idsvr.Validation.Interface;
using IdentityServer4.Stores;
using Kiwi.Common.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace icy2.idsvr.Validation
{
    public class AdditonalAudienceValidator : IAdditonalAudienceValidator
    {
        private readonly IKiwiLogger _logger;
        private readonly IResourceStore _resources;

        public AdditonalAudienceValidator(IKiwiLogger logger, IResourceStore resources)
        {
            _logger = logger;
            _resources = resources;
        }

        public async Task<AdditonalAudienceValidationResult> ValidateAsync(HttpContext context)
        {
            _logger.LogDebug("Start AdditonalAudienceValidator Validation");
            AdditonalAudienceValidationResult result =
                            new AdditonalAudienceValidationResult();
            var body = await context.Request.ReadFormAsync();
            if (body != null)
            {
                string additonalAuds;
                if (!ValidateKeyPresence(body, out additonalAuds))
                {
                    result = new AdditonalAudienceValidationResult
                    {
                        IsError = true,
                        Error = "additionalauds_missing",
                        ErrorDescription = "Additional Audience Missing"
                    };
                }
                else
                {
                    result = await ValidateAudience(additonalAuds);                    
                }              
            }
            else
            {
                result = new AdditonalAudienceValidationResult
                {
                    IsError = true,
                    Error = "additionalauds_missing",
                    ErrorDescription = "Additional Audience Missing"
                };
            }
            return result;
            _logger.LogDebug("Completed AdditonalAudienceValidator Validation");
        }

        private bool ValidateKeyPresence(IFormCollection body, out string addauds)
        {
            addauds = body[ExtentTokenConstants.additionalAudience].FirstOrDefault();
            
            if (!addauds.IsPresent())
            {
                _logger.LogWarn("Additional Audience missing, AdditonalAudienceValidator validation failed");
                return false;
            }
            else
            {
                _logger.LogDebug($"Additional Audience '{addauds}' found");
                return true;
            }
                
        }

        private async  Task<AdditonalAudienceValidationResult> ValidateAudience(string addauds)
        {
            AdditonalAudienceValidationResult result =
                            new AdditonalAudienceValidationResult();
            List<string> additionalAudience = addauds.Split(",").Select(c => c.Trim()).ToList();
            foreach(var aud in additionalAudience)
            {
                //Find API in resource store.                 
                var api = await _resources.FindApiResourceAsync(aud);
                if (api == null)
                {
                    _logger.LogWarn($"No API resource with that name '{aud}' found.");
                    result = new AdditonalAudienceValidationResult
                    {
                        IsError = true,
                        Error = "invalid_audience",
                        ErrorDescription = "Invalid Audience Passed"
                    };
                    break;
                }
                else
                {
                    result.IsError = false;
                }
                    

                if (!api.Enabled)
                {
                    _logger.LogWarn($"API resource '{aud}' not enabled");
                    result = new AdditonalAudienceValidationResult
                    {
                        IsError = true,
                        Error = "invalid_audience",
                        ErrorDescription = "Audience not enabled"
                    };
                    break;
                }
                else
                {
                    result.IsError = false;
                }

                //API secret aren't set in KIWI, hence not validated.
            }

            //Is all apis are valid
            if (!result.IsError)
            {
                result = new AdditonalAudienceValidationResult
                {
                    IsError = false,
                    AdditionAudieces = additionalAudience
                };
            }
            //return
            return result;
        }
    }

}
