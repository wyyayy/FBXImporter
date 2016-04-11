#pragma once

#ifdef FBXIMPORTER_BUILD
#define FBXIMPORTER_CORE __declspec( dllexport )
#else
#define FBXIMPORTER_CORE __declspec( dllimport )
#endif

//#ifdef _DEBUG
//#define _ITERATOR_DEBUG_LEVEL 2
//#else
//#define _ITERATOR_DEBUG_LEVEL 0
//#endif

#include <cassert>
#include <vector>
#include <string>
#include <unordered_map>

namespace FBXImporterUnmanaged
{
}