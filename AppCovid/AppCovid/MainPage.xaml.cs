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
using System.Collections;

namespace AppCovid
{
    public partial class MainPage : ContentPage
    {
        List<object> Paises = new List<object>();
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

                    Paises = JsonConvert.DeserializeObject<List<object>>(objResponse.ToString());

                    foreach (object pais in Paises.OrderBy(p => p.ToString()))
                    {
                        var dados = JsonConvert.DeserializeObject<Countries>(pais.ToString());
                        pckPaises.Items.Add(dados.Country);
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
                        dadosApi.Add("Total de casos confirmados: " + dados.Confirmed.ToString());
                        dadosApi.Add("Total de mortos: " + dados.Deaths.ToString());
                        dadosApi.Add("Total de casos recuperados: " + dados.Recovered.ToString());
                        dadosApi.Add("Total de casos ativos: " + dados.Active.ToString());
                        dadosApi.Add("Data do censo: " + dados.Date.ToString("dd/MM/yyyy"));                        
                    }
                    
                    if (post.Count > 0)
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

            btnLimpar.IsVisible = false;
            GetCountries();

            if (Paises.Count > 0)
            {
                pckPaises.Title = "Selecione um país";
            }
        }

        private void btnPesquisar_Clicked(object sender, EventArgs e)
        {
            if (pckPaises.SelectedIndex != -1)
            {
                var paisSelecionado = pckPaises.Items[pckPaises.SelectedIndex];
                GetDados(paisSelecionado);
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
