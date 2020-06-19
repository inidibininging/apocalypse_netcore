namespace Apocalypse.Any.Core.Input
{
    public interface IInputTranslator<TKeyInputType, TKeyOutputType>
    {
        TKeyOutputType Translate(TKeyInputType input);
    }

    public interface IInputTranslator<TKeyInputType, TKeyOutputType, TSharedBaseType>
        where TKeyInputType : TSharedBaseType
        where TKeyOutputType : TSharedBaseType
    {
        TKeyOutputType Translate(TKeyInputType input);
    }

    public interface IInputTranslator<TKeyInputType, TKeyOutputType, TLeftType, TRightType>
       where TKeyInputType : TLeftType
       where TKeyOutputType : TRightType
    {
        TKeyOutputType Translate(TKeyInputType input);
    }
}