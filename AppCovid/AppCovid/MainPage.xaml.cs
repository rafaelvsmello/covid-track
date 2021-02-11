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
        List<string> dadosApi = new List<string>();

        void GetCountries()
        {
            try
            {
                var requisicao = WebRequest.CreateHttp("https://api.covid19api.com/countries");
                requisicao.Method = "GET";
                requisicao.UserAgent = "RequisicaoCountries";

                using (var response = requisicao.GetResponse())
                {
                    var streamDados = response.GetResponseStream();
                    StreamReader reader = new StreamReader(streamDados);
                    object objResponse = reader.ReadToEnd();

                    var post = JsonConvert.DeserializeObject<List<object>>(objResponse.ToString());

                    foreach (var item in post)
                    {
                        var dados = JsonConvert.DeserializeObject<Countries>(item.ToString());
                        paises.Add(dados.Slug);
                    }

                    streamDados.Close();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Ocorreu um erro", ex.Message, "OK");                
            }            
        }

        void GetDados(string pais)
        {
            DateTime dataInicio = DateTime.Now.AddDays(-1);
            string dtInicio = dataInicio.ToString("yyyy-MM-dd");
            string dtFim = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                var requisicao = WebRequest.CreateHttp($"https://api.covid19api.com/country/{pais}/status/confirmed?from={dtInicio}T00:00:00Z&to={dtFim}T00:00:00Z");
                requisicao.Method = "GET";
                requisicao.UserAgent = "RequisicaoDados";

                using (var response = requisicao.GetResponse())
                {
                    var streamDados = response.GetResponseStream();
                    StreamReader reader = new StreamReader(streamDados);
                    object objResponse = reader.ReadToEnd();

                    var post = JsonConvert.DeserializeObject<List<object>>(objResponse.ToString());

                    foreach (var item in post)
                    {
                        var dados = JsonConvert.DeserializeObject<DadosApi>(item.ToString());
                        dadosApi.Add("País: " + dados.Country);
                        dadosApi.Add("Total de casos: " + dados.Cases.ToString());
                        dadosApi.Add("Data do censo: " + dados.Date.ToString("dd/MM/yyyy"));
                    }

                    if (dadosApi.Count > 0)
                    {
                        lstDados.ItemsSource = dadosApi;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Ocorreu um erro", ex.Message, "OK");
            }
        }

        public MainPage()
        {
            InitializeComponent();

            pckPaises.Title = "Selecione o país";            

            GetCountries();

            if (paises.Count > 0)
            {
                foreach (string pais in paises.OrderBy(p => p).ToList())
                {
                    pckPaises.Items.Add(pais);
                }
            }            
        }

        private void btnPesquisar_Clicked(object sender, EventArgs e)
        {
            if (pckPaises.SelectedIndex != -1)
            {
                var paisSelecionado = pckPaises.Items[pckPaises.SelectedIndex];
                GetDados(paisSelecionado);
            }
            else
            {
                DisplayAlert("Atenção", "Selecione um país para pesquisar", "OK");
            }
        }

        private void btnLimpar_Clicked(object sender, EventArgs e)
        {
            pckPaises.SelectedIndex = -1;
            lstDados.ItemsSource = null;
            dadosApi.Clear();
        }
    }
}
