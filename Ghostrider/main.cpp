#include "main.h"

void DLL_EXPORT GhostriderInit()
{
    slow_hash_allocate_state();
}

void DLL_EXPORT GhostriderWork(int r1 ,int r2 ,int r3 ,int r4 ,int r5 ,int r6 , const char* input, char* output)
{
    gr_hash(input, output);
}



extern "C" DLL_EXPORT BOOL APIENTRY DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
    switch (fdwReason)
    {
        case DLL_PROCESS_ATTACH:
            // attach to process
            // return FALSE to fail DLL load
            break;

        case DLL_PROCESS_DETACH:
            // detach from process
            break;

        case DLL_THREAD_ATTACH:
            // attach to thread
            break;

        case DLL_THREAD_DETACH:
            // detach from thread
            break;
    }
    return TRUE; // succesful
}
