class Interlocked
{
public:
    static int Add(int &location, int value)
    {
        return __sync_add_and_fetch(&location, value);
    }
};
