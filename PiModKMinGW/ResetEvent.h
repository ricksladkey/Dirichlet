#include <pthread.h>

class ManualResetEvent
{
public:
    ManualResetEvent( bool signalled = false, bool ar = true ) :
        _auto( ar ),
        _signalled( signalled )
    {
        pthread_mutex_init( &_mutex, NULL );
        pthread_cond_init( &_cond, NULL );
    }

    ~ManualResetEvent()
    {
        pthread_cond_destroy( &_cond );
        pthread_mutex_destroy( &_mutex );
    }

    void Set()
    {
        pthread_mutex_lock( &_mutex );

        // only set and signal if we are unset
        if ( _signalled == false )
        {
            _signalled = true;

            pthread_cond_signal( &_cond );
        }

        pthread_mutex_unlock( &_mutex );
    }

    void Wait()
    {
        pthread_mutex_lock( &_mutex );

        while ( _signalled == false )
        {
            pthread_cond_wait( &_cond, &_mutex );
        }

        // if we're an autoreset event, auto reset
        if ( _auto )
        {
            _signalled = false;
        }

        pthread_mutex_unlock( &_mutex );
    }

    void Reset()
    {
        pthread_mutex_lock( &_mutex );

        _signalled = false;

        pthread_mutex_unlock( &_mutex );
    }

private:
    pthread_mutex_t _mutex;
    pthread_cond_t _cond;
    bool _signalled;
    bool _auto;
};
