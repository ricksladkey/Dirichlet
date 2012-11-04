#include "stdafx.h"

#include "FixedAllocator.h"

#include <pthread.h>

#define VERBOSE 0
#define USE_FIXED_ALLOCATOR 0
#define MIXED 0
#define REENTRANT 0
#define USE_LOCK 0
#define USE_COMPARE_EXCHANGE 0
#define USE_SPINLOCK 0
#define VALIDATE 0
#define USE_TLS 1

static const int max_entries = 1 << 15;
static const int fixed_size = 32;
struct entry
{
    entry* next;
    char memory[fixed_size];
};
#if USE_TLS
static __thread entry* head = 0;
#else
static entry* head = 0;
#endif
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
#if VERBOSE
    printf("USE_FIXED_ALLOCATOR = %d\n", USE_FIXED_ALLOCATOR);
    printf("MIXED = %d\n", MIXED);
    printf("REENTRANT = %d\n", REENTRANT);
    printf("USE_LOCK = %d\n", USE_LOCK);
    printf("USE_COMPARE_EXCHANGE = %d\n", USE_COMPARE_EXCHANGE);
    printf("USE_SPINLOCK = %d\n", USE_SPINLOCK);
    printf("VALIDATE = %d\n", VALIDATE);
    printf("USE_TLS = %d\n", USE_TLS);
#endif
#if REENTRANT
#if USE_SPINLOCK
    pthread_spin_init(&mutex, PTHREAD_PROCESS_PRIVATE);
#else
    pthread_mutex_init(&mutex, NULL);
#endif
#endif
    mp_set_memory_functions(alloc_func, realloc_func, free_func);
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
#else
#if VALIDATE
    if (size > 16)
        printf("size = %d\n", size);
    if (size > fixed_size)
    {
        fprintf(stderr, "oversized allocation: %d\n", size);
        exit(1);
    }
#endif
#endif
    if (head == 0)
        return (void *)((entry *)malloc(sizeof(entry)))->memory;
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
#else
#if VALIDATE
    if (new_size > 16)
        printf("size = %d\n", new_size);
    if (new_size > fixed_size)
    {
        fprintf(stderr, "oversized allocation: %d\n", new_size);
        exit(1);
    }
#endif
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
