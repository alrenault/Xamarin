using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Ex4.Modele
{
	public class Token{

        private static Token _token;
        public string Data { get; set; }

        private Token(){
        }


        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        private DateTime _expiresIn;
        [JsonProperty(PropertyName = "expires_in")]
        public string ExpiresIn{
            get{
                return _expiresIn.ToString();
            }
            set{
                double add = Convert.ToDouble(value);
                DateTime day = DateTime.Today.AddSeconds(add);
                _expiresIn = new DateTime(day.Year, day.Month, day.Day, day.Hour, day.Minute, day.Second);
            }
        }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        public static Token Ticket{
            get{
                if (_token == null){
                    _token = new Token();
                }
                return _token;
            }
            set => _token = value;
        }


        public static void Destroy(){
            _token = null;
        }

        public static bool IsInit(){
            if (_token == null){
                return false;
            }
            return true;
        }


        public static void RefreshIfNecessary(){
            if (DateTime.Today.AddMinutes(10) >= Ticket._expiresIn){
                Refresh();
                // TODO : Si jamais Refresh fail et _token == null --> déconnecter l'utilisateur
            }
        }

        public static async void Refresh(){
            await RestService.Rest.RefreshToken();
        }
    }
}