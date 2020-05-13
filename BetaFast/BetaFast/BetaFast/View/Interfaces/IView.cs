namespace BetaFast.View.Interfaces
{
    public interface IView
    {
        object DataContext { get; set; }
        void Close();
    }
}
