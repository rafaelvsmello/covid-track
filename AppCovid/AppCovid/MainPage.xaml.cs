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
        List<string> registros = new List<string>();
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

                    if (post.Count > 0)
                    {
                        btnLimpar.IsVisible = true;
                        var ultimo = post.Count - 1;

                        foreach (var item in post)
                        {
                            var dados = JsonConvert.DeserializeObject<DadosApi>(item.ToString());
                            dadosApi.Add(new DadosApi { Country = dados.Country,
                                Confirmed = dados.Confirmed,
                                Deaths = dados.Deaths,
                                Recovered = dados.Recovered,
                                Active = dados.Active,
                                Date = dados.Date
                            });
                        }

                        registros.Add("País: " + dadosApi[ultimo].Country.ToString());
                        registros.Add("Casos confirmados: " + dadosApi[ultimo].Confirmed.ToString());
                        registros.Add("Mortes: " + dadosApi[ultimo].Deaths.ToString());
                        registros.Add("Casos recuperados: " + dadosApi[ultimo].Recovered.ToString());
                        registros.Add("Casos ativos: " + dadosApi[ultimo].Active.ToString());
                        registros.Add("Data do censo: " + dadosApi[ultimo].Date.ToString("dd/MM/yyyy"));
                        lstDados.ItemsSource = registros;                        
                    }
                    else
                    {
                        DisplayAlert("Atenção", "Sem dados para exibir", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Ocorreu um erro", ex.Message, "OK");
            }
        }

        void Limpar()
        {
            btnLimpar.IsVisible = false;            
            lstDados.ItemsSource = null;
            registros.Clear();
            dadosApi.Clear();
        }

        public MainPage()
        {
            InitializeComponent();

            btnLimpar.IsVisible = false;            
            pckPaises.Title = "Selecione um país";
            GetCountries();
        }

        private void btnPesquisar_Clicked(object sender, EventArgs e)
        {
            if (pckPaises.SelectedIndex != -1)
            {
                Limpar();
                var paisSelecionado = pckPaises.Items[pckPaises.SelectedIndex];
                var slugPais = listaPaises.Find(x => x.Country.Contains(paisSelecionado));
                GetDados(slugPais.Slug);                
            }
            else
            {
                DisplayAlert("Atenção", "Selecione um país para pesquisar", "OK");
            }
        }

        private void btnLimpar_Clicked(object sender, EventArgs e)
        {
            pckPaises.SelectedIndex = -1;
            Limpar();
        }
        
    }
}
