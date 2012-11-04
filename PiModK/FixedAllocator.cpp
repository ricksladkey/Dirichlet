#include "stdafx.h"

#include "FixedAllocator.h"

#define MIXED 0

const int max_entries = 1 << 15;
const int fixed_size = 32;
struct entry
{
    entry* next;
    char memory[fixed_size];
};
entry entries[max_entries];
entry* head = 0;

void init_func()
{
    for (int i = 0; i < max_entries; i++)
        free_func(entries[i].memory, fixed_size);
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
#endif
    entry* e = head;
    head = head->next;
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
    e->next = head;
    head = e;
}

