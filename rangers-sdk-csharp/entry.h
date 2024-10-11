#define WIN32_LEAN_AND_MEAN

#include <new>
#include <cassert>
#include <cstdint>
#include <cstring>
#include <windows.h>
#include <d3d11.h>

#ifndef RANGERS_SDK_CSHARP_NO_SHIMS

namespace csl::math {
	struct Vector2 {
		float x; float y;
	};

	struct alignas(16) Vector3 {
		float x; float y; float z;

		inline bool operator==(const Vector3& other) const {
			return x == other.x && y == other.y && z == other.z;
		}

		inline bool operator!=(const Vector3& other) const {
			return !operator==(other);
		}
	};

	struct alignas(16) Vector4 {
		float x; float y; float z; float w;
	};

	struct alignas(16) Quaternion  {
		float x; float y; float z; float w;

		inline bool operator==(const Quaternion& other) const {
			return x == other.x && y == other.y && z == other.z && w == other.w;
		}

		inline bool operator!=(const Quaternion& other) const {
			return !operator==(other);
		}
	};

	struct alignas(16) Matrix44 {
		Vector4 t; Vector4 u; Vector4 v; Vector4 w;
	};

	struct alignas(16) Matrix34 {
		Vector4 t; Vector4 u; Vector4 v; Vector4 w;
	};

	struct Position {
		float x; float y; float z;

		inline bool operator==(const Position& other) const {
			return x == other.x && y == other.y && z == other.z;
		}

		inline bool operator!=(const Position& other) const {
			return !operator==(other);
		}
	};

	struct Rotation {
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
#define NO_METADATA
#endif

#include "../rangers-sdk/include/rangers-sdk.h"
