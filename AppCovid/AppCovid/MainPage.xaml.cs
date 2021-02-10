using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.IO;

namespace AppCovid
{
    public partial class MainPage : ContentPage
    {
        List<string> paises = new List<string>();

        void GetCountries()
        {
            var requisicao = WebRequest.CreateHttp("https://api.covid19api.com/countries");
            requisicao.Method = "GET";
            requisicao.UserAgent = "RequiscaoCountries";

            using (var response = requisicao.GetResponse())
            {
                var streamDados = response.GetResponseStream();
                StreamReader reader = new StreamReader(streamDados);
                object objResponse = reader.ReadToEnd();

                var post = JsonConvert.DeserializeObject<List<object>>(objResponse.ToString());

                foreach (var item in post)
                {
                    var dados = JsonConvert.DeserializeObject<Countries>(item.ToString());
                    paises.Add(dados.Country);
                }

                streamDados.Close();
                reader.Close();
            }
        }

        public MainPage()
        {
            InitializeComponent();

            pckPaises.Title = "Selecione o país";

            GetCountries();

            if (paises.Count > 0)
            {
                foreach (string pais in paises)
                {
                    pckPaises.Items.Add(pais);
                }
            }            
        }
        
    }
}
