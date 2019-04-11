using Newtonsoft.Json;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Ex4.Modele
{
	public class User : NotifierBase{
        private int? _idPicture;
        [JsonProperty(PropertyName = "image_id", NullValueHandling = NullValueHandling.Include)]
        public int? IdPicture{
            get{
                return _idPicture;
            }
            set{
                _idPicture = value;
                UpdatePicture();
            }
        }

        private ImageSource _imageSrc;
        public ImageSource ImageSrc
        {
            get => _imageSrc;
            set => SetProperty(ref _imageSrc, value);
        }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        

        public User(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public User(string email, string password, string firstName, string lastName){
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
        }

        [JsonConstructor]
        public User(int id, string first_name, string last_name, string email, int? image_id){
            Id = id;
            FirstName = first_name;
            LastName = last_name;
            Email = email;
            IdPicture = image_id;
        }

        private async void UpdatePicture(){
            if (_idPicture == null){
                ImageSrc = ImageSource.FromFile("no_pic.jpg");
            }
            else{
                byte[] stream = await RestService.Rest.LoadPicture(IdPicture);
                ImageSrc = ImageSource.FromStream(() => new MemoryStream(stream));
            }
        }
    }
}