using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HttpClientMeteo
{
    class Program
    {
        public class LocationResponse
        {
            public string Key { get; set; }
        }

        public class JsonResponse
        {
            public Temperature Temperature { get; set; }
            public string WeatherText { get; set; }
        }

        public class Temperature
        {
            public Metric Metric { get; set; }
        }

        public class Metric
        {
            public double Value { get; set; }
            public string Unit { get; set; }
        }

        static async Task Main(string[] args)
        {
            string apiKey = "p7lfi1GX8mJFiG69XeowzzqIT55sFNvm";

            Console.WriteLine("Inserisci il nome della città che vuoi cercare");
            string cittaDaCercare = Console.ReadLine();

            string locationUrl = $"http://dataservice.accuweather.com/locations/v1/cities/search?apikey={apiKey}&q={cittaDaCercare}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage locationResponse = await client.GetAsync(locationUrl);

                if (locationResponse.IsSuccessStatusCode)
                {
                    string locationJsonResponse = await locationResponse.Content.ReadAsStringAsync();

                    LocationResponse[] locationResult = JsonConvert.DeserializeObject<LocationResponse[]>(locationJsonResponse);

                    if (locationResult.Length > 0)
                    {
                        string locationKey = locationResult[0].Key;

                        Console.WriteLine($"Key: {locationKey}");

                        string currentConditionsUrl = $"http://dataservice.accuweather.com/currentconditions/v1/{locationKey}?apikey={apiKey}";

                        HttpResponseMessage conditionsResponse = await client.GetAsync(currentConditionsUrl);

                        if (conditionsResponse.IsSuccessStatusCode)
                        {
                            string conditionsJsonResponse = await conditionsResponse.Content.ReadAsStringAsync();
                            JsonResponse[] conditionsResult = JsonConvert.DeserializeObject<JsonResponse[]>(conditionsJsonResponse);

                            if (conditionsResult.Length > 0)
                            {
                                Console.WriteLine($"Temperatura: {conditionsResult[0].Temperature.Metric.Value} {conditionsResult[0].Temperature.Metric.Unit}");
                                Console.WriteLine($"Condizioni meteo: {conditionsResult[0].WeatherText}");
                            }
                            else
                            {
                                Console.WriteLine("Nessun risultato trovato.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Errore nella richiesta dell'API (Condizioni meteo)");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nessun risultato trovato.");
                    }
                }
                else
                {
                    Console.WriteLine("Errore nella richiesta dell'API (Località)");
                }
            }
        }
    }
}
