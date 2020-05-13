namespace BetaBank.View.Interfaces
{
    public interface IView
    {
        object DataContext { get; set; }
        void Close();
    }
}