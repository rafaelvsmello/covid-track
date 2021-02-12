using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Xamarin.Forms;

namespace AppCovid
{
    public partial class MainPage : ContentPage
    {
        List<Countries> listaPaises = new List<Countries>();
        List<DadosApi> dadosApi = new List<DadosApi>();

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

                    List<object> Paises = new List<object>();
                    Paises = JsonConvert.DeserializeObject<List<object>>(objResponse.ToString());

                    if (Paises.Count > 0)
                    {
                        foreach (object pais in Paises.OrderBy(p => p.ToString()))
                        {
                            var dados = JsonConvert.DeserializeObject<Countries>(pais.ToString());
                            listaPaises.Add(new Countries { Country = dados.Country, Slug = dados.Slug, ISO2 = dados.ISO2 });
                            pckPaises.Items.Add(dados.Country);
                        }

                        streamDados.Close();
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Ocorreu um erro", ex.Message, "OK");
            }
        }

        void GetDados(string pais)
        {
            try
            {
                var requisicao = WebRequest.CreateHttp($"https://api.covid19api.com/total/country/{pais}");
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
                        dadosApi.Add(new DadosApi { Country = dados.Country, Confirmed = dados.Confirmed, Active = dados.Active, Deaths = dados.Deaths });
                    }

                    //if (post.Count > 0)
                    //{
                    //    lstDados.ItemsSource = dadosApi[0].ToString();
                    //}
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

            btnLimpar.IsVisible = false;
            GetCountries();
            pckPaises.Title = "Selecione um país";
        }

        private void btnPesquisar_Clicked(object sender, EventArgs e)
        {
            if (pckPaises.SelectedIndex != -1)
            {
                var paisSelecionado = pckPaises.Items[pckPaises.SelectedIndex];
                var slugPais = listaPaises.Where(item => item.Slug.Contains(paisSelecionado));
                GetDados(slugPais.ToString());
                btnLimpar.IsVisible = true;
            }
            else
            {
                DisplayAlert("Atenção", "Selecione um país para pesquisar", "OK");
            }
        }

        private void btnLimpar_Clicked(object sender, EventArgs e)
        {
            btnLimpar.IsVisible = false;
            pckPaises.SelectedIndex = -1;
            lstDados.ItemsSource = null;
            dadosApi.Clear();
        }
    }
}
