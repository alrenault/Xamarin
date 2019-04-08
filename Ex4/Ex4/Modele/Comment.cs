using Newtonsoft.Json;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Ex4.Modele
{
	public class Comment : NotifierBase
    {
        private string _date;
        [JsonProperty(PropertyName = "date")]
        public string Date{
            get {
                String[] myDate = Regex.Split(_date, "-");
                return myDate[2].Substring(0, 2) + "/" + myDate[1] + "/" + myDate[0];
            }
            set {
                _date = value;
            }
        }

        [JsonProperty(PropertyName = "author")]
        public User Author { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        public string CompletName{
            get => Author.FirstName + " " + Author.LastName;
        }

    }
}