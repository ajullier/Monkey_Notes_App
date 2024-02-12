namespace NoteWebMVC.Utilities
{
    public static class ApiRest
    {
        public static async Task<string> GetStringObjects(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {   // Realizar la solicitud GET
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Leer la respuesta como cadena de texto
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public static async Task<string> DeleteStringObjects(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {   // Realizar la solicitud GET
                HttpResponseMessage response = await client.DeleteAsync(apiUrl);
                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Leer la respuesta como cadena de texto
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public static async Task<string> PostStringObjects(string apiUrl, object content)
        {
            using (HttpClient client = new HttpClient())
            {
                // Realizar la solicitud GET
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
        public static async Task<string> PutStringObjects(string apiUrl, object content)
        {
            using (HttpClient client = new HttpClient())
            {
                // Realizar la solicitud GET
                HttpResponseMessage response = await client.PutAsJsonAsync(apiUrl, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}