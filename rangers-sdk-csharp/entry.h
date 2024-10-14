#define WIN32_LEAN_AND_MEAN

#include <new>
#include <cassert>
#include <cstdint>
#include <cstring>
#include <windows.h>
#include <d3d11.h>

namespace csl::math {
	class Vector2 {
	public:
		float x; float y;
	};

	class alignas(16) Vector3 {
	public:
		float x; float y; float z;

		inline bool operator==(const Vector3& other) const {
			return x == other.x && y == other.y && z == other.z;
		}

		inline bool operator!=(const Vector3& other) const {
			return !operator==(other);
		}
	};

	class alignas(16) Vector4 {
	public:
		float x; float y; float z; float w;
	};

	class alignas(16) Quaternion  {
	public:
		float x; float y; float z; float w;

		inline bool operator==(const Quaternion& other) const {
			return x == other.x && y == other.y && z == other.z && w == other.w;
		}

		inline bool operator!=(const Quaternion& other) const {
			return !operator==(other);
		}
	};

	class alignas(16) Matrix44 {
	public:
		Vector4 t; Vector4 u; Vector4 v; Vector4 w;
	};

	class alignas(16) Matrix34 {
	public:
		Vector4 t; Vector4 u; Vector4 v; Vector4 w;
	};

	class Position {
	public:
		float x; float y; float z;

		inline bool operator==(const Position& other) const {
			return x == other.x && y == other.y && z == other.z;
		}

		inline bool operator!=(const Position& other) const {
			return !operator==(other);
		}
	};

	class Rotation {
	public:
		float x; float y; float z; float w;

		inline bool operator==(const Rotation& other) const {
			return x == other.x && y == other.y && z == other.z && w == other.w;
		}

		inline bool operator!=(const Rotation& other) const {
			return !operator==(other);
		}
	};
}

#define NO_EIGEN_MATH

#ifndef RANGERS_SDK_CSHARP_NO_SHIMS
#define NO_METADATA
#endif

#include "../rangers-sdk/include/rangers-sdk.h"
