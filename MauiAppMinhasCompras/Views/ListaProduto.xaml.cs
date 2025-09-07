using MauiAppMinhasCompras.Models;
using MauiAppMinhasCompras.Helpers;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
    List<Produto> _todos = new();

    public ListaProduto()
    {
        InitializeComponent();
        lst_produtos.ItemsSource = lista;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarTodosAsync();
    }

    private async Task CarregarTodosAsync()
    {
        var dados = await App.Db.GetAll();
        _todos = dados;
        AtualizarLista(dados);
    }

    private void AtualizarLista(IEnumerable<Produto> itens)
    {
        lista.Clear();
        foreach (var p in itens)
            lista.Add(p);
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        var q = e.NewTextValue?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(q))
        {
            AtualizarLista(_todos);
            return;
        }

        // Busca no SQLite conforme digita (LIKE)
        var resultado = await App.Db.Search(q);
        AtualizarLista(resultado);
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        // Navega para a página de novo produto, se existir
        await Navigation.PushAsync(new NovoProduto());
    }

    private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);
        string msg = $"O total é {soma:C}";
        await DisplayAlert("Total dos Produtos", msg, "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        if (sender is MenuItem mi && mi.BindingContext is Produto p)
        {
            bool ok = await DisplayAlert("Remover", $"Remover '{p.Descricao}'?", "Sim", "Não");
            if (ok)
            {
                await App.Db.Delete(p.Id);
                await CarregarTodosAsync();
            }
        }
    }
}
