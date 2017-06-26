/// <summary>
/// 不用继承monobehaviour的单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class NormalSingleton<T> where T : new()
{
    private static T instance;
    private static readonly object objlock = new object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (objlock)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                }
            }
            return instance;
        }
    }
}
