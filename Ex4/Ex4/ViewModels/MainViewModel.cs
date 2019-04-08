using Ex4.Modele;
using Ex4.Views;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Plugin.Geolocator;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;
using Plugin.Permissions;
using System.Diagnostics;

namespace Ex4.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private string _titleLabel;
        public string TitleLabel{
            get => this._titleLabel;
            set => SetProperty(ref _titleLabel, value);
        }

        private ObservableCollection<PlaceModel> _listPlace;
        public ObservableCollection<PlaceModel> ListPlace {
            get => _listPlace;
            set => SetProperty(ref _listPlace, value);
        }

        public ICommand RefreshCommand { get; set; }
        private bool _isRefreshing = false;

        //Pour éviter que l'icone de refresh ne reste indéfiniment
        public bool IsRefreshing{
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }



        public Position MaLocation { get; set; }

        private PlaceModel _currentPlace;
        public PlaceModel CurrentPlace{
            get => _currentPlace;
            set{
                if (SetProperty(ref _currentPlace, value)){
                    OpenDetailPlace(_currentPlace);
                }
            }
        }




        public MainViewModel(){
            TitleLabel = "Liste des lieux";
            RefreshCommand = new Command(RefreshClicked);
        }

        private void RefreshClicked(){
            RefreshPlaces();
            IsRefreshing = false;
        }

        private async void RefreshPlaces(){
            await GetLocation();
            ListPlace = await RestService.Rest.LoadPlaces(MaLocation);
        }

        //Localisation
        private async Task<Plugin.Geolocator.Abstractions.Position> GetCurrentLocation(){
            Plugin.Geolocator.Abstractions.Position myPos = null;
            try{
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;
                myPos = await locator.GetLastKnownLocationAsync();

                if (myPos != null){
                    return myPos;
                }
                else{
                    Console.WriteLine("Erreur, la dernière position connue est nulle ! ");
                    return null;
                }
            }
            catch (Exception e){
                Console.WriteLine("Unable to get location : " + e.Message);
                return null;
            }
        }

        public async Task GetLocation(){
            Plugin.Geolocator.Abstractions.Position myPos = null;
            try{
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Location);

                if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted){
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Location)){
                        await Application.Current.MainPage.DisplayAlert("Géolocalisation demandée", "L'application à besoin de votre permission pour vous géolocaliser", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Location);
                    if (results.ContainsKey(Plugin.Permissions.Abstractions.Permission.Location)){
                        status = results[Plugin.Permissions.Abstractions.Permission.Location];
                    }
                }

                if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted){
                    myPos = await GetCurrentLocation();
                    if (myPos != null){
                        MaLocation = new Position((float)myPos.Latitude, (float)myPos.Longitude);
                    }
                    else{
                        // N'arrive pas à géolovcaliser
                        MaLocation = new Position(0, 0);
                    }
                }
                else{
                    await Application.Current.MainPage.DisplayAlert("Permissions non accordée", "L'application ne peut pas vous géolocaliser en raison d'une permission non accordée", "OK");
                }
            }
            catch (Exception e){
                Console.WriteLine("Erreur : " + e.Message);
            }
        }

        public override async Task OnResume()
        {
            await base.OnResume();

            await GetLocation();

            ListPlace = await RestService.Rest.LoadPlaces(MaLocation);
        }

        public async void OpenDetailPlace(PlaceModel place)
        {
            Debug.WriteLine("IIIIIIIIIIDDDDDDDDD");
            Debug.WriteLine(place.Id);
            await NavigationService.PushAsync<DetailPlace>(new Dictionary<string, object>() { { "PlaceId", place.Id } });
        }

    }
}
