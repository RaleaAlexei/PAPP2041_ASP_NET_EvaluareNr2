namespace Library.Models.ViewModels
{
    public class CarteUtilizatorViewModel
    {
        public CarteUtilizatorViewModel()
        {
            ListaCarti = new List<Carte>();
        }
        public Utilizator Utilizator { get; set; }
        public IList<Carte> ListaCarti { get; set; }
    }
}
