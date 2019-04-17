using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ex4.Modele
{
	public static class MyGeolocator{
        private static async Task<Plugin.Geolocator.Abstractions.Position> GetCurrentLocation(){
            Plugin.Geolocator.Abstractions.Position myPosition = null;
            try{
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;
                myPosition = await locator.GetLastKnownLocationAsync();

                if (myPosition != null){
                    return myPosition;
                }
                else{
                    Console.WriteLine("Erreur, la dernière position est inconnue ! ");
                    return null;
                }
            }
            catch (Exception e){
                Console.WriteLine("Unable to get location : " + e.Message);
                return null;
            }
        }

        public static async Task<Position> GetLocation(){
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
                        return new Position((float)myPos.Latitude, (float)myPos.Longitude);
                    }
                    else{
                        // Pb avec la geolocalisation ...
                        await Application.Current.MainPage.DisplayAlert("Permissions non accordée", "L'application ne peut pas vous géolocaliser en raison de la non activation de cette fonctionnalité.", "OK");
                        return new Position(0, 0);
                    }
                }
                else{
                    await Application.Current.MainPage.DisplayAlert("Permissions non accordée", "L'application ne peut pas vous géolocaliser en raison d'une permission non accordée", "OK");
                    return new Position(0, 0);
                }
            }
            catch (Exception e){
                Console.WriteLine("Erreur : " + e.Message);
                return new Position(0, 0);
            }
        }
    }
}