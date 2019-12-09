namespace Common
{
    public interface IScriptablePostProcess
    {
        bool OnPostProcess(bool isDeleted);
    }
}