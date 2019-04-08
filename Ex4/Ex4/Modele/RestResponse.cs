using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Ex4.Modele
{
    public class RestResponse
    {
        [JsonProperty(PropertyName = "is_success")]
        public string IsSuccess { get; set; }

        [JsonProperty(PropertyName = "error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "error_message")]
        public string ErrorMessage { get; set; }
    }

    public class RestResponse<T> : RestResponse
    {
        [JsonProperty(PropertyName = "data", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(null)]
        public T Data { get; set; }
    }
}