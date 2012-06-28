// PiModK.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#define MIXED 0

const int max_entries = 1 << 24;
const int fixed_size = 24;
struct entry
{
    entry* next;
    char memory[fixed_size];
};
entry entries[max_entries];
entry* head = 0;

void init_func();
void* alloc_func(size_t size);
void* realloc_func(void* p, size_t old_size, size_t new_size);
void free_func(void* p, size_t size);


void init_func()
{
    for (int i = 0; i < max_entries; i++)
        free_func(entries[i].memory, fixed_size);
    mp_set_memory_functions(alloc_func, realloc_func, free_func);
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

int _tmain(int argc, _TCHAR* argv[])
{
    init_func();

#if 0
    Integer a = 1234, b = 5678;
    Integer c = a * b;
    printf("a = %s, b = %s, c = %s\n", a.get_str().c_str(), b.get_str().c_str(), c.get_str().c_str());
#endif

    DivisorSummatoryFunctionOdd algorithm;
    CStopWatch timer;
    for (int i = 20; i <= 20; i++)
    {
        Integer n = Power(Integer(10), i);
        Integer x2 = sqrt(n);
        timer.startTimer();
        Integer s = algorithm.Evaluate(n, 1, x2);
        timer.stopTimer();
        std::string sRep = s.get_str();
        printf("i = %d, s = %s, elapsed = %g\n", i, sRep.c_str(), timer.getElapsedTime() * 1000);
    }
	return 0;
}

