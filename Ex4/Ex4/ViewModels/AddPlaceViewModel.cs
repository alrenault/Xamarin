using Ex4.Modele;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Ex4.ViewModels
{
	public class AddPlaceViewModel : ViewModelBase{
        public string TitleLabel { get; set; }

        private string _namePlace;
        public string NamePlace{
            get => _namePlace;
            set => SetProperty(ref _namePlace, value);
        }

        private string _descriptionPlace;
        public string DescriptionPlace{
            get => _descriptionPlace;
            set => SetProperty(ref _descriptionPlace, value);
        }

        private string _latitudePlace;
        public string LatitudePlace{
            get => _latitudePlace;
            set => SetProperty(ref _latitudePlace, value);
        }

        private string _longitudePlace;
        public string LongitudePlace{
            get => _longitudePlace;
            set => SetProperty(ref _longitudePlace, value);
        }

        private MediaFile _image;

        private ImageSource _imageSource;
        public ImageSource ImageSource{
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public Command NewPhoto { get; private set; }
        public Command NewImage { get; private set; }
        public Command GetPosition { get; private set; }
        public Command AddPlace { get; private set; }



        public AddPlaceViewModel(){
            TitleLabel = "Ajout d'un lieu";
            NewPhoto = new Command(PhotoCommand);
            NewImage = new Command(ImageCommand);
            GetPosition = new Command(PositionCommand);
            AddPlace = new Command(AddCommand);

            UpdatePicture();
        }





        private void UpdatePicture(){
            if (_image == null){
                ImageSource = ImageSource.FromFile("no_pic.jpg");
            }
            else{
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = _image.GetStream();
                    return stream;
                });
            }
        }

        private async void PhotoCommand(){
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsCameraAvailable || CrossMedia.Current.IsTakePhotoSupported){
                // Supply media options for saving our photo after it's taken.
                var mediaOptions = new StoreCameraMediaOptions{
                    Name = DateTime.Now.ToShortTimeString() + ".jpg",
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 4096,
                    CompressionQuality = 75,
                    AllowCropping = true
                };

                // Take a photo of the business receipt.
                var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);

                if (file != null){
                    _image = file;
                    UpdatePicture();
                }
            }
            else{
                await App.Current.MainPage.DisplayAlert("No Camera", " Aucun appareil photo disponible.", "OK");
            }
        }

        private async void ImageCommand(){
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsPickPhotoSupported){
                var mediaOptions = new PickMediaOptions{
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 4096,
                    CompressionQuality = 75
                };

                var file = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

                if (file != null){
                    _image = file;
                    UpdatePicture();
                }
            }
            else{
                await App.Current.MainPage.DisplayAlert("Photo picking unsupported", "Erreur", "Continue");
            }
        }


        private async void PositionCommand(){
            var res = await MyGeolocator.GetLocation();
            LatitudePlace = res.Latitude.ToString("G", CultureInfo.InvariantCulture);
            LongitudePlace = res.Longitude.ToString("G", CultureInfo.InvariantCulture);
        }


        private async void AddCommand(){
            if (_image != null)
            {
                if (!string.IsNullOrWhiteSpace(NamePlace) && !string.IsNullOrWhiteSpace(DescriptionPlace)){
                    if (!string.IsNullOrWhiteSpace(LatitudePlace) && !string.IsNullOrWhiteSpace(LongitudePlace)){
                        string pattern = @"^[\-]?\d+(\.\d+)*$";

                        Regex rgx = new Regex(pattern);

                        if (rgx.IsMatch(LatitudePlace) && rgx.IsMatch(LongitudePlace)){
                            MemoryStream memoryStream = new MemoryStream();
                            _image.GetStream().CopyTo(memoryStream);
                            byte[] pictureArray = memoryStream.ToArray();

                            (Boolean test, string infos) = await RestService.Rest.AddPlace(NamePlace, DescriptionPlace, pictureArray, LatitudePlace, LongitudePlace);

                            if (test){
                                await App.Current.MainPage.DisplayAlert("Ajout d'une place", infos, "ok");
                                await NavigationService.PopAsync();
                            }
                            else{
                                await App.Current.MainPage.DisplayAlert("Ajout d'une place", infos, "ok");
                            }
                        }
                        else{
                            await App.Current.MainPage.DisplayAlert("Ajout d'une place", "La lattitude et la longitude doivent être de la forme ^[\\-]?\\d+(\\.\\d+)*$ .", "ok");
                        }
                    }
                    else{
                        await App.Current.MainPage.DisplayAlert("Ajout d'une place", "Vous devez préciser une position pour votre lieu.", "ok");
                    }
                }
                else{
                    await App.Current.MainPage.DisplayAlert("Ajout d'une place", "Vous devez préciser un nom et une description.", "ok");
                }
            }
            else{
                await App.Current.MainPage.DisplayAlert("Ajout d'une place", "Vous devez selectionner une image.", "ok");
            }
        }
    }

}