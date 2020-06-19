namespace Apocalypse.Any.Core
{
    public interface ICommand<in TParam>
    {
        bool CanExecute(TParam parameters);

        void Execute(TParam parameters);
    }
}