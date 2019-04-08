using Newtonsoft.Json;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Ex4.Modele
{
    public class PlaceModel : NotifierBase{
        //id de l'image (peut etre null)
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

        //id de la place
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        //titre de la place
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        //image de la place
        private ImageSource _imageSrc;
        public ImageSource ImageSrc{
            get => _imageSrc;
            set => SetProperty(ref _imageSrc, value);
        }

        //commentaires de la place
        [JsonProperty(PropertyName = "comments")]
        public ObservableCollection<Comment> Comments { get; set; }

        //description de la place
        [JsonProperty(PropertyName = "description")]
        public string Desc { get; set; }

        //latitude, longitude et position de la place
        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }

        public Position Position{
            get => new Position(Latitude, Longitude);
        }

        private string _txtDist;
        public string TxtDist{
            get => _txtDist;
            set => SetProperty(ref _txtDist, value);
        }

        private int _distance;
        public int Distance{
            get{
                return _distance;
            }
            set{
                SetProperty(ref _distance, value);
                TxtDist = "Vous êtes à " + _distance + " km de cet endroit.";
            }
        }

        public static int Comparaison(PlaceModel p1, PlaceModel p2){
            if (p1.Distance == p2.Distance){
                return 0;
            }
            if (p1.Distance > p2.Distance){
                return 1;
            }
            return -1;
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
