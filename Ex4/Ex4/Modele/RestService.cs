using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Ex4.Modele
{
	public class RestService
	{
        private HttpClient client;
  
        private static RestService _rest;
        public static RestService Rest{
            get{
                if (_rest == null){
                    _rest = new RestService();
                }
                return _rest;
            }
        }

        private PlaceModel place;



        //Tools ---------------------------------
        private int GetDistanceBetweenPositions(Position src, Position dest){
            int R = 6378;

            double SourceLat = GetRadian(src.Latitude);
            double SourceLong = GetRadian(src.Longitude);
            double DestLat = GetRadian(dest.Latitude);
            double DestLong = GetRadian(dest.Longitude);

            return (int)(R * (Math.PI / 2 - Math.Asin(Math.Sin(DestLat) * Math.Sin(SourceLat) + Math.Cos(DestLong - SourceLong) * Math.Cos(DestLat) * Math.Cos(SourceLat))));
        }

        private double GetRadian(double degree){
            return Math.PI * degree / 180;
        }
        //-----------------------------------------



        private RestService (){
            client = new HttpClient();

            //Si en commentaire : incrément auto
            //this.client.MaxResponseContentBufferSize = 256000;
        }



        //Load all places
        public async Task<ObservableCollection<PlaceModel>> LoadPlaces(Position currentLocation){
            ObservableCollection<PlaceModel> _places = new ObservableCollection<PlaceModel>();

            string RestUrl = "https://td-api.julienmialon.com/places";
            var uri = new Uri(string.Format(RestUrl, string.Empty));

            try{
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode){
                    var json = await response.Content.ReadAsStringAsync();
                    RestResponse<ObservableCollection<PlaceModel>> restResponse = JsonConvert.DeserializeObject<RestResponse<ObservableCollection<PlaceModel>>>(json);

                    List<PlaceModel> ListOrder = new List<PlaceModel>();

                    foreach (PlaceModel p in restResponse.Data){
                        p.Distance = GetDistanceBetweenPositions(p.Position, currentLocation);
                        ListOrder.Add(p);
                    }
                    ListOrder.Sort(PlaceModel.Comparaison);
                    _places = new ObservableCollection<PlaceModel>(ListOrder);
                }
            }
            catch (Exception e){
                Debug.WriteLine(e.Message);
            }
            return _places;
        }


        //Load pictures
        public async Task<byte[]> LoadPicture(int? idPicture){
            byte[] stream = null;
            string RestUrl = "https://td-api.julienmialon.com/images/" + idPicture;
            var uri = new Uri(string.Format(RestUrl, string.Empty));

            try{
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode){
                    stream = await response.Content.ReadAsByteArrayAsync();
                }
                else{
                    Debug.WriteLine("Probleme lors du chargement d'une image");
                }
            }
            catch (Exception e){
                Debug.WriteLine(e.Message);
            }

            return stream;
        }


        //Load a single place
        public async Task<PlaceModel> LoadPlace(long idPlace){
            string RestUrl = "https://td-api.julienmialon.com/places/" + idPlace;
            var uri = new Uri(string.Format(RestUrl, string.Empty));

            try{
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode){
                    var json = await response.Content.ReadAsStringAsync();
                    RestResponse<PlaceModel> restResponse = JsonConvert.DeserializeObject<RestResponse<PlaceModel>>(json);

                    if ("true".Equals(restResponse.IsSuccess)){
                        place = restResponse.Data;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return place;
        }



        //Log in
        public async Task LogIn(string email, string password){
            User user = new User(email, password);

            string RestUrl = "https://td-api.julienmialon.com/auth/login";
            var uri = new Uri(string.Format(RestUrl, string.Empty));

            var stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            try{
                var response = await client.PostAsync(uri, stringContent);

                if (response.IsSuccessStatusCode){
                    var json = await response.Content.ReadAsStringAsync();
                    RestResponse<Token> restResponse = JsonConvert.DeserializeObject<RestResponse<Token>>(json);

                    if ("true".Equals(restResponse.IsSuccess)){
                        Token.Ticket = restResponse.Data;
                    }
                    else{
                        // TODO
                        Token.Destroy();
                    }
                }
                else{
                    Token.Destroy();
                }
            }
            catch (Exception e){
                Debug.WriteLine(e.Message);
            }
        }

        

        //Refresh The token
        public async Task RefreshToken(){
            string RestUrl = "https://td-api.julienmialon.com/auth/refresh";
            var uri = new Uri(string.Format(RestUrl, string.Empty));

            var stringContent = new StringContent("{ \"refresh_token\": \"" + Token.Ticket.RefreshToken + "\"}", Encoding.UTF8, "application/json");

            try{
                var response = await client.PostAsync(uri, stringContent);

                if (response.IsSuccessStatusCode){
                    var json = await response.Content.ReadAsStringAsync();
                    RestResponse<Token> restResponse = JsonConvert.DeserializeObject<RestResponse<Token>>(json);

                    if ("true".Equals(restResponse.IsSuccess)){
                        Token.Ticket = restResponse.Data;
                    }
                    else{
                        // TODO
                        Token.Destroy();
                    }
                }
                else{
                    Token.Destroy();
                }
            }
            catch (Exception e){
                Debug.WriteLine(e.Message);
            }
        }



        //Add a comment to a place
        public async Task<(Boolean, string)> AddComment(long idPlace, string texte){
            Token.RefreshIfNecessary();

            string RestUrl = "https://td-api.julienmialon.com/places/" + idPlace + "/comments";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, RestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue(Token.Ticket.TokenType, Token.Ticket.AccessToken);
            request.Content = new StringContent("{ \"text\": \"" + texte + "\"}", Encoding.UTF8, "application/json");

            try{
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode){
                    var json = await response.Content.ReadAsStringAsync();
                    RestResponse restResponse = JsonConvert.DeserializeObject<RestResponse>(json);

                    if ("true".Equals(restResponse.IsSuccess)){
                        return (true, "Commentaire ajouté.");
                    }
                    else{
                        return (false, "Votre commentaire n'a pas pû être ajouté.");
                    }
                }
            }
            catch (Exception e){
                Debug.WriteLine(e.Message);
            }
            return (false, "Erreur lors de la tentative d'ajout du commentaire. Veuillez réessayer.");
        }

        public async Task<(Boolean, string)> Register(string email, string password, string firstName, string lastName){
            User user = new User(email, password, firstName, lastName);

            string RestUrl = "https://td-api.julienmialon.com/auth/register";
            var uri = new Uri(string.Format(RestUrl, string.Empty));

            var stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            try{
                var response = await client.PostAsync(uri, stringContent);

                if (response.IsSuccessStatusCode){
                    var json = await response.Content.ReadAsStringAsync();
                    RestResponse<Token> restResponse = JsonConvert.DeserializeObject<RestResponse<Token>>(json);

                    if ("true".Equals(restResponse.IsSuccess)){
                        Token.Ticket = restResponse.Data;
                        return (true, "Votre compte a été crée avec succès !");
                    }
                    else{
                        return (false, "L'adresse email demandée est déjà utilisée. Veuillez en utiliser une autre ou vous connecter.");
                    }
                }
                else{
                    Token.Destroy();
                }
            }
            catch (Exception e){
                Debug.WriteLine(e.Message);
            }
            return (false, "Erreur lors de la tentative de création du compte. Veuillez réessayer.");
        }

        //Get data of an user
        public async Task<(Boolean, User)> GetUserData(){
            Token.RefreshIfNecessary();

            string RestUrl = "https://td-api.julienmialon.com/me";
            var uri = new Uri(string.Format(RestUrl, string.Empty));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, RestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue(Token.Ticket.TokenType, Token.Ticket.AccessToken);

            try{
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode){
                    var json = await response.Content.ReadAsStringAsync();
                    RestResponse<User> restResponse = JsonConvert.DeserializeObject<RestResponse<User>>(json);

                    if ("true".Equals(restResponse.IsSuccess)){
                        return (true, restResponse.Data);
                    }
                    else{
                        // TODO ?
                    }
                }
            }
            catch (Exception e){
                Debug.WriteLine(e.Message);
            }
            return (false, null);
        }


        //Add a place
        public async Task<(Boolean, string)> AddPlace(string title, string description, byte[] imageData, string latitude, string longitude){
            Token.RefreshIfNecessary();

            string RestUrl = "https://td-api.julienmialon.com/places/";

            // Je m'occupe en premier de l'image
            (Boolean test, int image_id) = await AddPicture(imageData);

            if (test){
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, RestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue(Token.Ticket.TokenType, Token.Ticket.AccessToken);
                string content = content = "{"
                    + "\"title\": \"" + title + "\","
                    + "\"description\": \"" + description + "\","
                    + "\"image_id\": " + image_id + ","
                    + "\"latitude\": " + latitude + ","
                    + "\"longitude\": " + longitude
                    + "}";
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");

                try{
                    var response = await client.SendAsync(request);

                    var json = await response.Content.ReadAsStringAsync();
                    RestResponse restResponse = JsonConvert.DeserializeObject<RestResponse>(json);


                    if (response.IsSuccessStatusCode){
                        if ("true".Equals(restResponse.IsSuccess)){
                            return (true, "Ajout de la place effectué.");
                        }
                        else{
                            return (false, "Votre place n'a pû être ajouté. " + restResponse.ErrorMessage);
                        }
                    }
                    else{
                        return (false, "Erreur lors de la tentative d'ajout d'une place. " + restResponse.ErrorMessage + " Veuillez réessayer.");
                    }
                }
                catch (Exception e){
                    return (false, "Erreur lors de la tentative d'ajout d'une place." + e.Message + " Veuillez réessayer.");
                }
            }
            else{
                return (false, "Une erreur est survenue lors de l'envoie de l'image. Merci de réessayer.");
            }
        }


        //Add a picture
        public async Task<(Boolean, int)> AddPicture(byte[] imageData){
            Token.RefreshIfNecessary();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://td-api.julienmialon.com/images");
            request.Headers.Authorization = new AuthenticationHeaderValue(Token.Ticket.TokenType, Token.Ticket.AccessToken);

            MultipartFormDataContent requestContent = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            requestContent.Add(imageContent, "file", "file.jpg");
            request.Content = requestContent;


            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode){
                var json = await response.Content.ReadAsStringAsync();
                RestResponse<ImageResponse> restResponse = JsonConvert.DeserializeObject<RestResponse<ImageResponse>>(json);
                return (true, restResponse.Data.IdNewImage);
            }
            else{
                return (false, 1);
            }
        }

    }
}