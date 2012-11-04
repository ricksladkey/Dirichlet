class Interlocked
{
public:
    template<class T>
    static T Add(T &location, T value)
    {
        return __sync_add_and_fetch(&location, value);
    }
    template<class T>
    static T CompareExchange(T &location, T value, T comparand)
    {
        return __sync_val_compare_and_swap(&location, comparand, value);
    }
};
