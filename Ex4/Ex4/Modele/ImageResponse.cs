using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Ex4.Modele
{
    public class ImageResponse
    {
        [JsonProperty(PropertyName = "id")]
        public int IdNewImage { get; set; }
    }
}