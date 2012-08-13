#include "stdafx.h"

#include "FixedAllocator.h"

#include <pthread.h>

#define USE_FIXED_ALLOCATOR 0
#define MIXED 0
#define REENTRANT 0
#define USE_LOCK 0
#define USE_COMPARE_EXCHANGE 0
#define USE_SPINLOCK 0

static const int max_entries = 1 << 15;
static const int fixed_size = 32;
struct entry
{
    entry* next;
    char memory[fixed_size];
};
static entry entries[max_entries];
static entry* head = 0;
#if REENTRANT
#if USE_SPINLOCK
static pthread_spinlock_t mutex;
#else
static pthread_mutex_t mutex;
#endif
#endif

static inline void lock()
{
#if REENTRANT
#if USE_SPINLOCK
    pthread_spin_lock(&mutex);
#else
    pthread_mutex_lock(&mutex);
#endif
#endif
}

static inline void unlock()
{
#if REENTRANT
#if USE_SPINLOCK
    pthread_spin_unlock(&mutex);
#else
    pthread_mutex_unlock(&mutex);
#endif
#endif
}

void init_func()
{
#if REENTRANT
#if USE_SPINLOCK
    pthread_spin_init(&mutex, PTHREAD_PROCESS_PRIVATE);
#else
    pthread_mutex_init(&mutex, NULL);
#endif
#endif
#if USE_FIXED_ALLOCATOR
    for (int i = 0; i < max_entries; i++)
        free_func(entries[i].memory, fixed_size);
    mp_set_memory_functions(alloc_func, realloc_func, free_func);
#endif
}

void exit_func()
{
    mp_set_memory_functions(0, 0, 0);
}

void* alloc_func(size_t size)
{
#if MIXED
    if (size > fixed_size)
        return malloc(size);
#endif
    entry* e;
#if USE_LOCK
    lock();
#endif
#if USE_COMPARE_EXCHANGE
    do
        e = head;
    while (Interlocked::CompareExchange(head, e->next, e) != e);
#else
    e = head;
    head = e->next;
#endif
#if USE_LOCK
    unlock();
#endif
    return e->memory;
}

void* realloc_func(void* p, size_t old_size, size_t new_size)
{
#if MIXED
    if (old_size <= fixed_size && new_size <= fixed_size)
        return p;

    if (new_size > fixed_size && old_size > fixed_size)
        return realloc(p, new_size);

    if (new_size > fixed_size)
    {
        void* q = malloc(new_size);
        memcpy(q, p, old_size < new_size ? old_size : new_size);
        free_func(p, old_size);
        return q;
    }
    if (old_size > fixed_size)
    {
        void* q = alloc_func(new_size);
        memcpy(q, p, old_size < new_size ? old_size : new_size);
        free(p);
        return q;
    }
#endif
    return p;
}

void free_func(void* p, size_t size)
{
#if MIXED
    if (size > fixed_size)
        return free(p);
#endif
    entry* e = (entry*)((UInt64)p - sizeof(((entry*)0)->next));
#if USE_LOCK
    lock();
#endif
#if USE_COMPARE_EXCHANGE
    do
        e->next = head;
    while (Interlocked::CompareExchange(head, e, e->next) != e->next);
#else
    e->next = head;
    head = e;
#endif
#if USE_LOCK
    unlock();
#endif
}
